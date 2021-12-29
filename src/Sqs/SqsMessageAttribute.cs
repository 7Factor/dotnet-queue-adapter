namespace _7Factor.QueueAdapter.Sqs;

internal class SqsMessageAttribute
{
    public static readonly SqsMessageAttribute MessageType = new("messageSchema", "String");

    private SqsMessageAttribute(string key, string type)
    {
        Key = key;
        Type = type;
    }

    public string Key { get; }

    public string Type { get; }
}
