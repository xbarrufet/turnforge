namespace BarelyAlive.Rules.Adapter.Dto;

using System.Collections.Generic;

public sealed class PropDto
{
    public string TypeId { get; init; } = default!;
    public List<BehaviourDto> Behaviours { get; init; } = new();
    public int MaxHealth { get; init; }
    public int MaxBaseMovement { get; init; }
    public int MaxActionPoints { get; init; }
    public PositionDto? Position { get; init; }
}

