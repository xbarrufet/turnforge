# FSM 2.0: Architecture & Design

## Overview
The Finite State Machine (FSM) in TurnForge has been refactored to prioritize type safety, explicitness, and separation of concerns. The new architecture moves away from loose enums and complex node hierarchies towards a flatter, command-driven design.

## Key Changes

### 1. Command Pattern Integration
*   **Old Way**: Using `CommandType` enums.
*   **New Way**: Using strictly typed C# classes implementing `ICommand`.
*   **Rationale**: This enables payload validation at the compiler level and allows pattern matching in nodes.

### 2. Flattened Node Structure
The `BaseFsmNode` has been simplified. We no longer enforce a strict parent-child tree structure for execution. Instead, the FSM is viewed as a flat collection of possible states, where transitions are explicit.

### 3. Strict Type Checking
Methods like `IsCommandAllowed` now take `Type` arguments instead of strings or enums.
```csharp
public override bool IsCommandAllowed(Type commandType) => commandType == typeof(StartGameCommand);
```

## Implementation Details

### BaseFsmNode
The base class now focuses on:
*   **Identity**: `NodeId` and Name.
*   **Permissions**: `GetAllowedCommands()` returns a list of allowed C# Types.
*   **Completion Logic**: `IsCompleted(GameState)` checks pure data in `GameState` to determine if transition is needed.
*   **Next Node Logic**: `GetNextNode(GameState)` returns the next node to transition to, is a funcion -> allows to have complex 
logic to determine the next node.
*   **OnEntry Actions**: System actions that execute automatically on node entry (preferred).
*   **Resolvers** [Deprecated]: Legacy resolver calls, use OnEntry actions instead.

### System Actions (OnEntry)

System actions execute automatically when entering a node. Unlike interactive actions, they don't suspend for user input.

```csharp
var endRoundNode = new FsmNode("EndRound")
    .OnEntry(new ResetActionPointsAction())      // Executes first
    .OnEntry(new EvaluateSpawnRulesAction())     // Executes second
    .WithCompletionCondition(_ => true);
```

**Action Types:**
| Type | Input | Example |
|------|-------|---------|
| **Interactive** | Waits for user | StartGame, SelectTarget |
| **System** | Automatic | Spawn, ResetAP, DrawCards |

**Execution Order on Node Entry:**
1. OnEntry Actions execute (in order)
2. Legacy Resolvers execute (for backward compatibility)
3. Check completion condition

### System Actions & Overlay Transaction

System actions use the same transactional overlay mechanism as interactive actions, but **complete immediately** without suspending.

#### How FsmGraph Executes System Actions

```csharp
FsmGraph.ExecuteNodeEntry()
├─ For each OnEntryAction:
│  ├─ Create SystemActionContext(currentState)
│  ├─ ActionOrchestrator.StartAction(action, context)
│  │  ├─ InitializeState() → Creates GameStateOverlay
│  │  ├─ Execute all nodes (use context.Overlay)
│  │  └─ Commit overlay → New GameState
│  └─ Update FsmGraph state with committed result
└─ Execute legacy Resolvers (if any)
```

#### Key Differences: System vs Interactive

| Aspect | System Actions | Interactive Actions |
|--------|-----------------|----------------------|
| **Suspension** | Never suspends | May suspend for input |
| **Completion** | Immediate | Asynchronous |
| **Overlay** | Created, used, committed in one call | May span multiple resume cycles |
| **Context** | `SystemActionContext` | Custom `ActionContext` |

#### Example: Spawn System Action

```csharp
public class EvaluateSpawnRulesAction : IAction
{
    public ActionStepResult Execute(ActionContext context)
    {
        // Get state from context
        var state = context.State;
        
        // Use context's overlay (shared across action)
        var view = new GameStateView(state, context.Overlay);
        
        // Evaluate spawn rules and record operations
        spawnOrchestrator.ExecuteSpawns(view, context.Overlay);
        
        return ActionStepResult.Success();
        // Orchestrator commits overlay after this returns
    }
}
```

**Result**: When the FSM node entry completes, all spawn operations have been committed atomically to the new GameState.

### Usage Example (RootNode)
```csharp
internal sealed class RootNode : BaseFsmNode
{
    public RootNode(Guid id) { Id = new NodeId(id.ToString()); Name = "Root"; }
    
    // Explicitly binding the Command Type
    public override bool IsCommandAllowed(Type commandType) => commandType == typeof(StartGameCommand);
    
    public override IReadOnlyList<Type> GetAllowedCommands() => new[] { typeof(StartGameCommand) };

    public override bool IsCompleted(GameState state) 
    {
        // Pure function checking state
        return false; 
    }
}
```

