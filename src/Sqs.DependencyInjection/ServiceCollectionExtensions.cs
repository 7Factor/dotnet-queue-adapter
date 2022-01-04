using _7Factor.QueueAdapter;
using _7Factor.QueueAdapter.Message;
using _7Factor.QueueAdapter.Sqs.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// This class adds extension methods to IServiceCollection making it easier to add SQS queues to the NET Core
/// dependency injection framework.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the QueueConfiguration object to the dependency injection framework providing information that will be used
    /// to construct SQS queues for the given queue identifier.
    /// </summary>
    /// <typeparam name="TQueueId">The identifier for the queue configuration to inject</typeparam>
    /// <param name="collection">The IServiceCollection to add the config to.</param>
    /// <param name="queueConfiguration">The default configuration used to construct SQS queues for the given queue
    /// identifier.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddSqsQueueConfiguration<TQueueId>(this IServiceCollection collection,
        QueueConfiguration<TQueueId> queueConfiguration) where TQueueId : IQueueIdentifier
    {
        collection.Add(new ServiceDescriptor(typeof(IQueueConfiguration<TQueueId>), queueConfiguration));
        return collection;
    }

    /// <summary>
    /// Adds the MessageSchemaProvider object to the dependency injection framework providing information that will be
    /// used to construct SQS queues for the given queue identifier.
    /// </summary>
    /// <typeparam name="TQueueId">The identifier for the message schema provider to inject</typeparam>
    /// <param name="collection">The IServiceCollection to add the provider to.</param>
    /// <param name="messageSchemaProvider">The default message schema provider used to construct SQS queues for the
    /// given queue identifier.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddMessageSchemaProvider<TQueueId>(this IServiceCollection collection,
        MessageSchemaProvider<TQueueId> messageSchemaProvider) where TQueueId : IQueueIdentifier
    {
        collection.Add(new ServiceDescriptor(typeof(IMessageSchemaProvider<TQueueId>), messageSchemaProvider));
        return collection;
    }

    /// <summary>
    /// Adds the SQS queue to the dependency injection framework. The SQS queue is not created until it is requested.
    /// If the ServiceLifetime property is set to Singleton, the default, then the same instance will be reused for the
    /// lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TQueueId">The identifier for the queue to inject</typeparam>
    /// <param name="collection">The IServiceCollection to add the queue to.</param>
    /// <param name="lifetime">The lifetime of the queue created. The default is Singleton.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddSqsQueue<TQueueId>(this IServiceCollection collection,
        ServiceLifetime lifetime = ServiceLifetime.Singleton) where TQueueId : IQueueIdentifier
    {
        return AddSqsQueue(collection, null as QueueConfiguration<TQueueId>, null as MessageSchemaProvider<TQueueId>,
            lifetime);
    }

    /// <summary>
    /// Adds the SQS queue to the dependency injection framework. The SQS queue is not created until it is requested.
    /// If the ServiceLifetime property is set to Singleton, the default, then the same instance will be reused for the
    /// lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TQueueId">The identifier for the queue to inject</typeparam>
    /// <param name="collection">The IServiceCollection to add the queue to.</param>
    /// <param name="queueUrl">The URL for the SQS queue.</param>
    /// <param name="lifetime">The lifetime of the queue created. The default is Singleton.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddSqsQueue<TQueueId>(this IServiceCollection collection, string queueUrl,
        ServiceLifetime lifetime = ServiceLifetime.Singleton) where TQueueId : IQueueIdentifier
    {
        return AddSqsQueue(collection, new QueueConfiguration<TQueueId>(queueUrl), null, lifetime);
    }

    /// <summary>
    /// Adds the SQS queue to the dependency injection framework. The SQS queue is not created until it is requested.
    /// If the ServiceLifetime property is set to Singleton, the default, then the same instance will be reused for the
    /// lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TQueueId">The identifier for the queue to inject</typeparam>
    /// <param name="collection">The IServiceCollection to add the queue to.</param>
    /// <param name="messageSchemata">The <see cref="MessageSchema"/> to use with the queue.</param>
    /// <param name="lifetime">The lifetime of the queue created. The default is Singleton.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddSqsQueue<TQueueId>(this IServiceCollection collection,
        IEnumerable<MessageSchema> messageSchemata, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TQueueId : IQueueIdentifier
    {
        return AddSqsQueue(collection, null, new MessageSchemaProvider<TQueueId>(messageSchemata), lifetime);
    }

    /// <summary>
    /// Adds the SQS queue to the dependency injection framework. The SQS queue is not created until it is requested.
    /// If the ServiceLifetime property is set to Singleton, the default, then the same instance will be reused for the
    /// lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TQueueId">The identifier for the queue to inject</typeparam>
    /// <param name="collection">The IServiceCollection to add the queue to.</param>
    /// <param name="queueUrl">The URL for the SQS queue.</param>
    /// <param name="messageSchemata">The <see cref="MessageSchema"/> to use with the queue.</param>
    /// <param name="lifetime">The lifetime of the queue created. The default is Singleton.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddSqsQueue<TQueueId>(this IServiceCollection collection, string queueUrl,
        IEnumerable<MessageSchema> messageSchemata, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TQueueId : IQueueIdentifier
    {
        return AddSqsQueue(collection, new QueueConfiguration<TQueueId>(queueUrl),
            new MessageSchemaProvider<TQueueId>(messageSchemata), lifetime);
    }

    /// <summary>
    /// Adds the SQS queue to the dependency injection framework. The SQS queue is not created until it is requested.
    /// If the ServiceLifetime property is set to Singleton, the default, then the same instance will be reused for the
    /// lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TQueueId">The identifier for the queue to inject</typeparam>
    /// <param name="collection">The IServiceCollection to add the queue to.</param>
    /// <param name="queueConfiguration">The configuration used to create the queue. If null, a previously injected
    /// configuration will be used.</param>
    /// <param name="lifetime">The lifetime of the queue created. The default is Singleton.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddSqsQueue<TQueueId>(this IServiceCollection collection,
        QueueConfiguration<TQueueId>? queueConfiguration, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TQueueId : IQueueIdentifier
    {
        return AddSqsQueue(collection, queueConfiguration, null, lifetime);
    }

    /// <summary>
    /// Adds the SQS queue to the dependency injection framework. The SQS queue is not created until it is requested.
    /// If the ServiceLifetime property is set to Singleton, the default, then the same instance will be reused for the
    /// lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TQueueId">The identifier for the queue to inject</typeparam>
    /// <param name="collection">The IServiceCollection to add the queue to.</param>
    /// <param name="messageSchemaProvider">The message schema provider used to create the queue. If null, a previously
    /// injected provider will be used.</param>
    /// <param name="lifetime">The lifetime of the queue created. The default is Singleton.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddSqsQueue<TQueueId>(this IServiceCollection collection,
        MessageSchemaProvider<TQueueId>? messageSchemaProvider, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TQueueId : IQueueIdentifier
    {
        return AddSqsQueue(collection, null, messageSchemaProvider, lifetime);
    }

    /// <summary>
    /// Adds the SQS queue to the dependency injection framework. The SQS queue is not created until it is requested.
    /// If the ServiceLifetime property is set to Singleton, the default, then the same instance will be reused for the
    /// lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TQueueId">The identifier for the queue to inject</typeparam>
    /// <param name="collection">The IServiceCollection to add the queue to.</param>
    /// <param name="queueConfiguration">The configuration used to create the queue. If null, a previously injected
    /// configuration will be used.</param>
    /// <param name="messageSchemaProvider">The message schema provider used to create the queue. If null, a previously
    /// injected provider will be used.</param>
    /// <param name="lifetime">The lifetime of the queue created. The default is Singleton.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddSqsQueue<TQueueId>(this IServiceCollection collection,
        QueueConfiguration<TQueueId>? queueConfiguration, MessageSchemaProvider<TQueueId>? messageSchemaProvider,
        ServiceLifetime lifetime = ServiceLifetime.Singleton) where TQueueId : IQueueIdentifier
    {
        var factory = new QueueFactory<TQueueId>(queueConfiguration, messageSchemaProvider).CreateQueue;

        var descriptor = new ServiceDescriptor(typeof(IMessageQueue<TQueueId>), factory, lifetime);
        collection.Add(descriptor);
        return collection;
    }
}
