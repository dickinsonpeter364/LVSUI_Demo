namespace WpfMvvmApp.Services;

/// <summary>
/// Abstraction over the messaging/pub-sub system.
/// Business logic libraries depend on this interface, not on CommunityToolkit directly.
/// </summary>
public interface IMessagingService
{
    /// <summary>
    /// Send a message to all registered recipients.
    /// </summary>
    void Send<TMessage>(TMessage message) where TMessage : class;

    /// <summary>
    /// Register a recipient to receive messages of a given type.
    /// Uses weak references — no manual unregister required for GC.
    /// </summary>
    void Register<TMessage>(object recipient, Action<TMessage> handler) where TMessage : class;

    /// <summary>
    /// Unregister a recipient from all message types.
    /// </summary>
    void Unregister(object recipient);

    /// <summary>
    /// Unregister a recipient from a specific message type.
    /// </summary>
    void Unregister<TMessage>(object recipient) where TMessage : class;
}
