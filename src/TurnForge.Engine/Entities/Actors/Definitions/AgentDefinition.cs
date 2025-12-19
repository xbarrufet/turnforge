using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public abstract record AgentDefinition(
    int MaxHealth,
    int MaxBaseMovement,
    int MaxActionPoints,
    IReadOnlyList<IActorBehaviour>? Behaviour) : ActorDefinition(Behaviour);


