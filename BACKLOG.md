# TurnForge Backlog

Ideas and improvements to implement in the future.

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
