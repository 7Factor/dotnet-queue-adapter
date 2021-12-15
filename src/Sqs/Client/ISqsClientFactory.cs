using Amazon.SQS;

namespace _7Factor.QueueAdapter.Sqs.Client;

/// <summary>
/// A factory that creates SQS clients.
/// </summary>
public interface ISqsClientFactory
{
    public IAmazonSQS CreateSqsClient();
}
