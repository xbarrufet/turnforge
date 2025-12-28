# Workflow Interrupt Model

## Overview

When a workflow is **suspended** waiting for user input, the user may execute **interrupt commands** that affect the game state or the workflow itself.

---

## Core Concepts

### Suspension with Allowed Interrupts

```csharp
public interface IAcceptsInput : INode
{
    InputRequirements GetInputRequirements();
    
    /// <summary>
    /// Commands that can be executed while this node awaits input.
    /// Empty = no interrupts allowed (strict wait).
    /// </summary>
    IEnumerable<Type> AllowedInterruptCommands { get; }
}
```

### Interrupt Outcomes

| Outcome | Example | Workflow Effect |
|---------|---------|-----------------|
| **State Modified** | UseArtifact(+1 def) | Workflow continues with updated state |
| **Workflow Ends** | FleeAbility | Workflow terminates (not cancelled, successful escape) |
| **Rejected** | MoveToOtherRoom | Not in AllowedInterruptCommands, rejected |

---

## Flow Diagram

```
┌──────────────────────────────────────────────────────────────┐
│ CombatWorkflow                                               │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│   ┌─────────────┐                                            │
│   │ RollDiceNode│ ← AllowedInterrupts: [UseArtifact, Flee]   │
│   └──────┬──────┘                                            │
│          │                                                   │
│          ▼                                                   │
│   ╔══════════════════════════════════════════════════════╗  │
│   ║ SUSPENDED: Waiting for dice roll                      ║  │
│   ╠══════════════════════════════════════════════════════╣  │
│   ║                                                       ║  │
│   ║  User sends: UseArtifactCommand(Shield)              ║  │
│   ║      → ArtifactWorkflow executes                     ║  │
│   ║      → State: +1 Defense applied                     ║  │
│   ║      → Returns to SUSPENDED state                    ║  │
│   ║                                                       ║  │
│   ║  User sends: DiceResult(4, 5)                        ║  │
│   ║      → Node receives UPDATED state                   ║  │
│   ║      → Re-validates situation                        ║  │
│   ║      → Continues execution                           ║  │
│   ║                                                       ║  │
│   ╚══════════════════════════════════════════════════════╝  │
│          │                                                   │
│          ▼                                                   │
│   ┌─────────────┐                                            │
│   │ ResolveNode │                                            │
│   └─────────────┘                                            │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

---

## Re-validation After Interrupt

When a node receives its expected input after an interrupt:

1. **Get current state** (may have changed)
2. **Re-validate preconditions** (enemy may have fled, combat may be over)
3. **If invalid** → Cancel node with appropriate result
4. **If valid** → Continue with updated context

```csharp
public ValidationResult Validate(WorkflowContext context)
{
    // Always check current state, not cached
    var target = context.State.GetEntity(TargetId);
    if (target == null)
        return ValidationResult.Cancel("Target no longer exists");
    
    return ValidationResult.OkResult;
}
```

---

## Examples

### Example 1: Buff Before Dice Roll

```
1. AttackWorkflow starts → RollAttackNode suspends
2. User: UseArtifactCommand(SwordOfPower)
   → ArtifactWorkflow executes
   → Adds +2 Attack buff to player
   → Returns SUSPENDED
3. User: DiceResult(3)
4. RollAttackNode resumes with +2 Attack in state
```

### Example 2: Escape During Combat

```
1. DefendWorkflow starts → RollDefenseNode suspends
2. User: FleeCommand
   → FleeWorkflow executes
   → Moves player to safe zone
   → Ends with "Escaped" status
3. DefendWorkflow → ENDS (no longer relevant)
```

### Example 3: Rejected Command

```
1. CombatWorkflow → RollDiceNode suspends
   AllowedInterrupts: [UseArtifact, Flee]
2. User: MoveCommand(Direction.North)
   → REJECTED: "Cannot move during combat"
3. User must send DiceResult or allowed interrupt
```

---

## Design Decisions

| Decision | Rationale |
|----------|-----------|
| Node declares allowed interrupts | Node knows its context best |
| Interrupt executes as separate workflow | Clean separation, reusable |
| Re-validation mandatory | State may have invalidated workflow |
| Empty AllowedInterrupts = strict | Backwards compatible with current model |
