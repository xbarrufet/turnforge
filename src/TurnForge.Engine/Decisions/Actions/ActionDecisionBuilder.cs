using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Decisions.Actions;

/// <summary>
/// Fluent builder for constructing ActionDecisions with validation.
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// var decision = new ActionDecisionBuilder()
///     .ForEntity("agent_123")
///     .UpdateComponent(new BasePositionComponent(targetPos))
///     .UpdateComponent(apComponent.Spend(cost))
///     .Build();
/// </code>
/// </remarks>
public sealed class ActionDecisionBuilder
{
    private string? _entityId;
    private readonly Dictionary<Type, IGameEntityComponent> _components = new();
    private DecisionTiming _timing = DecisionTiming.Immediate;
    private string _originId = string.Empty;
    
    /// <summary>
    /// Set the entity ID to update.
    /// </summary>
    public ActionDecisionBuilder ForEntity(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException("EntityId cannot be empty", nameof(entityId));
        
        _entityId = entityId;
        return this;
    }
    
    /// <summary>
    /// Add or update a component.
    /// If component of same type already added, replaces it.
    /// </summary>
    public ActionDecisionBuilder UpdateComponent<T>(T component) where T : IGameEntityComponent
    {
        if (component == null)
            throw new ArgumentNullException(nameof(component));
        
        _components[typeof(T)] = component;
        return this;
    }
    
    /// <summary>
    /// Set when the decision should execute.
    /// </summary>
    public ActionDecisionBuilder WithTiming(DecisionTiming timing)
    {
        _timing = timing;
        return this;
    }
    
    /// <summary>
    /// Set origin ID (command that created this decision).
    /// </summary>
    public ActionDecisionBuilder WithOrigin(string originId)
    {
        _originId = originId ?? string.Empty;
        return this;
    }
    
    /// <summary>
    /// Build the ActionDecision.
    /// Validates that required fields are set.
    /// </summary>
    public ActionDecision Build()
    {
        if (string.IsNullOrWhiteSpace(_entityId))
            throw new InvalidOperationException("EntityId must be set via ForEntity()");
        
        if (_components.Count == 0)
            throw new InvalidOperationException("At least one component must be added via UpdateComponent()");
        
        return new ActionDecision(
            _entityId,
            _components,
            _timing,
            _originId
        );
    }
}
