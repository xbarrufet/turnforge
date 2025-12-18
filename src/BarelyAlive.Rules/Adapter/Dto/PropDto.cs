
namespace BarelyAlive.Rules.Adapter.Dto;

public sealed class PropDto
{
    public string TypeId { get; init; } = default!;
    public PositionDto Position { get; init; } = default!;
    public List<BehaviourDto> Behaviours { get; init; } = new();
}

