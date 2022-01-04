using _7Factor.QueueAdapter.Message;

namespace _7Factor.QueueAdapter;

/// <summary>
/// Represents a first-in, first-out collection of messages.
/// </summary>
public interface IMessageQueue
{
    /// <summary>
    /// The provider for <see cref="MessageSchema"/> known to this queue.
    /// </summary>
    public IMessageSchemaProvider MessageSchemaProvider { get; }

    /// <summary>
    /// Pushes a message onto the end of the queue.
    /// </summary>
    /// <param name="message">The message to push onto the queue.</param>
    public Task Push(IMessage message);

    /// <summary>
    /// Processes the next message at the front of the queue, if there are any messages to process.
    /// </summary>
    /// <param name="messageProcessor">A function that processes the next message in the queue.
    /// While a message is being processed by this function it is effectively no longer in the queue.
    /// If processing is successful, the function should return true so the message will be removed from the
    /// queue. If the processing is unsuccessful, returning false will reinsert the message back at the
    /// front of the queue. If processing takes too long, the message may also be returned to the front of
    /// the queue, regardless of the result of this function.</param>
    /// <returns>
    /// The result of calling the messageProcessor. If messageProcessor was not called (ex: no messages in the
    /// queue), then null will be returned.
    /// </returns>
    public Task<ProcessingResult?> Process(MessageProcessor messageProcessor);
}

/// <summary>
/// Represents a first-in, first-out collection of messages. This form takes a generic type so that Dependency-Injection
/// can inject instances that represent different queue data sources.
/// </summary>
/// <typeparam name="TQueueId">An identifier of the data source that this queue represents.</typeparam>
// ReSharper disable once UnusedTypeParameter
public interface IMessageQueue<TQueueId> : IMessageQueue where TQueueId : IQueueIdentifier {}
