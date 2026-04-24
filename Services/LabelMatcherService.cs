using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCVComMatcherLib;
using Serilog;

namespace WpfMvvmApp.Services;

/// <summary>
/// One element returned by CreateAbsoluteMap.
/// </summary>
public record AbsoluteElement(
    string Type,        // "TEXT" or "IMAGE"
    string Text,
    int X, int Y, int Width, int Height,
    string FontName, double FontSize, bool IsBold, bool IsItalic);

/// <summary>
/// Parsed result of CreateAbsoluteMap.
/// </summary>
public class AbsoluteMapResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = "";
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public int CwRotations { get; set; }
    public bool Suitable { get; set; }
    public double SuitabilityScore { get; set; }
    public string SuitabilityReason { get; set; } = "";
    public List<AbsoluteElement> Elements { get; set; } = new();
}

/// <summary>
/// .NET wrapper around the OpenCVComMatcherLib COM DLL.
/// Stores the last AbsoluteMapResult as a static field so downstream
/// inspection code can access it without passing it around.
/// </summary>
public static class LabelMatcher
{
    private static readonly ILogger _log = Log.ForContext(typeof(LabelMatcher));

    /// <summary>Last result from CreateAbsoluteMap. Set after a successful LAF load.</summary>
    public static AbsoluteMapResult? LastMap { get; private set; }

    private static IImageMatcher CreateMatcher()
    {
        // Activate through the coclass from the tlbimp-generated interop. Goes
        // through the interop stubs rather than the dynamic binder, so out-SAFEARRAY
        // parameters marshal correctly.
        return (IImageMatcher)new ImageMatcher();
    }

    // Colours for box drawing
    private static readonly System.Drawing.Color TextBoxColour = System.Drawing.Color.FromArgb(0, 220, 0);   // green
    private static readonly System.Drawing.Color SearchBoxColour = System.Drawing.Color.Black;
    private const float HorizontalExpansion = 0.40f; // 40 % either side

