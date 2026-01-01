# TurnForge Backlog

Ideas and improvements to implement in the future.

---

## IDEA: Core Actions Architecture

**Status:** Design Approved  
**Priority:** High  
**Context:** Need to distinguish between game-specific actions and engine-provided actions.

**Design Decision:**

All actions follow the same pattern, but with different complexity levels:

| Type | Structure | Example |
|------|-----------|---------|
| **Composite Action** | Multiple chained nodes | `StartGame` (ProcessBoard → ProcessPlayers → Deploy → Build) |
| **Atomic Action** | Single node with one operation | `Spawn` (only SpawnNode) |

**Key Principles:**

1. **Uniform API**: All actions are dispatched the same way
2. **Uniform Registration**: All actions register the same way
3. **Composability**: Composite actions can internally call atomic actions (future)
4. **Simplicity**: No need to differentiate "Actions" vs "Core Operations" at API level

**Core Actions (provided by TurnForge):**

- `StartGame` → Composite: initializes board, players, entities
- `Spawn` → Atomic: creates a single entity

**Game Actions (provided by game rules):**

- Registered via `IActionRegistry` in game-specific registration class
- Examples: `Move`, `Attack`, `EndTurn`

**Access Points:**

1. **External**: User/UI via `GameEngineRuntime.Dispatch()`
2. **FSM Nodes**: Via `OnEntryActions` (already supported)
3. **Action Nodes**: (Future) Via nested action execution

**Rationale:** This unified model keeps the architecture simple while allowing both simple (atomic) and complex (composite) behaviors under the same abstraction.

---

## IDEA: Action-FSM Notification (AP Consumption)

**Status:** ✅ Implemented  
**Priority:** High  
**Context:** Currently, simulations manually access FSM nodes to consume AP:
```csharp
// BAD: Simulation manipulates FSM directly
var turnNode = fsmGraph.GetNode("Turn") as ParchisTurnNode;
turnNode.ConsumeAction(roll == 6);
```

**Problem:** Actions don't notify the FSM when they complete. The FSM can't track AP consumption or action completion.

**Design:**

1. **Actions notify FSM** after completion via an event/callback
2. **FSM TurnNode** observes action results and updates AP accordingly
3. **Roll 6 = bonus** → Action tells FSM "don't consume AP"

**Proposed Flow:**
```
User calls: engine.ExecuteAction(Move, { Roll = 5 })
   │
   ├── FSM validates: "Move allowed in TurnNode?" → YES
   │
   ├── MoveAction executes
   │   └── Returns: ActionResult { ConsumedAP = true, BonusTurn = false }
   │
   └── FSM receives notification
       └── TurnNode.OnActionCompleted(result) → Consumes AP
```

**Implementation Options:**

A) **ActionResult carries metadata** (recommended):
```csharp
public record ActionResult
{
    public bool ConsumedAP { get; init; } = true;
    public bool BonusTurn { get; init; } = false;
}
```

B) **Events emitted by Action**:
Actions emit `ActionCompletedEvent` that FSM subscribes to.

**Benefits:**
- Simulation only calls `ExecuteAction`, no FSM manipulation
- Clean separation: Action = logic, FSM = flow control
- AP rules encapsulated in Move action, not spread across codebase

---

## IDEA: Dynamic Turn Order Structure

**Status:** ✅ Implemented  
**Priority:** High  
**Context:** Current Parchís FSM has 1 fixed node per color:
```
Turn_Red → Turn_Blue → Turn_Green → Turn_Yellow
```
This doesn't work for variable player counts (2, 3, 6 players).

**Problem:** FSM nodes are hardcoded at graph creation time.

**Proposal:** Add `TurnOrderState` structure to `GameState`:
```csharp
public record TurnOrderState(
    IReadOnlyList<PlayerId> PlayerOrder,
    int CurrentPlayerIndex,
    int RoundNumber
);
```

