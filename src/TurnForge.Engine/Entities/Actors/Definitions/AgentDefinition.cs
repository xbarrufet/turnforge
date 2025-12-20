using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Components.Definitions;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public record AgentDefinition(
    AgentTypeId TypeId,
    PositionComponentDefinition PositionComponentDefinition,
    HealhtComponentDefinition HealthComponentDefinition,
    MovementComponentDefinition MovementComponentDefinition,
    IReadOnlyList<IActorBehaviour>? Behaviour) : ActorDefinition(PositionComponentDefinition,Behaviour);


