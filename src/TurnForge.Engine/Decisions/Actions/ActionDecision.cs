using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Decisions.Actions;

/// <summary>
/// Decision representing component updates to apply to a single entity.
/// </summary>
/// <remarks>
/// Produced by action strategies and consumed by ActionDecisionApplier.
/// Contains which entity to update and which components to add/replace.
/// 
/// Design: Uses dictionary for component updates to allow multiple components
/// of different types. Later updates to same component type replace earlier ones.
/// </remarks>
public sealed record ActionDecision : IDecision
{
    /// <summary>
    /// ID of entity to update (Agent or Prop).
    /// </summary>
    public string EntityId { get; init; }
    
    /// <summary>
    /// Components to add/update on the entity.
    /// Key = component type, Value = component instance.
    /// </summary>
    public IReadOnlyDictionary<Type, IGameEntityComponent> ComponentUpdates { get; init; }
    
    /// <summary>
    /// When this decision should be executed.
    /// </summary>
    public DecisionTiming Timing { get; init; }
    
    /// <summary>
    /// ID of command/source that originated this decision.
    /// </summary>
    public string OriginId { get; init; }
    
    public ActionDecision(
        string entityId,
        IReadOnlyDictionary<Type, IGameEntityComponent> componentUpdates,
        DecisionTiming timing,
        string originId)
    {

        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException("EntityId cannot be empty", nameof(entityId));
        
        EntityId = entityId;
        ComponentUpdates = componentUpdates ?? throw new ArgumentNullException(nameof(componentUpdates));
        Timing = timing??DecisionTiming.Immediate;
        OriginId = originId??"";
    }
    
    /// <summary>
    /// Get a specific component from updates if present.
    /// </summary>
    public T? GetComponent<T>() where T : IGameEntityComponent
    {
        return ComponentUpdates.TryGetValue(typeof(T), out var component)
            ? (T)component
            : default;
    }

    public Definitions.GameState Apply(Definitions.GameState state)
    {
        // 1. Try Update Agent
        var agentKey = new TurnForge.Engine.ValueObjects.EntityId(System.Guid.Parse(EntityId));
        if (state.Agents.TryGetValue(agentKey, out var agent))
        {
            var clone = (Definitions.Actors.Agent)agent.Clone();
            foreach(var component in ComponentUpdates.Values)
            {
                 clone.ReplaceComponent(component);
            }
            return state.WithAgent(clone);
        }

        // 2. Try Update Prop
        if (state.Props.TryGetValue(agentKey, out var prop))
        {
             var clone = (Definitions.Actors.Prop)prop.Clone();
             foreach(var component in ComponentUpdates.Values)
            {
                 clone.ReplaceComponent(component);
            }
            return state.WithProp(clone);
        }

        // 3. Entity Not Found - Return state as is (or log warning?)
        return state;
    }
}
