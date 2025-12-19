using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record NpcDefinition(
    NpcTypeId TypeId,
    int MaxHealth,
    int MaxBaseMovement,
    int MaxActionPoints,
    IReadOnlyList<IActorBehaviour> Behaviours
) : AgentDefinition(MaxHealth, MaxBaseMovement, MaxActionPoints, Behaviours);

    public readonly record struct NpcTypeId(string Value);

