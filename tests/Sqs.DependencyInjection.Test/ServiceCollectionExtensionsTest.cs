using _7Factor.QueueAdapter;
using _7Factor.QueueAdapter.Message;
using _7Factor.QueueAdapter.Sqs;
using _7Factor.QueueAdapter.Sqs.Configuration;
using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection.Test;

public class ServiceCollectionExtensionsTest
{
    private const string DefaultUrl = "https://the.url";
    private const string AlternateUrl = "https://the.other.url";
    private const string MessageSchemaAttrKey = "messageSchema";

    private static readonly MessageSchema DefaultSchema = new("DefaultSchema");
    private static readonly MessageSchema AlternateSchema = new("AlternateSchema");

    private readonly ServiceCollection _services = new();
    private readonly Mock<IAmazonSQS> _queueMock;

    public ServiceCollectionExtensionsTest()
    {
        _queueMock = new Mock<IAmazonSQS>();
        _services.AddSingleton(_queueMock.Object);
    }

    [Fact]
    public async void AddSqsQueue_WithDefaultConfig_UrlMatchesDefault()
    {
        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
        _services.AddMessageSchemaProvider(new MessageSchemaProvider<Queues.SomeQueue>(DefaultSchema));
        _services.AddSqsQueue<Queues.SomeQueue>();

        var serviceProvider = _services.BuildServiceProvider();
        var controller = ActivatorUtilities.CreateInstance<TestController>(serviceProvider);

        Assert.NotNull(controller.Queue);
        Assert.IsType<SqsMessageQueue<Queues.SomeQueue>>(controller.Queue);

        SendMessageRequest? messageRequest = null;
        _queueMock.Setup(q => q.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SendMessageRequest, CancellationToken>(
                (request, _) => { messageRequest = request; });
        await controller.Queue.Push(new SimpleMessage(MessageSchema.Unknown, "Hello"));

        Assert.NotNull(messageRequest);
        Assert.Equal(DefaultUrl, messageRequest?.QueueUrl);
    }

    [Fact]
    public async void AddSqsQueue_WithOverridingConfig_UrlMatchesAlternate()
    {
        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
        _services.AddMessageSchemaProvider(new MessageSchemaProvider<Queues.SomeQueue>(DefaultSchema));
        _services.AddSqsQueue(new QueueConfiguration<Queues.SomeQueue>(AlternateUrl));

        var serviceProvider = _services.BuildServiceProvider();
        var controller = ActivatorUtilities.CreateInstance<TestController>(serviceProvider);

        Assert.NotNull(controller.Queue);
        Assert.IsType<SqsMessageQueue<Queues.SomeQueue>>(controller.Queue);

        SendMessageRequest? messageRequest = null;
        _queueMock.Setup(q => q.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SendMessageRequest, CancellationToken>(
                (request, _) => { messageRequest = request; });
        await controller.Queue.Push(new SimpleMessage(MessageSchema.Unknown, "Hello"));

        Assert.NotNull(messageRequest);
        Assert.Equal(AlternateUrl, messageRequest?.QueueUrl);
    }

    [Fact]
    public async void AddSqsQueue_WithStringUrl_UrlMatchesAlternate()
    {
        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
        _services.AddMessageSchemaProvider(new MessageSchemaProvider<Queues.SomeQueue>(DefaultSchema));
        _services.AddSqsQueue<Queues.SomeQueue>(AlternateUrl);

        var serviceProvider = _services.BuildServiceProvider();
        var controller = ActivatorUtilities.CreateInstance<TestController>(serviceProvider);

        Assert.NotNull(controller.Queue);
        Assert.IsType<SqsMessageQueue<Queues.SomeQueue>>(controller.Queue);

        SendMessageRequest? messageRequest = null;
        _queueMock.Setup(q => q.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SendMessageRequest, CancellationToken>(
                (request, _) => { messageRequest = request; });
        await controller.Queue.Push(new SimpleMessage(MessageSchema.Unknown, "Hello"));

        Assert.NotNull(messageRequest);
        Assert.Equal(AlternateUrl, messageRequest?.QueueUrl);
    }

    [Fact]
    public async void AddSqsQueue_WithConfig_UrlMatchesAlternate()
    {
        _services.AddMessageSchemaProvider(new MessageSchemaProvider<Queues.SomeQueue>(DefaultSchema));
        _services.AddSqsQueue(new QueueConfiguration<Queues.SomeQueue>(AlternateUrl));

        var serviceProvider = _services.BuildServiceProvider();
        var controller = ActivatorUtilities.CreateInstance<TestController>(serviceProvider);

        Assert.NotNull(controller.Queue);
        Assert.IsType<SqsMessageQueue<Queues.SomeQueue>>(controller.Queue);

        SendMessageRequest? messageRequest = null;
        _queueMock.Setup(q => q.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SendMessageRequest, CancellationToken>(
                (request, _) => { messageRequest = request; });
        await controller.Queue.Push(new SimpleMessage(MessageSchema.Unknown, "Hello"));

        Assert.NotNull(messageRequest);
        Assert.Equal(AlternateUrl, messageRequest?.QueueUrl);
    }

    [Fact]
    public async void AddSqsQueue_WithDefaultSchemaProvider_MessageSchemaMatchesDefault()
    {
        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
        _services.AddMessageSchemaProvider(new MessageSchemaProvider<Queues.SomeQueue>(DefaultSchema));
        _services.AddSqsQueue<Queues.SomeQueue>();

        var serviceProvider = _services.BuildServiceProvider();
        var controller = ActivatorUtilities.CreateInstance<TestController>(serviceProvider);

        Assert.NotNull(controller.Queue);
        Assert.IsType<SqsMessageQueue<Queues.SomeQueue>>(controller.Queue);

        _queueMock.Setup(q => q.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse
            {
                Messages =
                {
                    new Message()
                    {
                        Body = "Hello",
                        MessageAttributes =
                            { { MessageSchemaAttrKey, new MessageAttributeValue { StringValue = DefaultSchema.Name } } }
                    }
                }
            });

        var result = await controller.Queue.Process(ProcessingResult.SuccessAsync);
        Assert.NotNull(result);
        Assert.Equal(DefaultSchema, result?.Message.MessageSchema);
    }

    [Fact]
    public async void AddSqsQueue_WithOverridingSchemaProvider_MessageSchemaMatchesAlternate()
    {
        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
        _services.AddMessageSchemaProvider(new MessageSchemaProvider<Queues.SomeQueue>(DefaultSchema));
        _services.AddSqsQueue(new MessageSchemaProvider<Queues.SomeQueue>(AlternateSchema));

        var serviceProvider = _services.BuildServiceProvider();
        var controller = ActivatorUtilities.CreateInstance<TestController>(serviceProvider);

        Assert.NotNull(controller.Queue);
        Assert.IsType<SqsMessageQueue<Queues.SomeQueue>>(controller.Queue);

        _queueMock.Setup(q => q.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse
            {
                Messages =
                {
                    new Message()
                    {
                        Body = "Hello",
                        MessageAttributes =
                        {
                            { MessageSchemaAttrKey, new MessageAttributeValue { StringValue = AlternateSchema.Name } }
                        }
                    }
                }
            });

        var result = await controller.Queue.Process(ProcessingResult.SuccessAsync);
        Assert.NotNull(result);
        Assert.Equal(AlternateSchema, result?.Message.MessageSchema);
    }

    [Fact]
    public async void AddSqsQueue_WithSchemaList_MessageSchemaMatchesAlternate()
    {
        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
        _services.AddMessageSchemaProvider(new MessageSchemaProvider<Queues.SomeQueue>(DefaultSchema));
        _services.AddSqsQueue<Queues.SomeQueue>(new List<MessageSchema> { AlternateSchema });

        var serviceProvider = _services.BuildServiceProvider();
        var controller = ActivatorUtilities.CreateInstance<TestController>(serviceProvider);

        Assert.NotNull(controller.Queue);
        Assert.IsType<SqsMessageQueue<Queues.SomeQueue>>(controller.Queue);

        _queueMock.Setup(q => q.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse
            {
                Messages =
                {
                    new Message()
                    {
                        Body = "Hello",
                        MessageAttributes =
                        {
                            { MessageSchemaAttrKey, new MessageAttributeValue { StringValue = AlternateSchema.Name } }
                        }
                    }
                }
            });

        var result = await controller.Queue.Process(ProcessingResult.SuccessAsync);
        Assert.NotNull(result);
        Assert.Equal(AlternateSchema, result?.Message.MessageSchema);
    }

    [Fact]
    public async void AddSqsQueue_WithSchemaProvider_MessageSchemaMatchesAlternate()
    {
        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
        _services.AddSqsQueue(new MessageSchemaProvider<Queues.SomeQueue>(AlternateSchema));

        var serviceProvider = _services.BuildServiceProvider();
        var controller = ActivatorUtilities.CreateInstance<TestController>(serviceProvider);

        Assert.NotNull(controller.Queue);
        Assert.IsType<SqsMessageQueue<Queues.SomeQueue>>(controller.Queue);

        _queueMock.Setup(q => q.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse
            {
                Messages =
                {
                    new Message()
                    {
                        Body = "Hello",
                        MessageAttributes =
                        {
                            { MessageSchemaAttrKey, new MessageAttributeValue { StringValue = AlternateSchema.Name } }
                        }
                    }
                }
            });

        var result = await controller.Queue.Process(ProcessingResult.SuccessAsync);
        Assert.NotNull(result);
        Assert.Equal(AlternateSchema, result?.Message.MessageSchema);
    }

    [Fact]
    public async void AddSqsQueue_WithConfigSchemaProvider_UrlsMatchesAlternateAndMessageSchemaMatchesAlternate()
    {
        _services.AddSqsQueue(new QueueConfiguration<Queues.SomeQueue>(AlternateUrl),
            new MessageSchemaProvider<Queues.SomeQueue>(AlternateSchema));

        var serviceProvider = _services.BuildServiceProvider();
        var controller = ActivatorUtilities.CreateInstance<TestController>(serviceProvider);

        Assert.NotNull(controller.Queue);
        Assert.IsType<SqsMessageQueue<Queues.SomeQueue>>(controller.Queue);

        SendMessageRequest? messageRequest = null;
        _queueMock.Setup(q => q.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SendMessageRequest, CancellationToken>(
                (request, _) => { messageRequest = request; });
        await controller.Queue.Push(new SimpleMessage(MessageSchema.Unknown, "Hello"));

        Assert.NotNull(messageRequest);
        Assert.Equal(AlternateUrl, messageRequest?.QueueUrl);

        _queueMock.Setup(q => q.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse
            {
                Messages =
                {
                    new Message()
                    {
                        Body = "Hello",
                        MessageAttributes =
                        {
                            { MessageSchemaAttrKey, new MessageAttributeValue { StringValue = AlternateSchema.Name } }
                        }
                    }
                }
            });

        var result = await controller.Queue.Process(ProcessingResult.SuccessAsync);
        Assert.NotNull(result);
        Assert.Equal(AlternateSchema, result?.Message.MessageSchema);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static class Queues
    {
        // ReSharper disable once ClassNeverInstantiated.Global
        public class SomeQueue : IQueueIdentifier
        {
        }
    }

    private record TestController(IMessageQueue<Queues.SomeQueue> Queue);
}
