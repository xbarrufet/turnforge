using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Components.Definitions;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record PropDefinition(
    PropTypeId TypeId,
    PositionComponentDefinition PositionComponentDefinition,
    HealhtComponentDefinition HealhtComponentDefinition,
    IReadOnlyList<IActorBehaviour> Behaviours)
 : ActorDefinition(PositionComponentDefinition, Behaviours);




