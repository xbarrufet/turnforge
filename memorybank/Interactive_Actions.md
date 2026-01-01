# Interactive Actions (Opportunity Window Pattern)

TurnForge uses a robust action engine to handle complex, multi-step game logic that requires user interaction (e.g., selecting a target, confirming an action, rolling dice).

This system is built around the **Opportunity Window Pattern**: logic runs until it needs input, then it **suspends** execution and opens a typed "window" for external interaction.

## Core Concepts

### 1. Core Actions vs Game Actions

TurnForge distinguishes between two types of actions:

| Type | Provider | Registration | Examples |
|------|----------|--------------|----------|
| **Core Actions** | TurnForge Engine | Auto-registered | `StartGame`, `Spawn` |
| **Game Actions** | Game rules | Manual registration | `Move`, `Attack` |

**Core Actions** are provided by the engine and automatically registered. Use them via `CoreActions`:

```csharp
using TurnForge.Engine.Core.Actions;

// Start a game (core action - no registration needed)
engine.ExecuteAction(CoreActions.StartGame, parameters);
```

**Game Actions** are defined by your game rules and registered in your game's `ActionRegistration`:

```csharp
// Game-specific actions (must be registered)
public static class ParchisActions
{
    public static readonly ActionId Move = new("parchis_move");
}

// Register in your game's bootstrap
registry.Register(ParchisActions.Move, ParchisMoveActionFactory.Create);
```

From the caller's perspective, both types are invoked identically via `ExecuteAction()`.

### 2. Action vs FSM
- **FSM (Finite State Machine)**: Controls the high-level game flow (Whose turn is it? What phase involved?). It defines *allowed commands*.
- **Action**: Encapsulates a specific process (e.g., "Attack Sequence", "Start Game Setup"). When a action is running, it **takes over** the engine's focus.

### 3. The Interaction Loop

1.  **Execute Node**: The engine runs a action node (e.g., `SelectTargetNode`).
2.  **Suspend**: If the node needs input, it returns `ActionStatus.Suspended` with a list of `AllowedInputTypes`.
3.  **Wait**: The engine pauses the action and waits for a command.
4.  **Submit Input**: The UI/Client sends a `ActionInputCommand` with the required `IActionInput`.
5.  **Resume**: The Orchestrator injects the input into the action context and re-runs the node logic.

## Architecture Components

### `IActionInput`
A marker interface for typed data payloads sent from the client to the engine.

```csharp
public record SelectCellInput(int X, int Y) : IActionInput;
public record ConfirmActionInput() : IActionInput;
```

### `InteractionNode<TContext>`
The base class for interactive nodes. It abstracts the suspend/resume loop.

```csharp
public class MyNode : InteractionNode<MyContext>
{
    protected override (string Reason, Type[] AllowedInputs) GetRequiredInteractions(MyContext context)
    {
        // Tell the UI we are waiting for a cell selection
        return ("Select a target cell", new[] { typeof(SelectCellInput) });
    }

    protected override void ProcessNewInputs(MyContext context)
    {
        // This runs when the engine "wakes up" with new input
        if (context.HasInput<SelectCellInput>())
        {
            var input = context.ConsumeInput<SelectCellInput>();
            context.TargetCell = input.Cell;
        }
    }

    protected override bool IsReadyToComplete(MyContext context)
    {
        return context.TargetCell != null;
    }
}
```

### `ActionContext`
The shared memory for a action instance. It holds:
- **Game State**: A read/write view of the game state (transactional).
- **Input Queue**: Inputs buffered for the current node.
- **Custom Data**: Action-specific data (e.g., `TargetCell`, `DamageCalculated`).

### `ActionOrchestrator`
Manages the lifecycle of actions.
- `StartAction(action, context)`: Initiates a new session.
- `SubmitInput(actionId, input)`: Resumes a suspended session.
- `ExecuteAction(actionId)`: Runs the logic loop until completion or suspension.

## Integration with Game Engine

The `GameEngineRuntime` acts as the gatekeeper:

- **Interception**: If a action is `Suspended`, the engine **blocks** normal game commands (e.g., "EndTurn") and only accepts `ActionInputCommand`.
- **Focus**: This ensures the player completes the active sequence (or cancels it) before doing anything else.

## Example: Start Game Action

1.  **Start**: FSM triggers `StartGameAction`.
2.  **Node 1 (Players)**: Suspends. UI shows "Add Players".
    - User sends `AddPlayerInput("P1")` -> Node processes -> Suspends again.
    - User sends `ConfirmPlayersInput()` -> Node completes.
