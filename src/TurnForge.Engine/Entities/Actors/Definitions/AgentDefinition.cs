using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public record AgentDefinition(
    AgentTypeId TypeId,
    int MaxHealth,
    int MaxBaseMovement,
    int MaxActionPoints,
    IReadOnlyList<IActorBehaviour>? Behaviour) : ActorDefinition(Behaviour);


