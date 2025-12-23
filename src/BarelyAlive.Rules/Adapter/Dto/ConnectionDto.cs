namespace BarelyAlive.Rules.Adapter.Dto;

public sealed class ConnectionDto
{
    public string Id { get; init; } = default!;
    public string From { get; init; } = default!;
    public string To { get; init; } = default!;
    public string Direction { get; init; } = default!;
}

