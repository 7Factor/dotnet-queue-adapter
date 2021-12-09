namespace Queue.Message;

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
