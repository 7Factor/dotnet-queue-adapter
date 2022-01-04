namespace _7Factor.QueueAdapter.Message;

/// <summary>
/// A provider for <see cref="MessageSchema"/> known to a queue.
/// <seealso cref="IMessageQueue.MessageSchemaProvider"/>
/// </summary>
public interface IMessageSchemaProvider
{
    /// <summary>
    /// Retrieves a MessageSchema that matches the given string. If a MessageSchema does not exist for the given
    /// string, <see cref="MessageSchema.Unknown"/> will be returned.
    /// </summary>
    /// <param name="schemaName">The name of the message schema to find. A null value will always cause this method
    /// to return <see cref="MessageSchema.Unknown"/></param>
    /// <returns>A MessageSchema matching the given schema name string or <see cref="MessageSchema.Unknown"/>.</returns>
    public MessageSchema ParseMessageSchema(string? schemaName);
}

// ReSharper disable once UnusedTypeParameter
public interface IMessageSchemaProvider<TQueueId> : IMessageSchemaProvider where TQueueId : IQueueIdentifier
{
}
