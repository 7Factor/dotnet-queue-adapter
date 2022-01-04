namespace _7Factor.QueueAdapter.Sqs.Configuration;

public record QueueConfiguration(string Url) : IQueueConfiguration;

// ReSharper disable once ClassNeverInstantiated.Global
public record QueueConfiguration<TQueueId>(string Url) : QueueConfiguration(Url), IQueueConfiguration<TQueueId>
    where TQueueId : IQueueIdentifier;
