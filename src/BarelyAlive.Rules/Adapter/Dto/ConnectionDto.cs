

namespace BarelyAlive.Rules.Adapter.Dto;

public sealed class ConnectionDto
{
    public PositionDto From { get; init; } = default!;
    public PositionDto To { get; init; } = default!;
    public string Direction { get; init; } = default!;
}

