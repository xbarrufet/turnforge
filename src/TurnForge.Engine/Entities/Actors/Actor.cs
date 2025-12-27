using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Actors;

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
        AddComponent(IPositionComponent.Empty());
        AddComponent(IHealthComponent.Empty());
        
        // Keep direct references for convenience and performance
        PositionComponent = GetRequiredComponent<IPositionComponent>();
        HealthComponent = GetRequiredComponent<IHealthComponent>();
    }

    public new bool HasTrait<T>() where T : IActorTrait
    {
        return GetTraitComponent().HasTrait<T>();
    }

    public override void AddComponent<T>(T component)
    {
        base.AddComponent(component);
        if (component is IPositionComponent pc) PositionComponent = pc;
        if (component is IHealthComponent hc) HealthComponent = hc;
    }

    public override void ReplaceComponent<T>(T component)
    {
        base.ReplaceComponent(component);
        if (component is IPositionComponent pc) PositionComponent = pc;
        if (component is IHealthComponent hc) HealthComponent = hc;
    }

    public void SetPositionComponent(IPositionComponent positionComponent)
    {
        ReplaceComponent(positionComponent);
    }

    public void SetHealthComponent(IHealthComponent healthComponent)
    {
        ReplaceComponent(healthComponent);
    }

 
}