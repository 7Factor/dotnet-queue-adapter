namespace _7Factor.QueueAdapter.Message;

/// <summary>
/// A message from an <see cref="IMessageQueue"/>. Messages have a simple <see cref="MessageSchema"/> metadata to
/// help identify what form the data in the <see cref="Body"/> is expected to be in.
/// </summary>
public interface IMessage
{
    /// <summary>
    /// The type of message represented in the <see cref="Body"/>.
    /// </summary>
    public MessageSchema MessageSchema { get; }

    /// <summary>
    /// The body of the message which should typically be in JSON format.
    /// </summary>
    public string Body { get; }
}
