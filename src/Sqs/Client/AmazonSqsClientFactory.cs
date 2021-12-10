using _7Factor.QueueAdapter.Sqs.Configuration;
using Amazon;
using Amazon.SQS;

namespace _7Factor.QueueAdapter.Sqs.Client;

public class AmazonSqsClientFactory : ISqsClientFactory
{
    private readonly IAwsConfiguration _config;

    private readonly IAmazonSQS _client;
        
    public AmazonSqsClientFactory(IAwsConfiguration config)
    {
        _config = config;
        _client = new AmazonSQSClient(new AmazonSQSConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(_config.Region)
        });
    }
        
    public IAmazonSQS CreateSqsClient()
    {
        return _client;
    }
}
