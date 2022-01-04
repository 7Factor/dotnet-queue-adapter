namespace _7Factor.QueueAdapter.Sqs.Configuration;

/// <summary>
/// Configuration for a given SQS queue.
/// </summary>
public interface IQueueConfiguration
{
    public string Url { get; }
}

/// <summary>
/// Configuration for a given SQS queue with a type parameter for selective dependency injection.
/// </summary>
/// <typeparam name="TQueueId">An identifier of the queue that this configuration is for.</typeparam>
// ReSharper disable once UnusedTypeParameter
public interface IQueueConfiguration<TQueueId> : IQueueConfiguration where TQueueId : IQueueIdentifier
{
}
