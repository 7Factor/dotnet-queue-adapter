namespace _7Factor.QueueAdapter.Sqs.Configuration;

public record struct QueueConfiguration<TQueueId>(string Url) : IQueueConfiguration<TQueueId>
    where TQueueId : IQueueIdentifier;