    /// <summary>
    /// Renders the L1 PDF to a bitmap, runs CreateAbsoluteMap against that
    /// image, draws annotated boxes, stores the result and returns the
    /// annotated image ready for display.
    /// Returns null if the COM DLL is not available or the PDF fails.
    /// </summary>
    public static BitmapImage? ProcessLaf1(string l1PdfPath, string l2PdfPath = "", double dpi = 300.0)
    {
        try
        {
            IImageMatcher matcher = CreateMatcher();

            // Step 1: render the PDF to a bitmap. tlbimp exposes the SAFEARRAY
            // out parameter as System.Array; we convert to byte[] for downstream use.
            Array imgArray;
            int w, h, ch;
            bool rendered = matcher.RenderPdfPage(l1PdfPath, dpi, 0,
                out imgArray, out w, out h, out ch);
            byte[] imgBytes = (byte[])imgArray;
            if (!rendered || imgBytes == null)
            {
                _log.Warning("LabelMatcher.ProcessLaf1: RenderPdfPage returned no image for {Path}", l1PdfPath);
                return null;
            }

            // Step 2: CreateAbsoluteMap
            string json;
            bool mapCreated = matcher.CreateAbsoluteMap(
                imgBytes, w, h, ch,
                l1PdfPath, l2PdfPath, dpi,
                false, "", out json);
            _ = mapCreated;

            var mapResult = ParseJson(json);
            LastMap = mapResult;
            _log.Information("LabelMatcher: CreateAbsoluteMap {Ok}, {N} elements",
                mapResult.Success, mapResult.Elements.Count);
            _log.Information("LabelMatcher: Suitable={Suitable} Score={Score:F2} Reason={Reason}",
                mapResult.Suitable, mapResult.SuitabilityScore, mapResult.SuitabilityReason);

            if (!mapResult.Success)
            {
                _log.Warning("LabelMatcher: {Error}", mapResult.ErrorMessage);
                return null;
            }

            // Step 3: draw annotated boxes onto the rendered bitmap
            var annotated = DrawBoxes(imgBytes, w, h, ch, mapResult);
            return BitmapToBitmapImage(annotated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "LabelMatcher.ProcessLaf1 failed: {Message}", ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Renders the PDF at <paramref name="dpi"/>, uses ComputeContentRect to find the label
    /// content area, clips the rendered image to that region, and returns the clipped BitmapImage.
    /// Falls back to the full rendered image if ComputeContentRect fails.
    /// </summary>
    public static BitmapImage? CaptureClippedLaf(string pdfPath, double dpi = 300.0)
    {
        try
        {
            _log.Information("CaptureClippedLaf: start {Path} @ {Dpi} dpi", pdfPath, dpi);
            IImageMatcher matcher = CreateMatcher();
            _log.Information("CaptureClippedLaf: COM activated, calling RenderPdfPage");

            Array imgArray;
            int w, h, ch;
            bool rendered = matcher.RenderPdfPage(pdfPath, dpi, 0,
                out imgArray, out w, out h, out ch);
            byte[]? imgBytes = imgArray as byte[];
            _log.Information("CaptureClippedLaf: RenderPdfPage returned {Ok}, {W}x{H}x{C}, {Bytes} bytes",
                rendered, w, h, ch, imgBytes?.Length ?? 0);

            if (!rendered || imgBytes == null)
            {
                _log.Warning("CaptureClippedLaf: RenderPdfPage failed for {Path}", pdfPath);
                return null;
            }

            var bmp = RawBytesToBitmap(imgBytes, w, h, ch);
            SaveDebugImage(bmp, pdfPath, "raw");

            _log.Information("CaptureClippedLaf: calling ComputeContentRect");
            double minX, minY, maxX, maxY;
            bool rectOk = matcher.ComputeContentRect(pdfPath,
                out minX, out minY, out maxX, out maxY);
            _log.Information("CaptureClippedLaf: ComputeContentRect returned {Ok}, rect=({MinX},{MinY})-({MaxX},{MaxY})",
                rectOk, minX, minY, maxX, maxY);

            if (!rectOk || maxX <= minX || maxY <= minY)
            {
                _log.Warning("CaptureClippedLaf: ComputeContentRect failed for {Path}, using full image", pdfPath);
                return BitmapToBitmapImage(bmp);
            }

            // ComputeContentRect returns screen-space coords: origin top-left, y increases downward.
            // Floor the origin so we never clip inside the boundary;
            // ceil the extents so we never drop a pixel at the right/bottom edge.
            double scale = dpi / 72.0;
            int clipX = (int)Math.Floor(minX * scale);
            int clipY = (int)Math.Floor(minY * scale);
            int clipW = (int)Math.Ceiling((maxX - minX) * scale);
            int clipH = (int)Math.Ceiling((maxY - minY) * scale);

            clipX = Math.Max(0, Math.Min(clipX, w - 1));
            clipY = Math.Max(0, Math.Min(clipY, h - 1));
            clipW = Math.Max(1, Math.Min(clipW, w - clipX));
            clipH = Math.Max(1, Math.Min(clipH, h - clipY));

            _log.Information("CaptureClippedLaf: clip ({X},{Y},{W},{H}) from {Pw}x{Ph}px",
                clipX, clipY, clipW, clipH, w, h);

            var cropped = bmp.Clone(new Rectangle(clipX, clipY, clipW, clipH), bmp.PixelFormat);
            SaveDebugImage(cropped, pdfPath, "clipped");
            return BitmapToBitmapImage(cropped);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "CaptureClippedLaf failed: {Message}", ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Writes <paramref name="bmp"/> to C:\LVSDEBUG\<pdfname>_<tag>_<timestamp>.png.
    /// Silent on failure — diagnostic only.
    /// </summary>
    private static void SaveDebugImage(Bitmap bmp, string pdfPath, string tag)
    {
        try
        {
            const string dir = @"C:\LVSDEBUG";
            System.IO.Directory.CreateDirectory(dir);
            string stem = System.IO.Path.GetFileNameWithoutExtension(pdfPath);
            string name = $"{stem}_{tag}_{DateTime.Now:HHmmss_fff}.png";
            string full = System.IO.Path.Combine(dir, name);
            bmp.Save(full, ImageFormat.Png);
            _log.Information("SaveDebugImage: wrote {Path} ({W}x{H})", full, bmp.Width, bmp.Height);
        }
        catch (Exception ex)
        {
            _log.Warning(ex, "SaveDebugImage failed for tag={Tag}: {Message}", tag, ex.Message);
        }
    }

    // ── private helpers ──────────────────────────────────────────────────────

    private static AbsoluteMapResult ParseJson(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json).RootElement;
            var result = new AbsoluteMapResult
            {
                Success = doc.GetProperty("success").GetBoolean(),
                ErrorMessage = doc.GetProperty("errorMessage").GetString() ?? "",
                ImageWidth = doc.GetProperty("imageWidth").GetInt32(),
                ImageHeight = doc.GetProperty("imageHeight").GetInt32(),
                CwRotations = doc.GetProperty("cwRotations").GetInt32(),
            };

            if (doc.TryGetProperty("suitable", out var suitableProp))
                result.Suitable = suitableProp.GetBoolean();
            if (doc.TryGetProperty("suitabilityScore", out var scoreProp))
                result.SuitabilityScore = scoreProp.GetDouble();
            if (doc.TryGetProperty("suitabilityReason", out var reasonProp))
                result.SuitabilityReason = reasonProp.GetString() ?? "";

            foreach (var e in doc.GetProperty("elements").EnumerateArray())
            {
                result.Elements.Add(new AbsoluteElement(
                    Type: e.GetProperty("type").GetString() ?? "TEXT",
                    Text: e.GetProperty("text").GetString() ?? "",
                    X: e.GetProperty("x").GetInt32(),
                    Y: e.GetProperty("y").GetInt32(),
                    Width: e.GetProperty("width").GetInt32(),
                    Height: e.GetProperty("height").GetInt32(),
                    FontName: e.GetProperty("fontName").GetString() ?? "",
                    FontSize: e.GetProperty("fontSize").GetDouble(),
                    IsBold: e.GetProperty("isBold").GetBoolean(),
                    IsItalic: e.GetProperty("isItalic").GetBoolean()));
            }
            return result;
        }
        catch (Exception ex)
        {
            return new AbsoluteMapResult
            { Success = false, ErrorMessage = $"JSON parse error: {ex.Message}" };
        }
    }

    private static Bitmap DrawBoxes(byte[] raw, int w, int h, int ch, AbsoluteMapResult map)
    {
        var bmp = RawBytesToBitmap(raw, w, h, ch);

        using var g = Graphics.FromImage(bmp);
        var greenPen = new System.Drawing.Pen(TextBoxColour, 2);
        var blackPen = new System.Drawing.Pen(SearchBoxColour, 1);

        foreach (var el in map.Elements)
        {
            if (el.Type != "TEXT") continue;

            // Green box: exact element bounds for every text element
            g.DrawRectangle(greenPen, el.X, el.Y, el.Width, el.Height);

            // Black box: horizontally expanded search region, only for
            // placeholder elements (text contains '<' or '>')
            if (el.Text.Contains('<') || el.Text.Contains('>'))
            {
                int expand = (int)(el.Width * HorizontalExpansion);
                int sx = el.X - expand;
                int sw = el.Width + expand * 2;
                g.DrawRectangle(blackPen, sx, el.Y, sw, el.Height);
            }
        }
        return bmp;
    }

    private static Bitmap RawBytesToBitmap(byte[] raw, int w, int h, int ch)
    {
        var bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        var bd = bmp.LockBits(new Rectangle(0, 0, w, h),
                               ImageLockMode.WriteOnly,
                               System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        int stride = Math.Abs(bd.Stride);
        for (int y = 0; y < h; y++)
            Marshal.Copy(raw, y * w * ch, bd.Scan0 + y * stride, w * ch);
        bmp.UnlockBits(bd);
        return bmp;
    }

    private static BitmapImage BitmapToBitmapImage(Bitmap bmp)
    {
        using var ms = new System.IO.MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        ms.Position = 0;
        var bi = new BitmapImage();
        bi.BeginInit();
        bi.CacheOption = BitmapCacheOption.OnLoad;
        bi.StreamSource = ms;
        bi.EndInit();
        bi.Freeze();
        return bi;
    }
}
