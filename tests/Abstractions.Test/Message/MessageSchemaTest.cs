using _7Factor.QueueAdapter.Message;
using Xunit;

namespace _7Factor.QueueAdapter.Test.Message;

public class MessageSchemaTest
{
    private static readonly MessageSchema SomeSchema = new("SomeSchema");

    [Fact]
    public void MessageType_StaticInitialization_DoesNotThrowException()
    {
        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        MessageSchema.Unknown.ToString();
    }

    [Fact]
    public void FromString_WithRandomString_ReturnsUnknownMessageType()
    {
        Assert.Equal(MessageSchema.Unknown, MessageSchema.FromString("asdfasdfav"));
    }

    [Fact]
    public void FromString_WithEmptyString_ReturnsUnknownMessageType()
    {
        Assert.Equal(MessageSchema.Unknown, MessageSchema.FromString(""));
    }

    [Fact]
    public void FromString_WithNull_ReturnsUnknownMessageType()
    {
        Assert.Equal(MessageSchema.Unknown, MessageSchema.FromString(null));
    }

    [Fact]
    public void FromString_WithKnownMessageTypeToString_ReturnsSameMessageType()
    {
        Assert.Equal(SomeSchema, MessageSchema.FromString(SomeSchema.ToString()));
    }

    [Fact]
    public void ToString_WithKnownMessageType_ReturnsKnownString()
    {
        Assert.Equal("SomeSchema", SomeSchema.ToString());
    }
}
