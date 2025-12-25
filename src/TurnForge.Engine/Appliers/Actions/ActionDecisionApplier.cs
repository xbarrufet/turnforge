using TurnForge.Engine.Appliers.Effects;
using TurnForge.Engine.Appliers.Entity;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Decisions.Actions;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Appliers.Actions;

/// <summary>
/// Applier for ActionDecisions - applies component updates to entities.
/// </summary>
/// <remarks>
/// Receives decisions from FSM and applies component changes to GameState.
/// 
/// Process:
/// 1. Get entity (Agent or Prop) from state
/// 2. Apply each component update via entity.AddComponent()
/// 3. Update state with modified entity (WithAgent/WithProp)
/// 4. Generate ComponentsUpdatedEffect for UI
/// 
/// Design: Uses mutable pattern for entity components + immutable GameState.
/// </remarks>
public sealed class ActionDecisionApplier : IApplier<ActionDecision>
{
    public ApplierResponse Apply(ActionDecision decision, GameState state)
    {
        if (decision == null)
            throw new ArgumentNullException(nameof(decision));
        
        if (state == null)
            throw new ArgumentNullException(nameof(state));
        
        // Try Agent first, then Prop
        var agent = state.GetAgents().FirstOrDefault(a => a.Id.ToString() == decision.EntityId);
        if (agent != null)
        {
            ApplyComponents(agent, decision);
            return CreateResponse(state.WithAgent(agent), agent.Id, decision);
        }
        
        var prop = state.GetProps().FirstOrDefault(p => p.Id.ToString() == decision.EntityId);
        if (prop != null)
        {
            ApplyComponents(prop, decision);
            return CreateResponse(state.WithProp(prop), prop.Id, decision);
        }
        
        // Entity not found - return unchanged state (graceful degradation)
        return new ApplierResponse(state, Array.Empty<IGameEffect>());
    }
    
    /// <summary>
    /// Apply component updates to any GameEntity (Agent or Prop).
    /// Uses mutable AddComponent pattern.
    /// </summary>
    private void ApplyComponents(GameEntity entity, ActionDecision decision)
    {
        foreach (var (_, component) in decision.ComponentUpdates)
        {
            entity.AddComponent(component);
        }
    }
    
    /// <summary>
    /// Create applier response with updated state and effect.
    /// </summary>
    private ApplierResponse CreateResponse(
        GameState newState, 
        EntityId entityId, 
        ActionDecision decision)
    {
        var effect = new ComponentsUpdatedEffect(
            entityId,
            decision.ComponentUpdates.Keys.ToArray()
        );
        
        return new ApplierResponse(newState, new[] { effect });
    }
}