3.  **Node 2 (Map)**: Suspends. UI shows "Select Map".
    - User sends `SelectMapInput("Map_01")` -> Node completes.
4.  **Node 3 (Build)**: Runs synchronously, creates the GameState, and finishes.
5.  **End**: Action completes, FSM resumes control.

## State-Overlay-Action Transaction

Actions operate on **GameState** using a transactional overlay pattern to ensure atomicity and consistency.

### The Transaction Lifecycle

```
ActionOrchestrator.StartAction()
├─ 1. InitializeState(baseState) → Creates GameStateOverlay
├─ 2. Execute nodes (all use context.Overlay)
│  ├─ Node 1 records operations to overlay
│  ├─ Node 2 records more operations (same overlay)
│  └─ Node N continues using shared overlay
└─ 3. Action completes
   ├─ Success → overlay.Commit(baseState) → New GameState
   ├─ Suspend → Keep overlay (resume will continue)
   └─ Fail → Discard overlay (rollback)
```

### Key Components

| Component | Responsibility |
|-----------|----------------|
| **GameState** | Immutable game state snapshot |
| **GameStateOverlay** | Mutable transaction log of operations |
| **ActionContext** | Holds both State + Overlay |
| **ActionOrchestrator** | Manages overlay lifecycle (create/commit) |

### How Nodes Use Overlay

```csharp
public ActionStepResult Execute(ActionContext context)
{
    // 1. Read current state
    var state = context.State;
    
    // 2. Create view with overlay for reading "projected" state
    var view = new GameStateView(state, context.Overlay);
    
    // 3. Record operations to the overlay
    var moveOp = new MoveOperation(entityId, newPosition);
    context.Overlay.Record(moveOp);
    
    // 4. Continue to next node (overlay persists!)
    return ActionStepResult.Success();
    
    // NOTE: Do NOT commit here! Orchestrator does it at the end.
}
```

### Why This Matters

**Atomicity**: All operations in a action succeed or fail together. If a action suspends or fails, the overlay is discarded—no partial state mutations.

**Consistency**: `GameStateView` shows the "projected" state (base + overlay), so nodes see pending changes from previous nodes in the same action.

**Isolation**: Each action has its own overlay. Multiple concurrent actions don't interfere.

**Multi-Node Operations**: All nodes share the same overlay, building up a complete transaction that commits at the end.

### Example: Attack Action

```csharp
// Node 1: Select Target (records selection)
context.Overlay.Record(new SelectionOperation(targetId));

// Node 2: Calculate Damage (uses view to see selection)
var view = new GameStateView(context.State, context.Overlay);
var target = view.GetEntity(targetId); // Sees selection from Node 1!
context.Overlay.Record(new DamageOperation(targetId, damage));

// Node 3: Apply Effects
context.Overlay.Record(new EffectOperation(...));

// Orchestrator commits ALL operations when action completes
var newState = context.Overlay.Commit(context.State);
```

## Batch Input Preloading

The action system supports **preloading all inputs** before starting or submitting. This allows the client to control the flow and skip interactive steps when all data is already known.

### How It Works

Inputs are stored in a **queue** inside `ActionContext`. Each node consumes only the inputs it needs, leaving the rest for subsequent nodes.

```csharp
// Pre-load all inputs BEFORE starting the action
context.EnqueueInput(new AddPlayerInput("Player1"));
context.EnqueueInput(new AddPlayerInput("Player2"));
context.EnqueueInput(new ConfirmPlayersInput());
context.EnqueueInput(new SelectMapInput("Map_01"));

// Start action - it will complete immediately without suspending
orchestrator.StartAction(startGameAction, context);

// Status: Completed (no interactive pauses!)
```

### Use Cases

| Scenario | Benefit |
|----------|---------|
| **Automated tests** | Run full actions without mocking UI interactions |
| **AI players** | Pre-calculate all moves and submit at once |
| **Replay/Undo** | Reconstruct game state by replaying all inputs |
| **Batch initialization** | Start game with all players and map in one call |

### Key Points

- Inputs are consumed **in order** (FIFO queue)
- Each node only takes what it needs; remaining inputs carry forward
- Works with both `EnqueueInput()` (direct) and `SubmitInput()` (via orchestrator)
- If a node needs an input not in the queue, it suspends as usual
