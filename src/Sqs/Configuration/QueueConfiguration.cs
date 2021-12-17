namespace _7Factor.QueueAdapter.Sqs.Configuration;

public record QueueConfiguration<TQueueId>(string Url) : IQueueConfiguration<TQueueId> where TQueueId : IQueueIdentifier
{
    public string Url { get; set; } = string.Empty;
}
