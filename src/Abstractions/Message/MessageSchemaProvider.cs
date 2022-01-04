namespace _7Factor.QueueAdapter.Message;

/// <summary>
/// A simple implementation of <see cref="IMessageSchemaProvider"/> which stores known schema in a <see cref="List"/>.
/// </summary>
public class MessageSchemaProvider : IMessageSchemaProvider
{
    private readonly List<MessageSchema> _knownMessageSchemata;

    public MessageSchemaProvider(params MessageSchema[] knownMessageSchemata) : this(
        knownMessageSchemata as IEnumerable<MessageSchema>)
    {
    }

    public MessageSchemaProvider(IEnumerable<MessageSchema> knownMessageSchemata)
    {
        _knownMessageSchemata = new List<MessageSchema>(knownMessageSchemata);
    }

    public MessageSchema ParseMessageSchema(string? messageSchema)
    {
        return _knownMessageSchemata.Find(s => s.Name == messageSchema) ?? MessageSchema.Unknown;
    }
}

// ReSharper disable once UnusedTypeParameter
// ReSharper disable once ClassNeverInstantiated.Global
public class MessageSchemaProvider<TQueueId> : MessageSchemaProvider, IMessageSchemaProvider<TQueueId>
    where TQueueId : IQueueIdentifier
{
    public MessageSchemaProvider(params MessageSchema[] knownMessageSchemata) : base(knownMessageSchemata)
    {
    }

    public MessageSchemaProvider(IEnumerable<MessageSchema> knownMessageSchemata) : base(knownMessageSchemata)
    {
    }
}
