namespace BarelyAlive.Rules.Adapter.Dto;

public sealed class ZoneDto
{
    public string Id { get; init; } = default!;
    public ZoneBoundDto Bound { get; init; } = default!;
    public List<BehaviourDto> Behaviours { get; init; } = new();
}

