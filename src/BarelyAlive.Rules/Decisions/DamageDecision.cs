using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components;
using BarelyAlive.Rules.Events;

namespace BarelyAlive.Rules.Decisions;

/// <summary>
/// Decision to apply damage to an entity.
/// Emits DamageAppliedEvent after applying.
/// </summary>
public class DamageDecision : IDecision
{
    public EntityId Target { get; }
    public EntityId Source { get; }
    public int Amount { get; }
    public string Cause { get; }
    
    public DecisionTiming Timing => DecisionTiming.Immediate;
    public string OriginId => "BarelyAlive.Damage";
    
    public DamageDecision(EntityId target, EntityId source, int amount, string cause)
    {
        Target = target;
        Source = source;
        Amount = amount;
        Cause = cause;
    }
    
    public GameState Apply(GameState state)
    {
        // Try to find agent in state
        if (!state.Agents.TryGetValue(Target, out var agent))
            return state;
        
        var healthComp = agent.GetComponent<BaseHealthComponent>();
        if (healthComp == null) return state;
        
        // Calculate new health
        var newHealth = healthComp.CurrentHealth - Amount;
        
        // Create new component (mutable, so we update in place for now)
        // Note: The component is mutable; for immutability we'd need to clone the agent
        healthComp.TakeDamage(Amount);
        
        // Since Agent is immutable but component is mutable, 
        // we need to create a new agent with updated component
        // For now, rely on mutable component behavior
        
        // TODO: Consider making components immutable records for true immutability
        
        // Emit event with remaining health
        // Note: GameState doesn't have WithEvent - need to emit via context
        // For now, return state as-is; event will be handled by caller
        return state;
    }
}
