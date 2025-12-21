using BarelyAlive.Rules.Apis.Messaging;

namespace BarelyAlive.Rules.Apis.Messaging;

public sealed record GameResponse
{
    public Guid TransactionId { get; init; }
    public bool Success { get; init; }
    public string? Error { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; } = [];
    public GameUpdatePayload Payload { get; init; } = default!;
}