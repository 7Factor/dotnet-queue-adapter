# DotnetQueueAdapter

A simple library for queues backed by infrastructure.

## Usage

Import the NuGet package `7Factor.QueueAdapter.Abstractions` for just the queue and message interfaces. You can DI different
queues by using the generic form of `IMessageQueue`. Just implement the `IQueueIdentifier` and use that as the generic type.

### SQS

An implementation of the abstractions for queues backed by AWS SQS can be found in the `7Factor.QueueAdapter.Sqs` NuGet package.
To instantiate an `SqsMessageQueue` you will need the URL of your SQS queue, an SQS client, and a logger. Configuration 
interfaces are provided to enable DI on configurable values.

Additionally, the `7Factor.QueueAdapter.Sqs.DependencyInjection` package provides some extension methods for `IServiceCollection`
to make injecting queues even easier.


### Example Usage

```c#
public static class Queues
{
    public class SomeQueue : IQueueIdentifier {}
}

public class QueueWorker
{
    private readonly IMessageQueue<Queues.SomeQueue> _queue;
    
    public QueueWorker(IMessageQueue<Queues.SomeQueue> queue)
    {
        _queue = queue;
    }
    
    // Call this when you want to process the next queue message
    public async Task<ProcessingResult?> ProcessNextMessage()
    {
        return await _queue.Process(async message => {
            // Do something with the message
        });
    }
}

// DI setup
serviceCollection.AddAWSService<IAmazonSQS>(new AWSOptions { Region = RegionEndpoint.GetBySystemName("us-east-1") });
serviceCollection.AddSqsQueue<Queues.SomeQueue>("https://the.url");
```
