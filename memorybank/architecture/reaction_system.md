# Reaction System Architecture

*Establishes the contract for the TurnForge Reaction System.*

## 1. Classification of Reactions

There are strictly two primary types of reactions based on execution:

### A. Passive (Automatic)
- **Constraint**: `RequiresInput == false`
- **Behavior**: Executes automatically whenever `CanReact` returns true.
- **Orchestration**: Never suspends the action.

### B. Manual (Optional)
- **Constraint**: `RequiresInput == true`
- **Behavior**: Requires external decision (Player/AI) to execute.
- **Orchestration**:
    - Suspends the action (`Status = Suspended`).
    - Executes **only** if the input confirms it.

---

## 2. Resolution Types

A reaction can resolve in three ways:

1.  **Modify Input** (`ModifiedInput != null`)
    - *Examples*: Reroll dice, modify value, convert result.
    - Can be Passive (auto-reroll) or Manual (optional reroll).

2.  **Modify State** (Side Effect)
    - *Examples*: Apply condition, spend resource, set flag.
    - Mutates `ActionContext` directly.
    - Can be Passive or Manual.

3.  **Launch Nested Action** (`NestedAction != null`)
    - *Examples*: Team Action, Extra Attack, Item usage.
    - Can be Passive (auto-trigger) or Manual (optional trigger).
    - Regulated by `ExecuteNestedAction` flag.

---

## 3. The Reaction Matrix

| Reaction Type | Resolution | Suspends? | Launches Action? |
| :--- | :--- | :--- | :--- |
| **Passive** | Modify Input | ❌ No | ❌ No |
| **Passive** | Modify State | ❌ No | ❌ No |
| **Passive** | Mod. State + Action | ❌ No | ✅ Yes |
| **Manual** | Modify Input | ✅ Yes | ❌ No |
| **Manual** | Modify State | ✅ Yes | ❌ No |
| **Manual** | Mod. State + Action | ✅ Yes | ✅ Yes |

*Any combination outside this matrix is considered invalid.*

---

## 4. Orchestrator Golden Rules

The `ActionOrchestrator` must adhere to these rules strictly:

1.  **Priority**: Execute **all** applicable Passive reactions first.
2.  **Suspension**: If a Manual reaction is applicable but has no input → **Suspend**.
3.  **Safety**: Never execute a Manual (Optional) action without explicit input confirming it (`ExecuteNestedAction` check).
4.  **Concurrency**: Never execute two Manual actions simultaneously.
5.  **Determinism**: The order of resolution must be deterministic.

## 5. Implementation Definition

The `ReactionResult` class is the sole carrier of this contract:

```csharp
public sealed class ReactionResult
{
    // Primary Classification
    public bool RequiresInput { get; }

    // Resolution Payload
    public IInputActionResult? ModifiedInput { get; }
    public IAction? NestedAction { get; }

    // Execution Control
    public bool ExecuteNestedAction { get; }
}
```
