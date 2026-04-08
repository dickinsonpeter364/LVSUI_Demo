using static LVS3.Delegates;

namespace LVS3;

/// <summary>
/// Replaces the FrmInspect parameter in IInspection.InitInspection().
/// Provides all the callbacks and data the inspection engine needs
/// without any WinForms dependency.
/// </summary>
public class InspectionContext
{
    /// <summary>Reel LPN being inspected.</summary>
    public string ReelLpn { get; set; } = "";

    /// <summary>Callback to display info text (replaces Label._lblInfoText.Text).</summary>
    public Action<string>? OnInfoTextChanged { get; set; }

    /// <summary>Callback to display time text (replaces Label._lblTime.Text).</summary>
    public Action<string>? OnTimeTextChanged { get; set; }

    /// <summary>Callback to enable/disable the end-inspection button (replaces Button._cmdEndInspection.Enabled).</summary>
    public Action<bool>? OnEndInspectionEnabled { get; set; }

    /// <summary>Callback to display pass/fail image (replaces PictureBox._pbPassFail.Image).
    /// Parameter is a System.Drawing.Bitmap or null to clear.</summary>
    public Action<object?>? OnPassFailImageChanged { get; set; }

    /// <summary>Callback to display an inspection image (replaces object display).
    /// Parameter is the Bitmap image to display.</summary>
    public Action<object?>? OnImageDisplay { get; set; }

    /// <summary>Callback to clear the inspection image display.</summary>
    public Action? OnImageClear { get; set; }

    /// <summary>Callback for system messages (replaces uscMessageDisplay.SystemMessage).</summary>
    public Action<SystemMessageEventArgs>? OnSystemMessage { get; set; }

    /// <summary>Callback for alarm events (replaces FrmInspect alarm handler).</summary>
    public AlarmMethodHandler? OnAlarm { get; set; }

    /// <summary>Callback for error messages (replaces FrmInspect error handler).</summary>
    public ErrorMethodHandler? OnError { get; set; }

    /// <summary>
    /// Callback to present a review dialog to the operator (replaces FrmUnderInvestigation).
    /// Returns true if operator accepted the label, false if rejected.
    /// </summary>
    public Func<ReviewLabelRequest, bool>? OnReviewLabelRequested { get; set; }

    /// <summary>Injected PLC client (replaces FrmSelect.MxClient).</summary>
    public ImxClient? MxClient { get; set; }
}

/// <summary>
/// Data passed to the review dialog callback.
/// </summary>
public class ReviewLabelRequest
{
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public object? Image { get; set; }
    public FailRecord? FailRecord { get; set; }
}
