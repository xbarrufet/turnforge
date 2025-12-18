using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public abstract record ActorDefinition
{
    public IReadOnlyList<IActorBehaviour> Behaviours { get; init; } =
        Array.Empty<IActorBehaviour>();
}