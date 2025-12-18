using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record PropDefinition(
    PropTypeId TypeId,
    int? MaxHealth,
    IReadOnlyList<IActorBehaviour> Behaviours
) : ActorDefinition;



public readonly record struct PropTypeId(string Value); 