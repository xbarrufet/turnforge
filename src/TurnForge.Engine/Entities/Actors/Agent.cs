using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public sealed class Agent : Actor
{
    public AgentDefinition Definition { get; }

    public Agent(
        EntityId id,
        AgentDefinition definition,
        PositionComponent position,
        BehaviourComponent behaviourComponent) : base(id, position, behaviourComponent)
    {
        Definition = definition;
    }
}