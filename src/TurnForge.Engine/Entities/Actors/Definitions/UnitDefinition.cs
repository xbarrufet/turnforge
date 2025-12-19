using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record UnitDefinition(
    UnitTypeId TypeId,
    int MaxHealth,
    int MaxBaseMovement,
    int MaxActionPoints,
    IReadOnlyList<IActorBehaviour>? Behaviours
) : AgentDefinition(MaxHealth, MaxBaseMovement, MaxActionPoints, Behaviours);


public readonly record struct UnitTypeId(string Value);