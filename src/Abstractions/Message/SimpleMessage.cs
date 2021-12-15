namespace _7Factor.QueueAdapter.Message;

/// <summary>
/// A simple implementation of <see cref="IMessage"/>.
/// </summary>
/// <param name="MessageSchema">The schema of the message body.</param>
/// <param name="Body">The body of the message.</param>
public record SimpleMessage(MessageSchema MessageSchema, string Body) : IMessage;
