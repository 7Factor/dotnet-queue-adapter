namespace Queue.Sqs.Configuration;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedTypeParameter
public interface IQueueConfiguration<TQueueId> where TQueueId : IQueueIdentifier
{
    public string Url { get; }
}
