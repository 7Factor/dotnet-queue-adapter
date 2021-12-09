using Amazon.SQS.Model;
using Queue.Message;

namespace Queue.Sqs;

internal static class SqsMessageRequestFactory
{
    public static SendMessageRequest CreateSendMessageRequest(string queueUrl, IMessage message)
    {
        return new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageAttributes =
            {
                {
                    SqsMessageAttribute.MessageType.Key,
                    new MessageAttributeValue
                    {
                        DataType = SqsMessageAttribute.MessageType.Type,
                        StringValue = message.MessageSchema.ToString()
                    }
                }
            },
            MessageBody = message.Body
        };
    }

    public static ReceiveMessageRequest CreateReceiveMessageRequest(string queueUrl)
    {
        return new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MessageAttributeNames =
            {
                SqsMessageAttribute.MessageType.Key
            }
        };
    }
}
