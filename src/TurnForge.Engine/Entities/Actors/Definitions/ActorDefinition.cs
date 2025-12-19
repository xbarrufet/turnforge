using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public abstract record ActorDefinition( IReadOnlyList<IActorBehaviour>? Behaviours)
{
    public IReadOnlyList<IActorBehaviour> Behaviours { get; init; } = Behaviours??
        [];
}