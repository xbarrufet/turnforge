# Combat Workflow Example

This example demonstrates how to implement a turn-based combat sequence using the **TurnForge Workflow & Reaction System**. It specifically showcases the **Atomic State Application** pattern, where state changes are generated as `IDecision` objects and applied transactionally, rather than modifying the state directly in reactions.

## Scenario
1. **Attack**: Attacker rolls dice to hit.
2. **Save**: Defender rolls dice to save.
3. **Damage**: Unsaved hits are converted to damage.
4. **Death Check**: If health drops to zero, the actor dies.

## Key Architecture Concepts
- **Workflow**: Orchestrates the steps (`ToHit` -> `Save` -> `ApplyDamage`).
- **Context**: Holds transient data (dice rolls, hits) for the duration of the workflow.
- **Node**: Performs inputs processing (e.g. storing dice results).
- **Reaction**: Analyzing inputs/context and deciding consequences (e.g. "Take 5 Damage").
- **Decision**: The *immutable* state change request (e.g. `DealDamageDecision`).

## Implementation

### 1. Workflow Context
Holds execution-scoped data.

```csharp
public class AttackWorkflowContext : WorkflowContext
{
    public EntityId Attacker { get; init; }
    public EntityId Defender { get; init; }
    public WeaponInfo Weapon { get; init; }

    public int HitCount { get; set; }
    public int UnsavedHits { get; set; }
}
```

### 2. Decisions (State Mutations)
Changes to the game state are defined as Decisions.

```csharp
public record DealDamageDecision(EntityId Target, int Amount) : IDecision
{
    public string OriginId => "CombatSystem";
    public DecisionTiming Timing => DecisionTiming.Immediate;

    public GameState Apply(GameState state)
    {
        // Copy-On-Write: Clone actor, modify, return new state
        if (!state.Actors.TryGetValue(Target, out var actor)) return state;

        // Assuming Actor has a Health component or similar
        // For this example, we assume we can clone and modify properties
        // real implementation would use Components.
        
        // Pseudo-code implementation of Apply logic
        var newActor = actor.Clone();
        // newActor.Attributes.Health -= Amount; 
        
        return state.WithActor(newActor);
    }
}
```

### 3. Logic: Reactions
Reactions determine *what* should happen, but don't *make* it happen immediately.

```csharp
public sealed class ApplyDamageReaction : IReaction
{
    public ReactionId Id => new("ApplyDamage");

    public bool CanReact(WorkflowContext context) => true;

    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        var ctx = (AttackWorkflowContext)context;
        var damage = ctx.UnsavedHits * ctx.Weapon.Damage;

        if (damage > 0)
        {
            // 1. Record the Decision (Transactional)
            context.RecordDecision(new DealDamageDecision(ctx.Defender, damage));

            // 2. Emit Event (for UI or other Reactions)
            context.AddEvent(new DamageAppliedEvent(ctx.Defender, damage));
        }

        return ReactionResult.NoChange(context);
    }
}
```

### 4. Workflow Definition
Nodes define the steps.

```csharp
public class AttackWorkflow : IWorkflow
{
    public WorkflowId Id => new("AttackSequence");
    public INode StartNode { get; }
    public IReadOnlyCollection<IReaction> GlobalReactions { get; } = new List<IReaction>();
    
    // ... Nodes initialization (ToHit -> Save -> Damage -> End) ...
    
    // The ApplyDamageNode defines the Reaction
    // public class ApplyDamageNode : Node { ... AllowedReactions = { new ApplyDamageReaction() } ... }
}
```
