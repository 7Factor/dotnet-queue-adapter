namespace _7Factor.QueueAdapter.Sqs.Configuration;

public record AwsConfiguration : IAwsConfiguration
{
    public string Region { get; set; } = "us-east-1";
};
