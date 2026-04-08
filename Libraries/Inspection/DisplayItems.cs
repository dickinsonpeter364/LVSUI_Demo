namespace LVS3;

/// <summary>
/// UI-agnostic representation of a VDE item for display.
/// Replaces the WinForms UserControl that was added to FlowLayoutPanel.
/// </summary>
public class VdeDisplayItem
{
    public string Name { get; set; } = "";
    public string OpZoneName { get; set; } = "";
    public string Placeholder { get; set; } = "";
    public string FontName { get; set; } = "";
    public int[] Region { get; set; } = Array.Empty<int>();
    public bool IsVDE { get; set; }
    public bool IsBarcode2D { get; set; }
    public bool IsBarcodeLinear { get; set; }
    public int RotatedAngle { get; set; }
}

/// <summary>
/// UI-agnostic representation of a MED data item for display.
/// </summary>
public class MedDataDisplayItem
{
    public string Placeholder { get; set; } = "";
    public string Data { get; set; } = "";
    public int Repeat { get; set; }
}
