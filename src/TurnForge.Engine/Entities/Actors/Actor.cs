using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class Actor : GameEntity, IActor
{
    public IPositionComponent PositionComponent { get; protected set; }
    public IHealthComponent HealthComponent { get; protected set; }

    protected Actor(
        EntityId id,
        string name,
        string category,
        string definitionId) : base(id, name, category, definitionId)
    {
        // Register empty components in GameEntity's component dictionary
        // This ensures EngineAutoMapper can find them when mapping properties
        AddComponent(IPositionComponent.Empty());
        AddComponent(IHealthComponent.Empty());
        
        // Keep direct references for convenience and performance
        PositionComponent = GetRequiredComponent<IPositionComponent>();
        HealthComponent = GetRequiredComponent<IHealthComponent>();
    }

    public bool HasBehaviour<T>() where T : IActorBehaviour
    {
        return GetBehaviourComponent().HasBehaviour<T>();
    }

    public void SetPositionComponent(IPositionComponent positionComponent)
    {
        AddComponent(positionComponent);
    }

    public void SetHealthComponent(IHealthComponent healthComponent)
    {
        AddComponent(healthComponent);
    }

 
}