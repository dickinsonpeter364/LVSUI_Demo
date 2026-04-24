using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

    // OpenCVComMatcher.ImageMatcher CLSID — see Libraries/ImageProc/OpenCVComMatcher/ImageMatcher.rgs
    // ProgID lookup can't be used because the .rgs does not declare a ProgID entry.
    private static readonly Guid ImageMatcherClsid = new Guid("456F81F6-6DCC-4DC9-BE23-55A18E67580B");

    private static dynamic CreateMatcher()
    {
        var type = Type.GetTypeFromCLSID(ImageMatcherClsid)
            ?? throw new InvalidOperationException(
                $"OpenCVComMatcherLib CLSID {ImageMatcherClsid:B} not registered. " +
                "Run elevated-build.ps1 -SkipBuild as Administrator.");
        return Activator.CreateInstance(type)!;
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
            dynamic matcher = CreateMatcher();

            // Step 1: render the PDF to a bitmap
            byte[] imgBytes;
            int w, h, ch;
            bool rendered = matcher.RenderPdfPage(l1PdfPath, dpi, 0,
                out imgBytes, out w, out h, out ch);
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
            dynamic matcher = CreateMatcher();

            byte[] imgBytes;
            int w, h, ch;
            bool rendered = matcher.RenderPdfPage(pdfPath, dpi, 0,
                out imgBytes, out w, out h, out ch);
            if (!rendered || imgBytes == null)
            {
                _log.Warning("CaptureClippedLaf: RenderPdfPage failed for {Path}", pdfPath);
                return null;
            }

            double minX, minY, maxX, maxY;
            bool rectOk = matcher.ComputeContentRect(pdfPath,
                out minX, out minY, out maxX, out maxY);

            var bmp = RawBytesToBitmap(imgBytes, w, h, ch);

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
            return BitmapToBitmapImage(cropped);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "CaptureClippedLaf failed: {Message}", ex.Message);
            return null;
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
