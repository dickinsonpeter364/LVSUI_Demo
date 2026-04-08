namespace WpfMvvmApp.Core.Messages;

public enum MessageSeverity
{
    Information,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Replaces legacy MSGEventArgs / SystemMessageEventArgs for cross-component notifications.
/// </summary>
public sealed class SystemMessage
{
    public string Message { get; }
    public string Title { get; }
    public MessageSeverity Severity { get; }
    public bool ShowDialog { get; }

    public SystemMessage(string message, string title = "", MessageSeverity severity = MessageSeverity.Information, bool showDialog = false)
    {
        Message = message;
        Title = title;
        Severity = severity;
        ShowDialog = showDialog;
    }
}
