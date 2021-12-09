using Microsoft.Extensions.Logging;
using Queue.Message;
using Queue.Sqs.Client;
using Queue.Sqs.Configuration;

namespace Queue.Sqs;

public class SqsMessageQueue : IMessageQueue
{
    private readonly string _queueUrl;
    private readonly ISqsClientFactory _clientFactory;
    private readonly ILogger<SqsMessageQueue> _logger;

    public SqsMessageQueue(string queueUrl, ISqsClientFactory clientFactory, ILogger<SqsMessageQueue> logger)
    {
        _clientFactory = clientFactory;
        _queueUrl = queueUrl;
        _logger = logger;
    }

    public async Task Push(IMessage message)
    {
        var client = _clientFactory.CreateSqsClient();
        var messageRequest = SqsMessageRequestFactory.CreateSendMessageRequest(_queueUrl, message);
        await client.SendMessageAsync(messageRequest);
    }

    public async Task<ProcessingResult?> Process(MessageProcessor messageProcessor)
    {
        var client = _clientFactory.CreateSqsClient();

        var messageRequest = SqsMessageRequestFactory.CreateReceiveMessageRequest(_queueUrl);
        var response = await client.ReceiveMessageAsync(messageRequest);
        var sqsMessage = response.Messages.Count > 0 ? response.Messages[0] : null;

        if (sqsMessage == null) return null;

        var message = CreateMessage(sqsMessage);
        var processingResult = await messageProcessor.Invoke(message);

        if (!processingResult.ShouldRetry)
        {
            await client.DeleteMessageAsync(_queueUrl, sqsMessage.ReceiptHandle);
        }

        return processingResult;
    }

    private IMessage CreateMessage(Amazon.SQS.Model.Message message)
    {
        var messageTypeAttr = GetMessageAttributeValue(message, SqsMessageAttribute.MessageType);
        var messageSchema = MessageSchema.FromString(messageTypeAttr);

        if (messageSchema == MessageSchema.Unknown)
        {
            _logger.LogWarning("Unknown message type \"{MessageType}\"", messageTypeAttr);
        }

        var body = message.Body;

        return new SimpleMessage(messageSchema, body);
    }

    private static string? GetMessageAttributeValue(Amazon.SQS.Model.Message message, SqsMessageAttribute attribute)
    {
        return message.MessageAttributes[attribute.Key]?.StringValue;
    }
}

public class SqsMessageQueue<TQueueId> : SqsMessageQueue, IMessageQueue<TQueueId> where TQueueId : IQueueIdentifier
{
    public SqsMessageQueue(IQueueConfiguration<TQueueId> queueConfig, ISqsClientFactory clientFactory,
        ILogger<SqsMessageQueue<TQueueId>> logger) : base(queueConfig.Url, clientFactory, logger) {}
}
