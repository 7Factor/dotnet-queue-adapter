using _7Factor.QueueAdapter.Message;
using _7Factor.QueueAdapter.Sqs.Client;
using _7Factor.QueueAdapter.Sqs.Test.Util;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace _7Factor.QueueAdapter.Sqs.Test;

public class SqsMessageQueueTest
{
    #region Test Data

    private const string SqsUrl = "https://www.example.com";
    private const string MessageSchemaKey = "messageSchema";
    private const string MessageSchemaDataType = "string";
    private const string MessageBody = "Hello";

    private static readonly MessageSchema SampleSchema = new MessageSchema("SampleSchema");

    private static readonly Amazon.SQS.Model.Message SampleSqsMessage = new()
    {
        Body = MessageBody,
        MessageAttributes =
        {
            {
                MessageSchemaKey, new MessageAttributeValue()
                {
                    DataType = MessageSchemaDataType,
                    StringValue = SampleSchema.ToString()
                }
            }
        }
    };

    private static readonly IMessage ExpectedQueueMessage = new SimpleMessage(SampleSchema, MessageBody);

    #endregion

    #region Test Initialization

    private readonly Mock<IAmazonSQS> _sqsMock;
    private readonly SqsMessageQueue _queue;

    public SqsMessageQueueTest()
    {
        _sqsMock = new Mock<IAmazonSQS>();

        var sqsClientFactoryMock = new Mock<ISqsClientFactory>();
        sqsClientFactoryMock.Setup(x => x.CreateSqsClient()).Returns(_sqsMock.Object);

        _queue = new SqsMessageQueue(SqsUrl, sqsClientFactoryMock.Object, Mock.Of<ILogger<SqsMessageQueue>>());
    }

    #endregion

    #region Tests

    [Fact]
    public async void Push_ForAnyPushCall_RequestContainsQueueUrl()
    {
        var call = PrepareSendMessageCall();
        await _queue.Push(new SimpleMessage(MessageSchema.Unknown, ""));

        call.VerifyCalled!(Times.Once());
        Assert.Equal(SqsUrl, call.Args?.Request.QueueUrl);
    }

    [Fact]
    public async void Push_WithSampleMessageSchema_RequestContainsSchemaMessageAttribute()
    {
        var call = PrepareSendMessageCall();
        await _queue.Push(new SimpleMessage(SampleSchema, "hello"));

        call.VerifyCalled!(Times.Once());

        var attribs = call.Args?.Request.MessageAttributes;
        Assert.True(attribs?.ContainsKey(MessageSchemaKey));
        Assert.Equal(MessageSchemaDataType, attribs?[MessageSchemaKey].DataType);
        Assert.Equal(SampleSchema.ToString(), attribs?[MessageSchemaKey].StringValue);
    }

    [Fact]
    public async void Process_ForAnyProcessCall_RequestContainsQueueUrl()
    {
        var call = PrepareReceiveMessageCallOnEmptyQueue();
        await _queue.Process(ProcessingResult.SuccessAsync);

        call.VerifyCalled!(Times.Once());
        Assert.Equal(SqsUrl, call.Args?.Request.QueueUrl);
    }

    [Fact]
    public async void Process_QueueHasSampleMessage_MessageProcessorSeesSampleMessage()
    {
        PrepareReceiveMessageCall(new List<Amazon.SQS.Model.Message> { SampleSqsMessage });
        IMessage? actualMessage = null;

        await _queue.Process(m =>
        {
            actualMessage = m;
            return ProcessingResult.SuccessAsync(m);
        });

        Assert.Equal(ExpectedQueueMessage, actualMessage);
    }

    [Fact]
    public async void Process_QueueHasNoMessages_ProcessReturnsNull()
    {
        PrepareReceiveMessageCallOnEmptyQueue();

        var result = await _queue.Process(m =>
        {
            return ProcessingResult.SuccessAsync(m);
        });

        Assert.Null(result);
    }

    [Fact]
    public async void Process_MessageProcessorSucceeds_ProcessReturnsMessageNoRetry()
    {
        PrepareReceiveMessageCall(new List<Amazon.SQS.Model.Message> { SampleSqsMessage });

        var result = await _queue.Process(ProcessingResult.SuccessAsync);

        Assert.Equal(new ProcessingResult(ExpectedQueueMessage, false), result);
    }

    [Fact]
    public async void Process_MessageProcessorFails_ProcessReturnsMessageNoRetry()
    {
        PrepareReceiveMessageCall(new List<Amazon.SQS.Model.Message> { SampleSqsMessage });

        var result = await _queue.Process(m => ProcessingResult.FailureAsync(m));

        Assert.Equal(new ProcessingResult(ExpectedQueueMessage, false), result);
    }

    [Fact]
    public async void Process_MessageProcessorFailsRequestingRetry_ProcessReturnsMessageWithRetry()
    {
        PrepareReceiveMessageCall(new List<Amazon.SQS.Model.Message> { SampleSqsMessage });

        var result = await _queue.Process(m => ProcessingResult.FailureAsync(m, true));

        Assert.Equal(new ProcessingResult(ExpectedQueueMessage, true), result);
    }

    #endregion

    #region Helpers

    private record SendMessageArgs(SendMessageRequest Request);

    private MockedMethodCall<SendMessageArgs, SendMessageResponse> PrepareSendMessageCall(string? returnedMessageId = null)
    {
        var data = new MockedMethodCall<SendMessageArgs, SendMessageResponse>
        {
            ReturnVal = new SendMessageResponse { MessageId = returnedMessageId }
        };

        _sqsMock
            .Setup(q => q.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<SendMessageRequest, CancellationToken>((request, _) =>
            {
                data.Args = new SendMessageArgs(request);
            })
            .ReturnsAsync(data.ReturnVal);

        data.VerifyCalled = times =>
        {
            _sqsMock.Verify(q => q.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()), times);
        };

        return data;
    }

    private record ReceiveMessageArgs(ReceiveMessageRequest Request);

    private MockedMethodCall<ReceiveMessageArgs, ReceiveMessageResponse> PrepareReceiveMessageCallOnEmptyQueue()
    {
        return PrepareReceiveMessageCall(new List<Amazon.SQS.Model.Message>());
    }

    private MockedMethodCall<ReceiveMessageArgs, ReceiveMessageResponse> PrepareReceiveMessageCall(
        List<Amazon.SQS.Model.Message> receivedMessages)
    {
        var data = new MockedMethodCall<ReceiveMessageArgs, ReceiveMessageResponse>
        {
            ReturnVal = new ReceiveMessageResponse { Messages = receivedMessages }
        };

        _sqsMock
            .Setup(q => q.ReceiveMessageAsync(
                It.IsAny<ReceiveMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<ReceiveMessageRequest, CancellationToken>((request, _) =>
            {
                data.Args = new ReceiveMessageArgs(request);
            })
            .ReturnsAsync(data.ReturnVal);

        data.VerifyCalled = times =>
        {
            _sqsMock.Verify(q => q.ReceiveMessageAsync(
                It.IsAny<ReceiveMessageRequest>(),
                It.IsAny<CancellationToken>()), times);
        };

        return data;
    }

    #endregion
}
