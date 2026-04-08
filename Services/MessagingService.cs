using CommunityToolkit.Mvvm.Messaging;

namespace WpfMvvmApp.Services;

/// <summary>
/// Implementation of IMessagingService using CommunityToolkit.Mvvm WeakReferenceMessenger.
/// Messages are wrapped in MessageWrapper to work with the toolkit's IRecipient pattern.
/// </summary>
public class MessagingService : IMessagingService
{
    private readonly WeakReferenceMessenger _messenger = WeakReferenceMessenger.Default;

    public void Send<TMessage>(TMessage message) where TMessage : class
    {
        _messenger.Send(new MessageWrapper<TMessage>(message));
    }

    public void Register<TMessage>(object recipient, Action<TMessage> handler) where TMessage : class
    {
        _messenger.Register<MessageWrapper<TMessage>>(recipient, (_, m) => handler(m.Content));
    }

    public void Unregister(object recipient)
    {
        _messenger.UnregisterAll(recipient);
    }

    public void Unregister<TMessage>(object recipient) where TMessage : class
    {
        _messenger.Unregister<MessageWrapper<TMessage>>(recipient);
    }
}

/// <summary>
/// Wraps any message type for use with WeakReferenceMessenger.
/// </summary>
internal sealed class MessageWrapper<T> where T : class
{
    public T Content { get; }

    public MessageWrapper(T content)
    {
        Content = content;
    }
}
