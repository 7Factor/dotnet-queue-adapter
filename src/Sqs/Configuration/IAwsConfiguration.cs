namespace _7Factor.QueueAdapter.Sqs.Configuration;

/// <summary>
/// Configuration data related to AWS.
/// </summary>
public interface IAwsConfiguration
{
    public string Region { get; }
}
