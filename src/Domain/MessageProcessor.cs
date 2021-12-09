using Queue.Message;

namespace Queue;

/// <summary>
/// A delegate function that processes a message. The function must return a <see cref="ProcessingResult"/> that
/// indicates the result of processing the given message.
/// </summary>
public delegate Task<ProcessingResult> MessageProcessor(IMessage message);