### 3. Turn Management

TurnForge provides generic nodes for turn-based games in `TurnForge.Engine.Core.Fsm.Nodes`.

#### TurnOrderState

Tracks turn order in `GameState`:

```csharp
public record TurnOrderState(
    ImmutableList<PlayerId> PlayerOrder,
    int CurrentPlayerIndex,
    int RoundNumber
) {
    public PlayerId CurrentPlayer => PlayerOrder[CurrentPlayerIndex];
    public bool IsRoundComplete => CurrentPlayerIndex >= PlayerOrder.Count;
    public TurnOrderState NextPlayer() => this with { CurrentPlayerIndex = CurrentPlayerIndex + 1 };
    public TurnOrderState NextRound() => this with { CurrentPlayerIndex = 0, RoundNumber = RoundNumber + 1 };
}
```

#### StartRoundNode

Controls turn order. Decides who plays next or if round is over.

```csharp
public class StartRoundNode : BaseFsmNode
{
    public override bool IsCompleted(GameState state) => true; // Immediate
    
    public override BaseFsmNode? GetNextNode(GameState state)
    {
        if (state.TurnOrder.IsRoundComplete)
            return _endRoundNode;  // All players done
        return _turnNode;           // Next player's turn
    }
}
```

**OnEntry Actions**: Reset AP, spawn enemies, draw cards, etc.

#### TurnNode

Executes a single player's turn. **Always returns to StartRoundNode**.

```csharp
public class TurnNode : BaseFsmNode
{
    public override BaseFsmNode? GetNextNode(GameState state)
    {
        return _startRoundNode; // Always back to StartRound
    }
}
```

#### Flow Pattern

```
StartRound → Turn → EndRound
                      ↓ (if not IsRoundComplete)
                   StartRound
                      ↓ (if IsRoundComplete + winner)  
                   EndGame
```

**Responsibilities:**
- **StartRound**: Prepares turn, selects current player, transitions to Turn
- **Turn**: Executes player actions, transitions to EndRound when done
- **EndRound**: Checks IsRoundComplete, decides continue or end game

#### Usage Example

```csharp
// Create turn order
var turnOrder = TurnOrderState.Create(new[] { player1, player2, player3 });
var state = stateBuilder.SetTurnOrder(turnOrder).Build();

// Create FSM nodes
var startRound = new StartRoundNode()
    .OnEntry(new AdvanceTurnAction())  // Advances to next player
    .WithTurnNode(turn);

var turn = new TurnNode()
    .WithEndRound(endRound);

var endRound = new EndRoundNode()
    .OnEntry(new StartNewRoundAction())  // Resets turn order when round complete
    .WithStartRound(startRound)
    .WithEndGame(endGame);
```

#### Updating TurnOrderState

TurnOrderState is updated via OnEntry actions:

| Action | Location | Action |
|----------|----------|--------|
| `AdvanceTurnAction` | StartRoundNode.OnEntry | `NextPlayer()` (skip if index=0) |
| `StartNewRoundAction` | EndRoundNode.OnEntry | `NextRound()` (reset to player 0) |

```csharp
// Wiring example
var startRound = new StartRoundNode()
    .OnEntry(AdvanceTurnActionFactory.Create())
    .WithTurnNode(turn);

var endRound = new EndRoundNode()
    .OnEntry(StartNewRoundActionFactory.Create())
    .WithStartRound(startRound);
```

Both actions use `SetTurnOrderOperation` which commits via the overlay transactionally.

#### Extending for Game-Specific Logic

```csharp
// Parchís example: AP management
public class ParchisTurnNode : TurnNode
{
    private int _actionPoints = 1;
    
    public override bool IsCompleted(GameState state) => _actionPoints <= 0;
    
    public void ConsumeAction(bool rolledSix)
    {
        if (!rolledSix) _actionPoints--;
        // Rolled 6 = bonus action, AP stays same
    }
}
```

### 4. Topology

```
RootNode (StartGameCommand)
    ↓
StartRoundNode ←→ TurnNode
    ↓ (when IsRoundComplete)
EndRoundNode
    ↓ (if winner) or ↺ (back to StartRound)
EndGameNode
```

### 5. Builder

```csharp
var fsm = FsmBuilder.Create()
    .WithRoot(startRound)
    .WithNode(turn)
    .WithNode(endRound)
    .WithNode(endGame)
    .Build();
```

