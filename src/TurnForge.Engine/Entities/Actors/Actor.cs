using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class Actor : GameEntity
{
    protected Actor(EntityId id, PositionComponent positionComponent, BehaviourComponent behaviorComponent) : base(id)
    {
        AddComponent(positionComponent);
        AddComponent(behaviorComponent);
    }
}