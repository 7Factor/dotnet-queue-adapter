namespace Queue.Message;

public readonly record struct SimpleMessage(MessageSchema MessageSchema, string Body) : IMessage;
