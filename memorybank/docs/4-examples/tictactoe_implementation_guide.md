# TicTacToe Implementation Guide

## Summary

A minimal TicTacToe game (~280 lines) demonstrating TurnForge.Core patterns. Serves as a reference for implementing simple turn-based games.

---

## Why This Architecture?

### The Problem with Direct State Management

```csharp
// ❌ Tempting but problematic approach
public class TicTacToeGame {
    private CellState[] _board;  // Direct field
    
    public void PlaceMark(int pos) {
        _board[pos] = currentPlayer;  // Mutation
    }
}
```

**Issues:**
- No undo/redo capability
- Can't serialize game state
- No validation pipeline
- No separation of concerns
- Can't replay or analyze games

---

### The TurnForge Approach

```csharp
// ✅ Clean architecture
public void PlaceMark(int position) {
    var context = new TicTacToeContext(_state);
    context.SetTypedData(new PlaceMarkData(position));
    
    _orchestrator.Execute(_placeMarkWorkflow, context);
    
    _state = context.State;
}
```

**Benefits:**
| Benefit | How TurnForge Provides It |
|---------|--------------------------|
| **Immutable State** | GameState is immutable, changes create new instances |
| **Undo/Redo** | `GetState()` / `SetState()` for full state snapshots |
| **Validation** | Workflow nodes can cancel invalid moves |
| **Extensibility** | Add reactions without changing core logic |
| **Testability** | Each node can be unit tested |

---

## Implementation Decisions

### 1. Why `GameState.Metadata` Instead of Custom State?

```csharp
_state = GameState.Empty()
    .WithMetadata("Board", TicTacToeBoard.CreateEmptyBoard())
    .WithMetadata("CurrentPlayer", Player.X)
    .WithMetadata("GameResult", GameResult.InProgress);
```

**Rationale:**
- TurnForge already provides immutable `GameState`
- No need to create custom state class
- Metadata is serializable by default
- Consistent with Parchís and other games

**Trade-off:** Type casting required (`(CellState[])_state.Metadata["Board"]`)

---

### 2. Why Typed Workflow Data (`IWorkflowData`)?

```csharp
public record PlaceMarkData(int Position) : IWorkflowData;

// Usage
context.SetTypedData(new PlaceMarkData(position));
var data = context.GetTypedData<PlaceMarkData>();
```

**Rationale:**
- Compile-time type safety
- No magic strings for keys
- Autocomplete in IDE
- Clear contract between nodes

**Alternative rejected:** `context.Set("Position", 4)` - error-prone

---

### 3. Why 4 Nodes Instead of 1?

```
ValidatePlacement → PlaceMark → CheckResult → SwitchPlayer
```

**Rationale:**
- **Single Responsibility**: Each node does one thing
- **Testability**: Can test validation separately
- **Extensibility**: Easy to add logging, animations, etc.
- **Clarity**: Flow is readable

**Alternative rejected:** Single node doing everything - monolithic

---

### 4. Why `WorkflowOrchestrator.Execute()` Over Direct Calls?

```csharp
_orchestrator.Execute(_placeMarkWorkflow, context);
```

**Rationale:**
- Consistent with TurnForge's execution model
- Supports nested workflows (future)
- Handles suspension/resumption (if needed)
- Provides execution result with status

---

### 5. Why `TicTacToeContext : WorkflowContext`?

```csharp
public class TicTacToeContext : WorkflowContext
{
    public TicTacToeContext(GameState state)
    {
        InitializeState(state);
    }
}
```

**Rationale:**
- `WorkflowContext` is abstract (can't instantiate directly)
- Encapsulates game-specific initialization
- Could add game-specific methods later
- Clean separation from engine

---

### 6. Why Decisions Apply Immediately?

```csharp
public void RecordDecision(IDecision decision)
{
    _appliedDecisions.Add(decision);
    _workingState = decision.Apply(_workingState);  // Immediate!
}
```

**Rationale:**
- Subsequent nodes see updated state
- No need to track pending decisions
- Simpler mental model
- History preserved in `Decisions` list

---

## Comparison: Direct vs TurnForge

| Aspect | Direct | TurnForge |
|--------|--------|-----------|
| Lines of code | ~100 | ~280 |
| Undo/Redo | ❌ Manual | ✅ Built-in |
| Validation | ❌ If statements | ✅ Cancellable nodes |
| Extensibility | ❌ Modify code | ✅ Add reactions |
| Testability | ⚠️ Integration | ✅ Unit tests |
| Serialization | ❌ Custom | ✅ GameState |
| Learning curve | Low | Medium |

---

## When to Use This Pattern

**Use TurnForge for:**
- Games needing undo/redo
- Games with complex validation
- Games requiring save/load
- Multi-step turns
- Games you want to extend later

**Consider simpler approach for:**
- Throwaway prototypes
- Games with trivial rules
- Performance-critical scenarios (millions of moves/sec)

---

## File Structure

```
src/TicTacToe.Rules/
├── TicTacToeGame.cs          # All game code (~280 lines)
│   ├── Enums                 # Player, CellState, GameResult
│   ├── TicTacToeBoard        # Static board logic
│   ├── TicTacToeBootstrap    # Creates initial GameState
│   ├── TicTacToeContext      # Concrete WorkflowContext
│   ├── PlaceMarkData         # Typed workflow data
│   ├── Workflow Factory      # Creates workflow
│   ├── Nodes                 # Validate, PlaceMark, CheckResult, SwitchPlayer
│   ├── Decision              # UpdateStateDecision
│   ├── Command               # PlaceMarkCommand
│   └── TicTacToeGame         # Public API
└── TicTacToe.Rules.csproj

tests/TicTacToe.Simulation/
├── Program.cs                # Console simulation with AI
└── TicTacToe.Simulation.csproj
```

---

## Execution Flow Diagram

```
┌──────────────────────────────────────────────────────────────────┐
│  TicTacToeGame.PlaceMark(position)                               │
├──────────────────────────────────────────────────────────────────┤
│  1. Validate position (quick checks)                             │
│  2. Create TicTacToeContext(currentState)                        │
│  3. context.SetTypedData(PlaceMarkData)                          │
│  4. orchestrator.Execute(workflow, context)                      │
│     ┌────────────────────────────────────────────────────────┐   │
│     │  WorkflowOrchestrator                                  │   │
│     ├────────────────────────────────────────────────────────┤   │
│     │  ValidatePlacementNode.Validate(context)               │   │
│     │    └── Check position valid, cell empty, game active   │   │
│     │  PlaceMarkNode.Validate(context)                       │   │
│     │    └── Clone board, place mark, RecordDecision         │   │
│     │  CheckResultNode.Validate(context)                     │   │
│     │    └── Check win/draw, RecordDecision                  │   │
│     │  SwitchPlayerNode.Validate(context)                    │   │
│     │    └── If game active, switch player, RecordDecision   │   │
│     └────────────────────────────────────────────────────────┘   │
│  5. _state = context.State (decisions already applied)           │
│  6. Return PlaceMarkResult                                       │
└──────────────────────────────────────────────────────────────────┘
```
