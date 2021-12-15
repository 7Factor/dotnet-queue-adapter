namespace _7Factor.QueueAdapter.Sqs.Configuration;

/// <summary>
/// Configuration for a given SQS queue.
/// </summary>
/// <typeparam name="TQueueId">An identifier of the queue that this configuration is for.</typeparam>
// ReSharper disable once UnusedTypeParameter
public interface IQueueConfiguration<TQueueId> where TQueueId : IQueueIdentifier
{
    public string Url { get; }
}
