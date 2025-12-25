# Orchestrator

**The ONLY place where `GameState` mutates.**

## Applier Registration

```csharp
orchestrator.RegisterApplier(new AgentSpawnApplier(factory));
orchestrator.RegisterApplier(new MoveApplier());
orchestrator.RegisterApplier(new AttackApplier());
```

## Decision Execution

```csharp
public IGameEffect[] Apply(IDecision decision)
{
    // Find registered applier by decision type
    var applier = _appliers[decision.GetType()];
    
    // Execute (dynamic dispatch)
    var response = ((dynamic)applier).Apply((dynamic)decision, CurrentState);
    
    // Update state (ONLY mutation point)
    CurrentState = response.GameState;
    
    return response.GameEffects;
}
```

## Applier Pattern

```csharp
public interface IApplier<TDecision> where TDecision : IDecision
{
    ApplierResponse Apply(TDecision decision, GameState state);
}

public record ApplierResponse(GameState GameState, IGameEffect[] GameEffects);
```

**Rules:**
- ✅ Receives immutable state
- ✅ Returns new state (never mutates input)
- ✅ Generates effects for UI/logging
- ❌ No business logic (that's in handlers/strategies)

**Example:**
```csharp
public class MoveApplier : IApplier<MoveDecision>
{
    public ApplierResponse Apply(MoveDecision decision, GameState state)
    {
        var agent = state.Agents[decision.AgentId];
        var updatedAgent = agent.WithPosition(decision.To);
        var newState = state.WithAgent(updatedAgent);
        
        var effect = new AgentMovedEffect(decision.AgentId, decision.From, decision.To);
        return new ApplierResponse(newState, [effect]);
    }
}
```

## Scheduler

Manages delayed/recurring decisions:

```csharp
// Enqueue decisions
CurrentState = CurrentState.WithScheduler(Scheduler.Add(decisions));

// Execute scheduled decisions matching phase/timing
var toExecute = Scheduler.GetDecisions(d => 
    d.Timing.Phase == phase && d.Timing.When == when);

foreach (var decision in toExecute)
{
    var effects = Apply(decision);
    
    // Remove if single-use
    if (decision.Timing.Frequency == Single)
        Scheduler.Remove(decision);
}
```
