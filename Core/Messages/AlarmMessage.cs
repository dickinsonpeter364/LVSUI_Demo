namespace WpfMvvmApp.Core.Messages;

/// <summary>
/// Published when an IO alarm channel changes state.
/// Replaces legacy IO_CHANGE_Handler delegate.
/// </summary>
public sealed class AlarmMessage
{
    public int Channel { get; }
    public bool IsActive { get; }

    public AlarmMessage(int channel, bool isActive)
    {
        Channel = channel;
        IsActive = isActive;
    }
}
