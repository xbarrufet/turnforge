# Command & Execution Flow

Commands form the primary interaction layer with the engine.

## Command Structure

```csharp
public interface ICommand { Type CommandType { get; } }
```

All commands must:
- Be immutable (use `record` types)
- Contain all necessary data
- Not perform business logic (that's in handlers)

## Execution Steps

1. **Command Validation** - FSM checks command allowed in current state
2. **Handler Execution** - Handler generates `IDecision[]` (NO state mutation)
3. **Decision Scheduling** - Decisions enqueued to `Scheduler`
4. **OnCommandExecutionEnd** - Immediate decisions execute
5. **Transition Check** - FSM checks if state should advance
6. **OnStateEnd** - Cleanup decisions for old state
7. **Transition** - FSM moves to new state
8. **OnStateStart** - Init decisions for new state
9. **ACK Wait** - Engine awaits UI acknowledgment

## Command Validation

```csharp
// FSM checks if command allowed
if (!_fsmController.CurrentState.IsCommandAllowed(command.GetType()))
    throw new Exception($"Command {command} not allowed in state {CurrentState}");
```

**Example:** `MoveCommand` only allowed during `PlayerTurnNode`, not during `SetupNode`.

## Decision Timing

Decisions execute at specific moments:

```csharp
public enum DecisionTimingWhen
{
    OnStateStart,            // Entering a phase
    OnStateEnd,              // Leaving a phase
    OnCommandExecutionEnd    // Immediately after command
}
```

**Example:**
```csharp
var decision = new SpawnDecision<AgentDescriptor>(descriptor)
{
    Timing = DecisionTiming.Immediate // Execute now
};

var recurringDecision = new DrawCardDecision()
{
    Timing = new DecisionTiming(
        When: DecisionTimingWhen.OnStateStart,
        Phase: "PlayerTurn",
        Frequency: DecisionTimingFrequency.Permanent // Every turn
    )
};
```

## ACK System

After each command, engine sets `WaitingForACK = true`. No commands accepted until `CommandAck` received.

**Purpose:** Allows UI to animate effects before accepting next input.
