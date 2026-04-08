namespace WpfMvvmApp.Core.Messages;

/// <summary>
/// Published when a hardware IO interrupt fires.
/// Replaces legacy IO_INTERRUPT_Handler delegate.
/// </summary>
public sealed class IOInterruptMessage
{
    public int Channel { get; }
    public bool IsHigh { get; }

    public IOInterruptMessage(int channel, bool isHigh)
    {
        Channel = channel;
        IsHigh = isHigh;
    }
}
