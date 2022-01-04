using _7Factor.QueueAdapter;
using _7Factor.QueueAdapter.Message;
using _7Factor.QueueAdapter.Sqs;
using _7Factor.QueueAdapter.Sqs.Configuration;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection.Test;

public class ServiceCollectionExtensionsTest
{
    private const string DefaultUrl = "https://the.url";

    private readonly ServiceCollection _services = new();
    private readonly Mock<IAmazonSQS> _queueMock;

    public ServiceCollectionExtensionsTest()
    {
        _queueMock = new Mock<IAmazonSQS>();
        _services.AddSingleton(_queueMock.Object);
        _services.AddSingleton(Mock.Of<ILogger<SqsMessageQueue<Queues.SomeQueue>>>());
    }

    [Fact]
    public async void AddSqsQueue_WithDefaultConfig()
    {
        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
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
    public async void AddSqsQueue_WithOverridingConfig()
    {
        var alternateUrl = "https://the.other.url";

        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
        _services.AddSqsQueue(new QueueConfiguration<Queues.SomeQueue>(alternateUrl));

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
        Assert.Equal(alternateUrl, messageRequest?.QueueUrl);
    }

    [Fact]
    public async void AddSqsQueue_WithStringUrl()
    {
        var alternateUrl = "https://the.other.url";

        _services.AddSqsQueueConfiguration(new QueueConfiguration<Queues.SomeQueue>(DefaultUrl));
        _services.AddSqsQueue<Queues.SomeQueue>(alternateUrl);

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
        Assert.Equal(alternateUrl, messageRequest?.QueueUrl);
    }

    [Fact]
    public async void AddSqsQueue_WithConfig()
    {
        var alternateUrl = "https://the.other.url";

        _services.AddSqsQueue(new QueueConfiguration<Queues.SomeQueue>(alternateUrl));

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
        Assert.Equal(alternateUrl, messageRequest?.QueueUrl);
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
