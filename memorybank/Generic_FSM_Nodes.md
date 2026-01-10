# Generic FSM Nodes & Core Actions

## Overview

TurnForge provides generic, reusable FSM nodes and core actions that handle common turn-based game patterns. These components can be used out-of-the-box or extended for game-specific behavior.

---

## Generic FSM Nodes

### NextPlayerEndRoundNode

**Purpose**: Automatically advances to the next player when ending a round.

**Extends**: `EndRoundNode`

**Behavior**: 
- Executes `MoveToNextPlayerEndTurnAction` on entry
- Advances turn order to next player
- Transitions to StartRound or EndGame based on game state

**Usage**:
```csharp
var endRound = new NextPlayerEndRoundNode()
    .WithStartRound(startRoundNode)
    .WithEndGame(endGameNode);
```

**When to use**:
- Simple turn-based games where players take turns sequentially
- Games without complex end-of-round logic
- When you want automatic turn advancement without manual configuration

**Example (Parchis)**:
```csharp
var fsmGraph = new FsmGraph()
    .WithStartGame(new StartGameNode()
        .WithStartRound(startRound))
    .WithStartRound(startRound)
    .WithTurn(turnNode)
    .WithEndRound(new NextPlayerEndRoundNode()  // Auto-advances to next player
        .WithStartRound(startRound)
        .WithEndGame(endGame))
    .WithEndGame(endGame);
```

---

### ChekEndTurnAndResetApStartRoundNode

**Purpose**: Checks if turn has ended and resets action points at the start of each round.

**Extends**: `StartRoundNode`

**Behavior**:
- Executes `NextTurnResetAction` on entry
- Checks `GameStateView.IsEndTurn` flag
- If turn ended, resets all players' action points via `NextTurnResetApOperation`
- Transitions to Turn node

**Usage**:
```csharp
var startRound = new ChekEndTurnAndResetApStartRoundNode()
    .WithTurnNode(turnNode);
```

**When to use**:
- Games with action point systems
- Games where players' resources reset each round
- When you need automatic AP management

**Example**:
```csharp
var startRound = new ChekEndTurnAndResetApStartRoundNode()
    .WithTurnNode(turnNode);

var fsmGraph = new FsmGraph()
    .WithStartRound(startRound)
    .WithTurn(turnNode)
    .WithEndRound(endRound);
```

---

## Core Actions

### MoveToNextPlayerEndTurnAction

**Action ID**: `"Core.EndTurn"`

**Purpose**: Advances the turn order to the next player.

**Parameters**: None (system action)

**Behavior**:
1. Gets current turn order from `GameStateView`
2. Calls `TurnOrder.NextPlayer()` to advance
3. Records `SetTurnOrderOperation` to update state

**Node**: `SetNextPlayerInTurn`

**Usage**:
```csharp
// Automatically used by NextPlayerEndRoundNode
var endRound = new NextPlayerEndRoundNode();

// Or manually in custom nodes
public override void OnEnter(GameStateView state)
{
    var action = MoveToNextPlayerEndTurnAction.Create();
    // Execute action...
}
```

**Implementation**:
```csharp
public class SetNextPlayerInTurn : LinkableNode
{
    public override ActionStepResult Execute(ActionContext context, GameStateView state)
    {
        state.RecordOperation(new SetTurnOrderOperation(state.TurnOrder.NextPlayer()));
        return ActionStepResult.Success();
    }
}
```

---

### NextTurnResetAction

**Action ID**: `"Core.NextTurnResetAction"`

**Purpose**: Resets action points for all players when a turn ends.

**Parameters**: None (system action)

**Behavior**:
1. Checks `GameStateView.IsEndTurn` flag
2. If true, records `NextTurnResetApOperation`
3. Operation resets all players' action points to their maximum

**Node**: `NextTurnResetActionNode`

**Usage**:
```csharp
// Automatically used by ChekEndTurnAndResetApStartRoundNode
var startRound = new ChekEndTurnAndResetApStartRoundNode();

// Or manually in custom nodes
public override void OnEnter(GameStateView state)
{
    var action = NextTurnResetAction.Create();
    // Execute action...
}
```

**Implementation**:
```csharp
public class NextTurnResetActionNode : LinkableNode
{
    public override ActionStepResult Execute(ActionContext context, GameStateView state)
    {
        if(state.IsEndTurn) 
            state.RecordOperation(new NextTurnResetApOperation());
        return ActionStepResult.Success();
    }
}
```