**Implementation:**
1. Single generic `TurnNode` that reads `GameState.TurnOrder.CurrentPlayer`
2. `StartRoundNode.OnEntry` → workflow that resets CurrentPlayerIndex to 0
3. `TurnNode.IsCompleted` → when AP = 0
4. `TurnNode.GetNextNode` → increments CurrentPlayerIndex via applier
5. When `CurrentPlayerIndex >= PlayerCount` → transition to EndRound

**Benefits:**
- Variable player count support
- Turn order can be modified mid-game (skip player, reverse order)
- State is serializable (save/load games)

---

## IDEA: Entity Owner Query API

**Status:** ✅ Implemented  
**Priority:** Medium  
**Context:** Currently getting player entities requires string matching:
```csharp
state.Entities.Values.Where(e => e.DefinitionId.Contains($"pawn_{color}"))
```

**Proposal:** Add to `GameStateView`:
- `GetEntitiesForOwner(PlayerId)` - all entities owned by player
- `GetEntities<TDefinition>(PlayerId)` - filtered by definition type

**Implementation:**
1. Actors with `TeamTrait` → `TeamComponent` with `PlayerId` owner
2. Query via component, not string matching
3. Consolidate `GameStateView` as the single query entry point

**Future:** `GetEntities<TEntityView>(player)` with source generation

---

## IDEA: Category Refactoring

**Status:** Pending  
**Priority:** Low  
**Context:** Category is currently a string, limiting query capabilities.

**Proposal:** Refactor to class structure enabling:
- Query entities by category
- Link same-category entities to shared skills
- Hierarchical categories

---

## IDEA: Generic WorkflowContext with Custom View

**Status:** Pending  
**Priority:** Medium  
**Context:** Current API uses generic `GameStateView` with methods like `GetEntity()`, which lacks domain semantics.

**Current Workaround:** Extension methods provide semantic API:
```csharp
public static class ParchisViewExtensions
{
    public static IEnumerable<Actor> GetPawns(this GameStateView view, PlayerId owner) => ...
}
```

**Proposal:** Allow injecting custom View type via generic WorkflowContext:
```csharp
public class ParchisWorkflowContext : WorkflowContext<ParchisGameView>
{
    public override ParchisGameView CreateView(GameState state, GameStateOverlay overlay)
        => new ParchisGameView(new GameStateView(state, overlay));
}

// At workflow node:
public override WorkflowStepResult Execute(WorkflowContext<ParchisGameView> context)
{
    var pawns = context.View.GetPawns(playerId);  // Strongly typed!
}
```

**Benefits:**
- Compile-time type safety for domain-specific queries
- Better developer experience with autocomplete
- Forces consistent domain API usage

**Implementation Steps:**
1. Make `WorkflowContext` generic: `WorkflowContext<TView>`
2. Add abstract `CreateView()` method
3. Update `WorkflowOrchestrator` to work with generic contexts
4. Games implement their own `TView` (e.g., `ParchisGameView`)

---

## IDEA: Game Over Handling and FSM Reset

**Status:** ✅ Implemented  
**Priority:** High  
**Context:** When a game ends (IsGameOver = true), the engine should:
1. Signal to UI that the game has ended
2. Reset the FSM graph to initial state
3. Return to waiting for a new StartGame command

**Current Problem:** After `WorkflowTransaction.IsGameOver = true`, the FSM remains in its final state and there's no clean way to start a new game.

**Proposal:**
1. Add `ResetGame()` method to `IGameEngine`
2. `FsmGraph.Reset()` to return to root node
3. Clear current `GameState` from repository
4. Return `GameOverResult` with winner info

**Implementation:**
```csharp
public interface IGameEngine
{
    // Existing
    WorkflowTransaction ExecuteWorkflow(...);
    
    // New
    void ResetGame();  // Clears state and resets FSM
    GameStatus GetStatus();  // WaitingForStart, InProgress, GameOver
}
```

**Example Flow:**
```csharp
var result = engine.ExecuteWorkflow(ParchisWorkflows.Move, params);
if (result.IsGameOver)
{
    ShowWinner(result);
    engine.ResetGame();  // Ready for new game
}
```

---
