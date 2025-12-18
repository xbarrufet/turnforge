using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record UnitDefinition(
    UnitTypeId TypeId,
    int InitialMaxHealth,
    IReadOnlyList<IActorBehaviour> Behaviours
) : AgentDefinition
{
    public new int MaxHealth { get; init; } = InitialMaxHealth;
}

public readonly record struct UnitTypeId(string Value);