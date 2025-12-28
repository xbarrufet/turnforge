# Reaction Patterns

Common patterns for implementing game rules as Reactions.

---

## Base Pattern

```csharp
public class MyReaction : IReaction
{
    public ReactionId Id { get; } = new("Game.MyReaction");

    public bool CanReact(WorkflowContext context)
    {
        // Check if this reaction applies
        return context.PendingEvents.OfType<MyEvent>().Any();
    }

    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        // Process the event and record decisions
        var evt = context.PendingEvents.OfType<MyEvent>().First();
        context.RecordDecision(new MyDecision(evt.Data));
        return ReactionResult.Continue();
    }
}
```

---

## Pattern 1: Trait-Based Trigger

React when entity with specific trait is involved.

```csharp
public class TrapReaction : IReaction
{
    public ReactionId Id { get; } = new("Game.Trap");

    public bool CanReact(WorkflowContext context)
    {
        var movedEvents = context.PendingEvents.OfType<MovedToEvent>();
        if (!movedEvents.Any()) return false;

        var state = context.GetProjectedState();
        return movedEvents.Any(evt => 
            state.GetEntitiesAt(evt.NewPosition)
                 .Any(e => e.HasTrait<ExplodeOnWalkOver>()));
    }

    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        var state = context.GetProjectedState();
        foreach (var evt in context.PendingEvents.OfType<MovedToEvent>())
        {
            var traps = state.GetEntitiesAt(evt.NewPosition)
                             .Where(e => e.HasTrait<ExplodeOnWalkOver>());
            
            foreach (var trap in traps)
            {
                var damage = trap.GetTrait<ExplodeOnWalkOver>()!.Damage;
                context.RecordDecision(new DamageDecision(evt.AgentId, damage));
                context.RecordDecision(new DestroyEntityDecision(trap.Id));
            }
        }
        return ReactionResult.Continue();
    }
}
```

---

## Pattern 2: Cost Modifier

Modify resource costs based on game state.

```csharp
public class CrowdCostReaction : IReaction
{
    public ReactionId Id { get; } = new("Game.CrowdCost");

    public bool CanReact(WorkflowContext context)
    {
        return context.PendingEvents.OfType<MovementCostEvent>().Any();
    }

    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        var moveCtx = context as MoveWorkflowContext;
        if (moveCtx == null) return ReactionResult.NoChange(context);
        
        var state = context.GetProjectedState();
        var enemies = state.GetEnemiesAt(moveCtx.Target);
        
        // Add +1 cost per enemy (Zombicide "Grab" rule)
        moveCtx.Cost += enemies.Count;
        
        return ReactionResult.Continue();
    }
}
```

---

## Pattern 3: Requires Input

React but need player decision first.

```csharp
public class ChooseTargetReaction : IReaction
{
    public ReactionId Id { get; } = new("Game.ChooseTarget");

    public bool CanReact(WorkflowContext context)
    {
        return context.PendingEvents.OfType<AttackDeclaredEvent>().Any()
            && !context.Has("SelectedTarget");
    }

    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        if (input is TargetSelectionInput selection)
        {
            context.Set("SelectedTarget", selection.TargetId);
            return ReactionResult.Continue();
        }
        
        // Suspend and request input
        return ReactionResult.InputRequired(context);
    }
}
```

---

## Pattern 4: Nested Workflow

Trigger a sub-workflow from reaction.

```csharp
public class DeathReaction : IReaction
{
    public ReactionId Id { get; } = new("Game.Death");

    public bool CanReact(WorkflowContext context)
    {
        return context.PendingEvents.OfType<DamageAppliedEvent>()
            .Any(evt => evt.RemainingHealth <= 0);
    }

    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        var deathEvent = context.PendingEvents
            .OfType<DamageAppliedEvent>()
            .First(evt => evt.RemainingHealth <= 0);
        
        // Launch death workflow (loot drop, XP distribution, etc.)
        var deathWorkflow = new DeathWorkflow(deathEvent.EntityId);
        return ReactionResult.WithNestedWorkflow(context, deathWorkflow, executeImmediately: true);
    }
}
```

---

## Pattern 5: Chain Reaction (Emit New Event)

Reaction that triggers other reactions.

```csharp
public class KillReaction : IReaction
{
    public ReactionId Id { get; } = new("Game.Kill");

    public bool CanReact(WorkflowContext context)
    {
        return context.PendingEvents.OfType<EntityDestroyedEvent>()
            .Any(evt => evt.Cause == "Damage");
    }

    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        var killEvent = context.PendingEvents
            .OfType<EntityDestroyedEvent>()
            .First();
        
        // Emit new event for XP/loot reactions
        context.AddEvent(new EnemyKilledEvent(
            Killer: killEvent.By,
            Victim: killEvent.EntityId,
            XPValue: 10
        ));
        
        return ReactionResult.Continue();
    }
}
```

---

## Registration

```csharp
GlobalReactions = new List<IReaction>
{
    // Order matters! First match wins for conflicting reactions
    new TrapReaction(),        // Priority: hazards
    new CrowdCostReaction(),   // Priority: cost modifiers
    new DeathReaction(),       // Priority: entity removal
    new KillReaction(),        // Priority: post-death
    new XPReaction()           // Priority: rewards
};
```
