using System.Drawing;
using static LVS3.Delegates;

namespace LVS3;

/// <summary>
/// Minimal dummy inspection for offline testing without hardware.
/// </summary>
public class DummyInspection : IInspection
{
    public bool EnableCapture { get; set; }
    public bool SampleIncluded { get; set; }
    public bool FixedData { get; set; }
    public string FailFolder { get; set; } = "";
    public int LabelCount { get; set; }
    public event AlarmMethodHandler? Amh;

    public void MoveNextCaller(Action movenextcaller) { }
    public bool LoadInspectionDataFromDb(string lpn, string labelitem) => true;
    public bool InitInspection(InspectionContext context) => true;
    public bool LoadVdeToolsAndData(string reelLpn, string lin, string lwo) => true;
    public List<VdeDisplayItem> GetVdeDisplayItems() => new();
    public bool LoadMedDataList(string lin, string lpn, string lwo) => true;
    public List<MedDataDisplayItem> GetMedDisplayItems() => new();
    public bool LoadInspectionVdeItemParams(string lin) => true;
    public bool InspectLabel(Bitmap img, ref FailRecord fp) => true;
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
}
