namespace WpfMvvmApp.Core.Messages;

/// <summary>
/// Published when inspection state changes (start, stop, label result).
/// Replaces static SYSTEM_IO.PROCESSING flag.
/// </summary>
public sealed class InspectionStatusMessage
{
    public bool IsProcessing { get; }
    public int LabelIndex { get; }
    public string? Result { get; }

    public InspectionStatusMessage(bool isProcessing, int labelIndex = 0, string? result = null)
    {
        IsProcessing = isProcessing;
        LabelIndex = labelIndex;
        Result = result;
    }
}
