using _7Factor.QueueAdapter.Sqs.Configuration;
using Amazon;
using Amazon.SQS;

namespace _7Factor.QueueAdapter.Sqs.Client;

/// <summary>
/// A basic implementation of <see cref="ISqsClientFactory"/> for getting SQS queues living in AWS.
/// </summary>
public class AmazonSqsClientFactory : ISqsClientFactory
{
    private readonly IAmazonSQS _client;
        
    public AmazonSqsClientFactory(IAwsConfiguration config)
    {
        _client = new AmazonSQSClient(new AmazonSQSConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(config.Region)
        });
    }
        
    public IAmazonSQS CreateSqsClient()
    {
        return _client;
    }
}
