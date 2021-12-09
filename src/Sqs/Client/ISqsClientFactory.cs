using Amazon.SQS;

namespace Queue.Sqs.Client;

public interface ISqsClientFactory
{
    public IAmazonSQS CreateSqsClient();
}
