using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Components;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class Actor : GameEntity, IActor
{
    // Direct properties (structural data)
    public IBoardPositionId CurrentPosition { get; set; } = IBoardPositionId.Limbo;

    // Component references for optional behaviors
    public HealthComponent HealthComponent { get; protected set; }

    /// <summary>
    /// Convenience getter for current health value.
    /// </summary>
    public int CurrentHealth => HealthComponent?.CurrentHealth ?? 0;

    protected Actor(
        EntityId id,
        string definitionId,
        string name,
        Category category
        ) : base(id, name, category, definitionId)
    {
        _initializeComponents();
    }

    // Constructor for Builder (with startPosition)
    protected Actor(
        EntityId id,
        string definitionId,
        string name,
        Category category,
        IBoardPositionId startPosition
        ) : base(id, name, category, definitionId)
    {
        CurrentPosition = startPosition;
        _initializeComponents();
    }

    private void _initializeComponents()
    {
        // Register empty components in GameEntity's component dictionary
        AddComponent(HealthComponent.Empty);

        // Keep direct references for convenience and performance
        HealthComponent = GetRequiredComponent<HealthComponent>();
    }

    public override bool RemoveComponent<T>()
    {
        if (typeof(T) == typeof(HealthComponent))
        {
            throw new InvalidOperationException("Cannot remove health components from the actor.");
        }
        return base.RemoveComponent<T>();
    }

    public void SetHealthComponent(HealthComponent healthComponent)
    {
        ReplaceComponent(healthComponent);
    }


    public Actor CloneWithNewPosition(IBoardPositionId position)
    {
        var clone = (Actor)this.Clone();
        clone.CurrentPosition = position;
        return clone;
    }
}