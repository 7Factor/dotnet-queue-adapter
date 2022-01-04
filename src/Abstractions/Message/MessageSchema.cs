namespace _7Factor.QueueAdapter.Message;

/// <summary>
/// Used on <see cref="IMessage"/> to identify the form of the data in the message body. Custom MessageSchema should
/// be set up to be provided by a <see cref="IMessageSchemaProvider"/> which each <see cref="IMessageQueue"/> has.
/// </summary>
public sealed record MessageSchema(string Name)
{
    /// <summary>
    /// The MessageSchema object to represent an unknown schema string.
    /// <seealso cref="IMessageSchemaProvider.ParseMessageSchema"/>
    /// </summary>
    public static readonly MessageSchema Unknown = new("");

    public override string ToString()
    {
        return Name;
    }
}
