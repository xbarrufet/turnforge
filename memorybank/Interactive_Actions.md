# Interactive Actions (Opportunity Window Pattern)

TurnForge uses a robust action engine to handle complex, multi-step game logic that requires user interaction (e.g., selecting a target, confirming an action, rolling dice).

This system is built around the **Opportunity Window Pattern**: logic runs until it needs input, then it **suspends** execution and opens a typed "window" for external interaction.

## Core Concepts

### 1. Core Actions vs Game Actions

TurnForge distinguishes between two types of actions:

| Type | Provider | Registration | Examples |
|------|----------|--------------|----------|
| **Core Actions** | TurnForge Engine | Auto-registered | `StartGame`, `Spawn` |
| **Game Actions** | Game rules | Factory Implementation | `Move`, `Attack` |

**Core Actions** are provided by the engine.

**Game Actions** are defined by your game rules and provided via an implementation of `IActionFactory`.

```csharp
// 1. Define Action IDs
public static class ParchisActions
{
    public static readonly ActionId Move = new("parchis_move");
}

// 2. Implement Factory
public class ParchisActionFactory : IActionFactory
{
    public IAction BuildAction(ActionId actionId)
    {
        if (actionId == ParchisActions.Move)
        {
            return ParchisMoveAction.Create();
        }
        throw new NotImplementedException($"Action {actionId} not implemented.");
    }

    public IReadOnlyList<ActionId> GetRegisteredActionIds()
    {
        return new List<ActionId> { ParchisActions.Move };
    }
}

// 3. Register in Engine
GameEngineFactory.Create(rootNode)
    .WithActionFactory(new ParchisActionFactory())
    .Build();
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
├─ 1. Creates GameStateView (wraps State)
├─ 2. Execute nodes
│  ├─ Node 1 calls state.RecordOperation(op1) → Overlay in GameState
│  ├─ Node 2 calls state.RecordOperation(op2)
│  └─ Node N sees updated state via GameStateView
└─ 3. Action completes
   ├─ Success → state.CommitOverlayChanges() → New GameState persistence
   ├─ Suspend → Keep overlay (resume will continue)
   └─ Fail → Discard overlay (rollback)
```

### Key Components

| Component | Responsibility |
|-----------|----------------|
| **GameState** | Source of truth. Manages `GameStateOverlay` internally. |
| **GameStateView** | Read/Write view passed to nodes. Delegates recording to `GameState`. |
| **ActionContext** | Holds **workflow data** (inputs, variables) only. No state. |
| **ActionOrchestrator** | Manages execution flow and triggers final commit. |

### How Nodes Use Overlay

Nodes interact with the state via `GameStateView`. They do not access `ActionContext.Overlay` anymore.

```csharp
public ActionStepResult Execute(ActionContext context, GameStateView state)
{
    // 1. Read projected state (includes previous operations)
    var entity = state.GetEntity(entityId); 
    
    // 2. Record new operations
    var moveOp = new MoveOperation(entityId, newPosition);
    state.RecordOperation(moveOp);
    
    // 3. Continue to next node
    return ActionStepResult.Success();
    
    // NOTE: Do NOT commit here! Orchestrator does it at the end.
}
```

### Typed Contexts & Action Creation

Actions often need to store temporary data specific to their workflow (e.g., dice rolls, selected targets). You should define a custom `ActionContext`.

#### 1. Define Context
```csharp
public class MoveActionContext : ActionContext 
{
    // Workflow-scoped data, not game state
    public int RollResult { get; set; }
    public bool HasBounced { get; set; }
}
```

#### 2. Create Action with Typed Context
Use `.WithContext()` in the `ActionBuilder` to factory the correct context type.

```csharp
public static IAction Create()
{
    return ActionBuilder.Create("MoveAction")
        .WithContext(() => new MoveActionContext()) // <--- Creates specific context
        .AddNode(new RuleOfFiveNode())
        .AddNode(new SelectPawnNode())
        .Build();
}
```

### 3. Use in Nodes
Use the helper method `GetTypedContext<T>` to safely access your custom properties.

```csharp
public override ActionStepResult Execute(ActionContext context, GameStateView state)
{
    // Safe cast with error handling
    var ctx = GetTypedContext<MoveActionContext>(context);
    
    // Read/Write workflow data
    if (ctx.RollResult == 0) return ActionStepResult.Failed("No roll");
    
    ctx.HasBounced = true;
    
    return ActionStepResult.Success();
}

### 4. Parameter Injection from Start

You can pass initial parameters when starting an action using `ExecuteAction(id, parameters)`. To support this, your context properties must be backed by the underlying `ActionContext` data store.

#### Implementing Injectable Properties
Refactor your context properties to use `Get/Set` (or `TryGet`) methods. This maps the dictionary keys from the injection to your strongly-typed properties.

```csharp
public class MyContext : ActionContext
{
    // This property will automatically receive value from parameters["TargetId"]
    public string TargetId 
    {
        get => Get<string>("TargetId");
        set => Set("TargetId", value);
    }
}
```

#### Passing Parameters
Pass a dictionary matching the property names:

```csharp
var params = new Dictionary<string, object> 
{
    { "TargetId", "Entity_123" }
};
engine.ExecuteAction(MyActionId, params);
```
```

## Semantic API via Extensions

Directly using `GameStateView` can lead to verbose and low-level code. It is highly recommended to create **Extension Methods** for your specific game rules to provide a clean, semantic API.

### 1. Create Extensions Class

```csharp
public static class ParchisViewExtensions
{
    // Retrieve typed entities directly
    public static IEnumerable<Actor> GetPawns(this GameStateView view, PlayerId owner)
        => view.GetEntitiesForOwner(owner).OfType<Actor>();

    // Semantic queries
    public static bool IsSafeTile(this GameStateView view, TileId tile)
    {
        return tile.Value == "center" || tile.Value.EndsWith("_entry");
    }
}
```

### 2. Use in Nodes

The node code becomes much more readable and "speaks" the language of the game design.

```csharp
public override ActionStepResult Execute(ActionContext context, GameStateView state)
{
    // BEFORE (Generic)
    var entities = state.GetEntitiesForOwner(pid);
    var pawns = entities.OfType<Actor>();
    
    // AFTER (Semantic)
    var pawns = state.GetPawns(pid);
    
    if (state.IsSafeTile(targetTile))
    {
        // ...
    }
}
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
