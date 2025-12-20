
using System.Collections.Generic;

namespace BarelyAlive.Rules.Adapter.Dto;

public sealed class AgentDto
{
    public string TypeId { get; init; } = default!;
    public List<BehaviourDto> Behaviours { get; init; } = new();
    public int MaxHealth { get; init; }
    public int MaxBaseMovement { get; init; }
    public int MaxActionPoints { get; init; }
}
