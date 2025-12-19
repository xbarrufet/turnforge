using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record PropDefinition(
    PropTypeId TypeId,
    int MaxBaseMovement,
    int MaxActionPoints,
    IReadOnlyList<IActorBehaviour> Behaviours,
    int MaxHealth)
 : AgentDefinition(MaxHealth, MaxBaseMovement, MaxActionPoints, Behaviours);



public readonly record struct PropTypeId(string Value);
