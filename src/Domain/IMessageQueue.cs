using Queue.Message;

namespace Queue;

public interface IMessageQueue
{
    /// <summary>
    /// Pushes a message onto the end of the queue.
    /// </summary>
    /// <param name="message">The message to push onto the queue.</param>
    public Task Push(IMessage message);

    /// <summary>
    /// Processes the next message in the queue, if there are any messages to process.
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

// ReSharper disable once UnusedTypeParameter
public interface IMessageQueue<TQueueId> : IMessageQueue where TQueueId : IQueueIdentifier {}
