using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Components.Definitions;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public abstract record ActorDefinition( 
    PositionComponentDefinition PositionComponentDefinition,
    IReadOnlyList<IActorBehaviour>? Behaviours)
    
{
    public IReadOnlyList<IActorBehaviour> Behaviours { get; init; } = Behaviours??
        [];
}