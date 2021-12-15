namespace _7Factor.QueueAdapter.Sqs.Configuration;

public record struct AwsConfiguration(string Region) : IAwsConfiguration;
