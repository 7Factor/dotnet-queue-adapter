using _7Factor.QueueAdapter.Message;

namespace _7Factor.QueueAdapter;

/// <summary>
/// Holds the result of processing a message with the messageProcessor passed into the Process method of
/// <see cref="IMessageQueue"/>.
/// </summary>
/// <param name="Message">The message that was processed.</param>
/// <param name="ShouldRetry">Whether the message should remain on the queue for retrying processing.</param>
public record ProcessingResult(IMessage Message, bool ShouldRetry)
{
    /// <summary>
    /// Indicate <see cref="IMessageQueue"/> processing succeeded for the given message.
    /// </summary>
    /// <param name="message">The message that was processed.</param>
    public static ProcessingResult Success(IMessage message)
    {
        return new ProcessingResult(message, false);
    }

    /// <summary>
    /// Identical to <see cref="Success"/> but with the return value wrapped in a task.
    /// </summary>
    public static Task<ProcessingResult> SuccessAsync(IMessage message)
    {
        return Task.FromResult(Success(message));
    }

    /// <summary>
    /// Indicate <see cref="IMessageQueue"/> processing failed in the message processor function. The unprocessed
    /// message will be removed from the queue unless retry is set to <c>true</c>.
    /// </summary>
    /// <param name="message">The message that could not be processed.</param>
    /// <param name="retry">If set to true, the message will be returned to the front of the queue so it can
    /// attempted to be reprocessed. Otherwise, the message will be removed from the queue.</param>
    /// <returns></returns>
    public static ProcessingResult Failure(IMessage message, bool retry = false)
    {
        return new ProcessingResult(message, retry);
    }

    /// <summary>
    /// Identical to <see cref="Failure"/> but with the return value wrapped in a task.
    /// </summary>
    public static Task<ProcessingResult> FailureAsync(IMessage message, bool retry = false)
    {
        return Task.FromResult(Failure(message, retry));
    }
}
