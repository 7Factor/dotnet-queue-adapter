using _7Factor.QueueAdapter;
using _7Factor.QueueAdapter.Message;
using _7Factor.QueueAdapter.Sqs;
using _7Factor.QueueAdapter.Sqs.Configuration;
using Amazon.SQS;

namespace Microsoft.Extensions.DependencyInjection;

internal class QueueFactory<TQueueId> where TQueueId : IQueueIdentifier
{
    private readonly IQueueConfiguration<TQueueId>? _queueConfiguration;
    private readonly IMessageSchemaProvider<TQueueId>? _messageSchemaProvider;

    internal QueueFactory(IQueueConfiguration<TQueueId>? queueConfiguration,
        IMessageSchemaProvider<TQueueId>? messageSchemaProvider)
    {
        _queueConfiguration = queueConfiguration;
        _messageSchemaProvider = messageSchemaProvider;
    }

    internal object CreateQueue(IServiceProvider provider)
    {
        var queueConfiguration = _queueConfiguration ?? provider.GetRequiredService<IQueueConfiguration<TQueueId>>();
        var schemaProvider = _messageSchemaProvider ?? provider.GetRequiredService<IMessageSchemaProvider<TQueueId>>();
        var sqsClient = provider.GetRequiredService<IAmazonSQS>();
        return new SqsMessageQueue<TQueueId>(queueConfiguration, schemaProvider, sqsClient);
    }
}
