using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Serilog;
using static LVS3.Delegates;
using ILogger = Serilog.ILogger;

namespace LVS3;

/// <summary>
/// Minimal inspection implementation for production-online machines without
/// the full Halcon/OpenCV-backed inspection pipeline. Its only job is to
/// capture images passed into it and save them to a configured folder.
/// </summary>
public class DummyInspection : IInspection
{
    private static readonly ILogger _logger = Log.ForContext(typeof(DummyInspection));

    private string _reelLpn = "";
    private int _imageCount;
    private InspectionContext? _ctx;

    public bool EnableCapture { get; set; }
    public bool SampleIncluded { get; set; }
    public bool FixedData { get; set; }
    public string FailFolder { get; set; } = "";
    public int LabelCount { get; set; }

    public event AlarmMethodHandler? Amh;

    public void MoveNextCaller(Action movenextcaller) { }
    public bool LoadInspectionDataFromDb(string lpn, string labelitem)
    {
        _reelLpn = lpn;
        _imageCount = 0;
        return true;
    }

    public bool InitInspection(InspectionContext context)
    {
        _ctx = context;
        _reelLpn = context.ReelLpn;
        _imageCount = 0;
        return true;
    }

    public bool LoadVdeToolsAndData(string reelLpn, string lin, string lwo)
    {
        _reelLpn = reelLpn;
        return true;
    }

    public List<VdeDisplayItem> GetVdeDisplayItems() => new();
    public bool LoadMedDataList(string lin, string lpn, string lwo) => true;
    public List<MedDataDisplayItem> GetMedDisplayItems() => new();
    public bool LoadInspectionVdeItemParams(string lin) => true;

    public bool InspectLabel(Bitmap img, ref FailRecord fp)
    {
        SaveImage(img);
        return true;
    }

    public bool InspectAndMaskMasks(ref Bitmap img, ref FailRecord fp) => true;
    public bool InspectBarcodes2D(ref Bitmap img, ref FailRecord fp) => true;
    public bool MaskBarcodes2D(ref Bitmap img, ref FailRecord fp) => true;
    public bool MaskBarcodesLinear(ref Bitmap img, ref FailRecord fp) => true;
    public bool InspectBarcodesLinear(ref Bitmap img, ref FailRecord fp) => true;
    public void GetImageInspection() { }
    public bool MoveToNextVde() => true;
    public string PreviousMedData() => "";
    public void MoveLast() { }
    public string ThisMedData() => "";
    public void SetPauseCaller(Action pausemethodcaller) { }
    public void SetAlarmCaller(Action alarmmethodcaller) { }
    public void ClearFails() { }
    public void ClearResultData(string reelLpn, string lin) { }

    /// <summary>
    /// Saves a captured image to AppData.SavedImagesPath/&lt;ReelLpn&gt;/&lt;n&gt;.bmp.
    /// If SavedImagesPath is empty, uses &lt;baseDir&gt;/ImageDump.
    /// No-op if AppData.SaveImages is false.
    /// </summary>
    private void SaveImage(Bitmap? img)
    {
        if (img == null) return;
        if (!AppData.SaveImages) return;

        try
        {
            string root = string.IsNullOrEmpty(AppData.SavedImagesPath)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageDump")
                : AppData.SavedImagesPath;

            string folder = string.IsNullOrEmpty(_reelLpn) ? root : Path.Combine(root, _reelLpn);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _imageCount++;
            string path = Path.Combine(folder, $"{_imageCount}.bmp");
            img.Save(path, ImageFormat.Bmp);
            _logger.Information("DummyInspection saved image {Path}", path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "DummyInspection.SaveImage failed: {Message}", ex.Message);
        }
    }
}
