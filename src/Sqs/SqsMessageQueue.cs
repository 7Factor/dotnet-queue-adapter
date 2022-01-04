using _7Factor.QueueAdapter.Message;
using _7Factor.QueueAdapter.Sqs.Configuration;
using Amazon.SQS;

namespace _7Factor.QueueAdapter.Sqs;

/// <summary>
/// An implementation of <see cref="IMessageQueue"/> that uses SQS as a data source for the queue.
/// </summary>
public class SqsMessageQueue : IMessageQueue
{
    private readonly string _queueUrl;
    private readonly IAmazonSQS _sqsClient;

    public SqsMessageQueue(IQueueConfiguration queueConfig, IMessageSchemaProvider messageSchemaProvider,
        IAmazonSQS sqsClient)
    {
        _queueUrl = queueConfig.Url;
        MessageSchemaProvider = messageSchemaProvider;
        _sqsClient = sqsClient;
    }

    public IMessageSchemaProvider MessageSchemaProvider { get; }

    public async Task Push(IMessage message)
    {
        var messageRequest = SqsMessageRequestFactory.CreateSendMessageRequest(_queueUrl, message);
        await _sqsClient.SendMessageAsync(messageRequest);
    }

    public async Task<ProcessingResult?> Process(MessageProcessor messageProcessor)
    {
        var messageRequest = SqsMessageRequestFactory.CreateReceiveMessageRequest(_queueUrl);
        var response = await _sqsClient.ReceiveMessageAsync(messageRequest);
        var sqsMessage = response.Messages.Count > 0 ? response.Messages[0] : null;

        if (sqsMessage == null) return null;

        var message = CreateMessage(sqsMessage);
        var processingResult = await messageProcessor.Invoke(message);

        if (!processingResult.ShouldRetry)
        {
            await _sqsClient.DeleteMessageAsync(_queueUrl, sqsMessage.ReceiptHandle);
        }

        return processingResult;
    }

    private IMessage CreateMessage(Amazon.SQS.Model.Message message)
    {
        var messageTypeAttr = GetMessageAttributeValue(message, SqsMessageAttribute.MessageType);
        var messageSchema = MessageSchemaProvider.ParseMessageSchema(messageTypeAttr);

        var body = message.Body;

        return new SimpleMessage(messageSchema, body);
    }

    private static string? GetMessageAttributeValue(Amazon.SQS.Model.Message message, SqsMessageAttribute attribute)
    {
        return message.MessageAttributes[attribute.Key]?.StringValue;
    }
}

/// <summary>
/// An implementation of <see cref="IMessageQueue"/> that uses SQS as a data source for the queue. This form takes a
/// generic type so that Dependency-Injection can inject instances that represent different queue data sources.
/// </summary>
/// <typeparam name="TQueueId">An identifier of the data source that this queue represents.</typeparam>
// ReSharper disable once UnusedType.Global
public class SqsMessageQueue<TQueueId> : SqsMessageQueue, IMessageQueue<TQueueId> where TQueueId : IQueueIdentifier
{
    public SqsMessageQueue(IQueueConfiguration<TQueueId> queueConfig,
        IMessageSchemaProvider<TQueueId> messageSchemaProvider, IAmazonSQS sqsClient) : base(queueConfig,
        messageSchemaProvider, sqsClient)
    {
    }
}
