using Moq;

namespace _7Factor.QueueAdapter.Sqs.Test.Util;

public class MockedMethodCall<TArgs, TReturn>
{
    public TArgs? Args { get; set; }
    public TReturn? ReturnVal { get; set; }
    public Action<Times>? VerifyCalled { get; set; }
}
