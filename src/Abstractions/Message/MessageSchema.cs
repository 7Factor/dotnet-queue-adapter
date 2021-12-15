namespace _7Factor.QueueAdapter.Message;

/// <summary>
/// Used on <see cref="IMessage"/> to identify the form of the data in the message body. Any custom MessageSchema should
/// be instantiated prior to reading messages from a queue.
/// </summary>
public sealed record MessageSchema
{
    private static readonly Dictionary<string, MessageSchema> MessageSchemas = new();

    public static readonly MessageSchema Unknown = new("");

    /// <summary>
    /// Retrieves a MessageSchema that matches the given string. If a MessageSchema does not exist for the given
    /// string, <see cref="Unknown"/> will be returned.
    /// </summary>
    /// <param name="name">The name of the message type to find. A null value will always cause this method
    /// to return <see cref="Unknown"/></param>
    /// <returns>A MessageSchema matching the given name string or <see cref="Unknown"/>.</returns>
    public static MessageSchema FromString(string? name)
    {
        return name == null ? Unknown : MessageSchemas.GetValueOrDefault(name, Unknown);
    }

    private string Name { get; }

    public MessageSchema(string name)
    {
        Name = name;
        MessageSchemas[name] = this;
    }

    public override string ToString()
    {
        return Name;
    }
}
