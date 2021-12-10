using Amazon.SQS;

namespace _7Factor.QueueAdapter.Sqs.Client;

public interface ISqsClientFactory
{
    public IAmazonSQS CreateSqsClient();
}
