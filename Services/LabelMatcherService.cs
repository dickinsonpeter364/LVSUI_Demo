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
/// Strategy used by <see cref="LabelMatcher.CaptureClippedLaf"/> to compute
/// the clip region. L1 LAFs are always clipped by largest rectangle; L2 LAFs
/// are always clipped by trim lines.
/// </summary>
public enum ClipMode
{
    LargestRectangle,
    TrimLines,
}

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

            // Step 3: draw annotated boxes onto the full rendered bitmap
            var annotated = RawBytesToBitmap(imgBytes, w, h, ch);
            DrawAnnotations(annotated, mapResult, clipX: 0, clipY: 0);
            return BitmapToBitmapImage(annotated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "LabelMatcher.ProcessLaf1 failed: {Message}", ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Renders the PDF at <paramref name="dpi"/>, clips the rendered image, and
    /// returns the clipped BitmapImage. If <paramref name="mode"/> is null (the
    /// default) the clip strategy is inferred from the file name: names
    /// starting with "L2" use trim lines, everything else uses the largest
    /// rectangle. For largest-rectangle mode, element boxes from
    /// CreateAbsoluteMap are drawn on top; trim-lines mode is clip-only.
    /// </summary>
    public static BitmapImage? CaptureClippedLaf(string pdfPath, ClipMode? mode = null, double dpi = 300.0)
    {
        try
        {
            ClipMode effectiveMode = mode ?? InferClipModeFromFilename(pdfPath);
            _log.Information("CaptureClippedLaf: start {Path} @ {Dpi} dpi, mode={Mode}{Source}",
                pdfPath, dpi, effectiveMode, mode is null ? " (inferred from filename)" : " (caller override)");
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
            SaveDebugImage(bmp, pdfPath, effectiveMode == ClipMode.TrimLines ? "l2_raw" : "raw");

            double minX, minY, maxX, maxY;
            bool rectOk = ComputeClipRect(matcher, pdfPath, effectiveMode,
                out minX, out minY, out maxX, out maxY);

            if (!rectOk || maxX <= minX || maxY <= minY)
            {
                _log.Warning("CaptureClippedLaf: clip rect computation failed for {Path} (mode={Mode}), using full image",
                    pdfPath, effectiveMode);
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
            SaveDebugImage(cropped, pdfPath, effectiveMode == ClipMode.TrimLines ? "l2_clipped" : "clipped");

            // L2 (TrimLines) is a clip-only preview; no element map / annotations.
            if (effectiveMode == ClipMode.TrimLines)
                return BitmapToBitmapImage(cropped);

            // L1 (LargestRectangle): run CreateAbsoluteMap against the raw (pre-clip)
            // image to get text elements via Tesseract, and draw them on the cropped bitmap.
            _log.Information("CaptureClippedLaf: calling CreateAbsoluteMap");
            string json = "";
            bool mapOk = matcher.CreateAbsoluteMap(
                imgArray, w, h, ch,
                pdfPath, "", dpi,
                false, "", out json);
            _log.Information("CaptureClippedLaf: CreateAbsoluteMap returned {Ok}", mapOk);

            var mapResult = ParseJson(json);
            LastMap = mapResult;
            _log.Information("LabelMatcher: CreateAbsoluteMap {Ok}, {N} elements",
                mapResult.Success, mapResult.Elements.Count);
            _log.Information("LabelMatcher: Suitable={Suitable} Score={Score:F2} Reason={Reason}",
                mapResult.Suitable, mapResult.SuitabilityScore, mapResult.SuitabilityReason);

            if (mapResult.Success)
            {
                DrawAnnotations(cropped, mapResult, clipX, clipY);
                SaveDebugImage(cropped, pdfPath, "annotated");
            }

            return BitmapToBitmapImage(cropped);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "CaptureClippedLaf failed: {Message}", ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Infers clip mode from the PDF filename: files beginning with "L2" (case
    /// insensitive) use trim lines; everything else uses the largest rectangle.
    /// Whitespace, directory separators and non-alphanumeric characters are
    /// skipped so "L2_foo.pdf", "l2-bar.pdf" and "  L2baz.pdf" all match.
    /// </summary>
    private static ClipMode InferClipModeFromFilename(string pdfPath)
    {
        string stem = System.IO.Path.GetFileNameWithoutExtension(pdfPath ?? "").TrimStart();
        if (stem.Length >= 2 &&
            (stem[0] == 'L' || stem[0] == 'l') &&
            stem[1] == '2')
        {
            return ClipMode.TrimLines;
        }
        return ClipMode.LargestRectangle;
    }

    /// <summary>
    /// Computes the clip rectangle according to <paramref name="mode"/>.
    /// LargestRectangle -> calls the existing IImageMatcher.ComputeContentRect.
    /// TrimLines -> attempts to call a ComputeTrimLines method via dynamic
    /// dispatch; if the C++ side has not yet exposed that method, logs a
    /// warning and falls back to ComputeContentRect so L2 still renders something.
    /// </summary>
    private static bool ComputeClipRect(IImageMatcher matcher, string pdfPath, ClipMode mode,
        out double minX, out double minY, out double maxX, out double maxY)
    {
        if (mode == ClipMode.TrimLines)
        {
            if (TryComputeTrimLines(matcher, pdfPath, out minX, out minY, out maxX, out maxY))
            {
                _log.Information("CaptureClippedLaf: ComputeTrimLines rect=({MinX},{MinY})-({MaxX},{MaxY})",
                    minX, minY, maxX, maxY);
                return true;
            }
            _log.Warning("CaptureClippedLaf: ComputeTrimLines unavailable, falling back to ComputeContentRect. " +
                         "Add a ComputeTrimLines method to IImageMatcher (OpenCVComMatcher.idl) to enable trim-line clipping for L2.");
        }

        _log.Information("CaptureClippedLaf: calling ComputeContentRect");
        bool ok = matcher.ComputeContentRect(pdfPath, out minX, out minY, out maxX, out maxY);
        _log.Information("CaptureClippedLaf: ComputeContentRect returned {Ok}, rect=({MinX},{MinY})-({MaxX},{MaxY})",
            ok, minX, minY, maxX, maxY);
        return ok;
    }

    /// <summary>
    /// Attempts to call a ComputeTrimLines method on the COM object via dynamic
    /// dispatch. Returns false if the method does not exist or throws.
    /// </summary>
    private static bool TryComputeTrimLines(IImageMatcher matcher, string pdfPath,
        out double minX, out double minY, out double maxX, out double maxY)
    {
        minX = minY = maxX = maxY = 0;
        try
        {
            dynamic dyn = matcher;
            bool ok = dyn.ComputeTrimLines(pdfPath, out minX, out minY, out maxX, out maxY);
            return ok;
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
        {
            return false;
        }
        catch (Exception ex)
        {
            _log.Warning(ex, "ComputeTrimLines dynamic call failed: {Message}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Draws boxes on <paramref name="bmp"/> for elements from <paramref name="map"/>.
    /// TEXT elements get a green rectangle at their exact bounds. Every other element
    /// (e.g. IMAGE) gets a black rectangle extended horizontally by 40% on each side.
    /// Element coordinates are in the full (pre-clip) page, so we offset by the
    /// clip origin and skip elements that fall wholly outside the bitmap.
    /// </summary>
    private static void DrawAnnotations(Bitmap bmp, AbsoluteMapResult map, int clipX, int clipY)
    {
        using var g = Graphics.FromImage(bmp);
        using var greenPen = new System.Drawing.Pen(TextBoxColour, 2);
        using var blackPen = new System.Drawing.Pen(SearchBoxColour, 2);

        int text = 0, other = 0;
        foreach (var el in map.Elements)
        {
            int x = el.X - clipX;
            int y = el.Y - clipY;

            if (x + el.Width < 0 || y + el.Height < 0 ||
                x >= bmp.Width || y >= bmp.Height)
                continue;

            if (el.Type == "TEXT")
            {
                g.DrawRectangle(greenPen, x, y, el.Width, el.Height);
                text++;
            }
            else
            {
                // Extend the box rightward by up to 40% of its width, but cap the
                // right edge just before any other element that's vertically
                // overlapping and positioned to the right.
                int desiredRight = el.X + el.Width + (int)(el.Width * HorizontalExpansion);
                int actualRight = desiredRight;

                foreach (var neighbour in map.Elements)
                {
                    if (ReferenceEquals(neighbour, el)) continue;
                    // no vertical overlap -> ignore
                    if (neighbour.Y + neighbour.Height <= el.Y ||
                        neighbour.Y >= el.Y + el.Height) continue;
                    // not to the right of el -> ignore
                    if (neighbour.X < el.X + el.Width) continue;

                    if (neighbour.X < actualRight)
                        actualRight = neighbour.X;
                }

                int width = Math.Max(el.Width, actualRight - el.X);
                g.DrawRectangle(blackPen, x, y, width, el.Height);
                other++;
            }
        }
        _log.Information("DrawAnnotations: drew {Text} TEXT (green), {Other} non-TEXT (black, +40% right, capped at neighbours)",
            text, other);
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
