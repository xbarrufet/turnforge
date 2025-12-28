using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Traits.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components;
using BarelyAlive.Rules.Events;
using BarelyAlive.Rules.Decisions;

namespace BarelyAlive.Rules.Traits;

/// <summary>
/// Reactive trait that triggers when any entity moves to this entity's position.
/// Deals damage to the moving entity and destroys itself.
/// 
/// Usage: Add to trap entities (spikes, landmines, etc.)
/// </summary>
public class ExplosiveTrait : IReactiveTrait
{
    // ─────────────────────────────────────────────────────────────
    // Trait Data
    // ─────────────────────────────────────────────────────────────
    
    public int Damage { get; init; } = 5;
    
    /// <summary>
    /// The entity that owns this trait (the trap).
    /// </summary>
    public GameEntity Owner { get; private set; } = null!;
    
    // ─────────────────────────────────────────────────────────────
    // IReactiveTrait
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// This trait activates when any entity moves to a position.
    /// </summary>
    public Type TriggerEvent => typeof(MovedToEvent);
    
    // ─────────────────────────────────────────────────────────────
    // IReaction Implementation
    // ─────────────────────────────────────────────────────────────
    
    public ReactionId Id => new("BarelyAlive.Trait.Explosive");
    
    public bool CanReact(WorkflowContext context)
    {
        // Check if any MovedToEvent targets our position
        var posComponent = Owner.GetComponent<BasePositionComponent>();
        if (posComponent == null) return false;
        
        var myPosition = posComponent.CurrentPosition;
        if (myPosition == Position.Empty) return false;
        
        return context.PendingEvents.OfType<MovedToEvent>()
            .Any(evt => evt.NewPosition == myPosition);
    }
    
    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        var posComponent = Owner.GetComponent<BasePositionComponent>();
        if (posComponent == null) return ReactionResult.NoChange(context);
        
        var myPosition = posComponent.CurrentPosition;
        if (myPosition == Position.Empty) return ReactionResult.NoChange(context);
        
        var relevantEvents = context.PendingEvents.OfType<MovedToEvent>()
            .Where(evt => evt.NewPosition == myPosition);
        
        foreach (var evt in relevantEvents)
        {
            // Apply damage to the entity that stepped on us
            context.RecordDecision(new DamageDecision(
                evt.AgentId, 
                Owner.Id, 
                Damage,
                "Trap"));
            
            // Destroy self (one-time trap)
            context.RecordDecision(new DestroyEntityDecision(Owner.Id, null, "Triggered"));
        }
        
        return ReactionResult.NoChange(context);
    }
    
    // ─────────────────────────────────────────────────────────────
    // Factory
    // ─────────────────────────────────────────────────────────────
    
    public ExplosiveTrait WithOwner(GameEntity owner)
    {
        return new ExplosiveTrait { Damage = this.Damage, Owner = owner };
    }
}
