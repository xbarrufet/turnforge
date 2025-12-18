using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record NpcDefinition(
    NpcTypeId TypeId,
    int MaxHealth,
    int BaseMovement,
    IReadOnlyList<IActorBehaviour> Behaviours
) : AgentDefinition;

    public readonly record struct NpcTypeId(string Value);
