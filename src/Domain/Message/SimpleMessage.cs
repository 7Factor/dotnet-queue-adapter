namespace _7Factor.QueueAdapter.Message;

public readonly record struct SimpleMessage(MessageSchema MessageSchema, string Body) : IMessage;
