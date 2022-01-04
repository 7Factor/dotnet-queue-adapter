using _7Factor.QueueAdapter;
using _7Factor.QueueAdapter.Sqs;
using _7Factor.QueueAdapter.Sqs.Configuration;
using Amazon.SQS;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

internal class QueueFactory<TQueueId> where TQueueId : IQueueIdentifier
{
    private readonly IQueueConfiguration<TQueueId>? _queueConfiguration;

    internal QueueFactory(IQueueConfiguration<TQueueId>? queueConfiguration)
    {
        _queueConfiguration = queueConfiguration;
    }

    internal object CreateQueue(IServiceProvider provider)
    {
        var config = _queueConfiguration ?? provider.GetRequiredService<IQueueConfiguration<TQueueId>>();
        var sqsClient = provider.GetRequiredService<IAmazonSQS>();
        var logger = provider.GetRequiredService<ILogger<SqsMessageQueue<TQueueId>>>();
        return new SqsMessageQueue<TQueueId>(config, sqsClient, logger);
    }
}
