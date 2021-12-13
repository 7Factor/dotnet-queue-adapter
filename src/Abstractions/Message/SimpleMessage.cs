namespace _7Factor.QueueAdapter.Message;

public record SimpleMessage(MessageSchema MessageSchema, string Body) : IMessage;