---

## Combining Generic Nodes

### Example: Simple Turn-Based Game

```csharp
// Create nodes
var startGame = new StartGameNode();
var startRound = new ChekEndTurnAndResetApStartRoundNode();
var turn = new MyGameTurnNode();
var endRound = new NextPlayerEndRoundNode();
var endGame = new EndGameNode();

// Wire up FSM
var fsmGraph = new FsmGraph()
    .WithStartGame(startGame.WithStartRound(startRound))
    .WithStartRound(startRound.WithTurnNode(turn))
    .WithTurn(turn)
    .WithEndRound(endRound
        .WithStartRound(startRound)
        .WithEndGame(endGame))
    .WithEndGame(endGame);

// Build engine
var engine = GameEngineFactory.Create(fsmGraph)
    .WithDefinitions(definitions)
    .Build();
```

This setup provides:
- ✅ Automatic turn advancement
- ✅ Automatic AP reset each round
- ✅ Proper game flow (Start → Round → Turn → EndRound → ...)

---

## Custom Extensions

### Extending Generic Nodes

You can extend these nodes for game-specific behavior:

```csharp
public class MyCustomEndRoundNode : NextPlayerEndRoundNode
{
    public MyCustomEndRoundNode() : base()
    {
        // Add additional OnEntry actions
        OnEntry(MyCustomAction.Create());
    }
    
    public override void OnEnter(GameStateView state)
    {
        // Custom logic before advancing player
        LogRoundEnd(state);
        
        base.OnEnter(state);
    }
}
```

### Creating Similar Patterns

Use the same pattern for other common behaviors:

```csharp
public class ResetResourcesStartRoundNode : StartRoundNode
{
    public ResetResourcesStartRoundNode() : base()
    {
        OnEntry(ResetResourcesAction.Create());
    }
}

public class DrawCardsStartRoundNode : StartRoundNode
{
    public DrawCardsStartRoundNode() : base()
    {
        OnEntry(DrawCardsAction.Create());
    }
}
```

---

## Best Practices

### 1. Use Generic Nodes for Standard Patterns

```csharp
// ✅ Good - Use generic nodes for common patterns
var endRound = new NextPlayerEndRoundNode()
    .WithStartRound(startRound);

// ❌ Avoid - Don't reimplement standard behavior
var endRound = new EndRoundNode();
endRound.OnEntry(MoveToNextPlayerEndTurnAction.Create()); // Redundant
```

### 2. Extend When Needed

```csharp
// ✅ Good - Extend for additional behavior
public class ParchisEndRoundNode : NextPlayerEndRoundNode
{
    public ParchisEndRoundNode() : base()
    {
        OnEntry(CheckWinConditionAction.Create());
    }
}

// ❌ Avoid - Don't create from scratch if generic exists
public class ParchisEndRoundNode : EndRoundNode
{
    public ParchisEndRoundNode() : base()
    {
        OnEntry(MoveToNextPlayerEndTurnAction.Create());
        OnEntry(CheckWinConditionAction.Create());
    }
}
```

### 3. Compose Actions

```csharp
// ✅ Good - Combine multiple actions
var startRound = new ChekEndTurnAndResetApStartRoundNode();
startRound.OnEntry(DrawCardsAction.Create());
startRound.OnEntry(RefreshAbilitiesAction.Create());
```

---

## Available Generic Components

### FSM Nodes

| Node | Base | Auto Action | Purpose |
|------|------|-------------|---------|
| `NextPlayerEndRoundNode` | `EndRoundNode` | `MoveToNextPlayerEndTurnAction` | Auto-advance to next player |
| `ChekEndTurnAndResetApStartRoundNode` | `StartRoundNode` | `NextTurnResetAction` | Auto-reset AP each round |

### Core Actions

| Action | Node | Purpose |
|--------|------|---------|
| `MoveToNextPlayerEndTurnAction` | `SetNextPlayerInTurn` | Advance turn order |
| `NextTurnResetAction` | `NextTurnResetActionNode` | Reset action points |

### Operations

| Operation | Purpose |
|-----------|---------|
| `SetTurnOrderOperation` | Update turn order state |
| `NextTurnResetApOperation` | Reset all players' AP |

---

## See Also

- [FSM 2.0](FSM_2.0.md) - FSM architecture and custom nodes
- [Actions Catalog](Actions_Catalog.md) - Creating custom actions
- [GameState Transactions](GameState_Transactions.md) - Operations and overlay pattern
