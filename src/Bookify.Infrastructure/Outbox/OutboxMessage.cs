namespace Bookify.Infrastructure.Outbox;
public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occuredAtUtc, string type, string content)
    {
        Id = id;
        OccurredAtUtc = occuredAtUtc;
        Type = type;
        Content = content;
    }

    public Guid Id { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public string Type { get; init; }
    public string Content { get; init; }
    public DateTime? ProcessedOnUtc { get; init; }
    public string? Error { get; init; }
}
