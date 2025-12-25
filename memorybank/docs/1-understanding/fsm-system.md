# FSM (Finite State Machine)

The FSM controls game flow through phases (setup, player turn, enemy turn, etc.).

## Node Types

| Type | Nature | Can Handle Commands | Behavior | Example |
|------|--------|---------------------|----------|---------|
| **BranchNode** | Structural | ❌ No | Grouping only. Passes control to children. | `SystemRootNode` |
| **PassNode** | Background | ❌ No | Executes logic, applies decisions, auto-advances. | `SetupNode` |
| **LeafNode** | Interactive | ✅ Yes | Stops recursion. Waiting for user/command input. | `PlayerTurnNode` |

**Rule:** Recursion continues through `BranchNode` and `PassNode` until a `LeafNode` (or a `LaunchCommand` result) is reached.

## Recursive Navigation & Phases

The FSM engine uses a **Recursive Navigation** system (v2.0).

**NodePhaseResult:**
Nodes return a standardized result from `OnStart`/`OnEnd` events:

```csharp
public struct NodePhaseResult
{
    public PhaseResult Result;                 // Pass, LaunchCommand, ApplyDecisions
    public ICommand? Command;                  // If LaunchCommand
    public IEnumerable<IFsmApplier>? Decisions; // Effects to apply immediately
}
```

1. **Pass**: Engine moves to the next sibling immediately.
2. **ApplyDecisions**: Engine applies effects to state, then moves to next sibling.
3. **LaunchCommand**: recursion STOPS, and the Engine executes the returned Command (as if it came from User). Matches `StopLeafNode` behavior but automatic.

**Loop Guard:**
The controller acts as a circuit breaker, detecting infinite loops (e.g. A -> B -> A) and terminating execution to prevent freezes.

## Auto-Navigation

> **CRITICAL:** `FsmController` auto-navigates from `BranchNode` to first `LeafNode` on initialization.

**Rationale:** BranchNodes can't handle commands. FSM must always start at a command-ready state.

```csharp
public FsmController(FsmNode root, NodeId initialId)
{
    _currentStateId = initialId;
    
    // Auto-navigate if initialId is a BranchNode
    if (_nodesById.TryGetValue(initialId, out var node) && node is BranchNode branch)
    {
        var leaf = FindFirstLeaf(branch); // Depth-first search
        if (leaf != null) _currentStateId = leaf.Id;
    }
}
```

**Consequence:** Tests/code initializing with `SystemRootNode` will actually start at `InitialStateNode` (first leaf).

## State Synchronization

FSM maintains state in **two places**:
1. `FsmController._currentStateId` (in-memory)
2. `GameState.CurrentStateId` (persistent)

**Must sync after initialization:**
```csharp
var controller = new FsmController(root, rootId);
var state = repository.LoadGameState()
    .WithCurrentStateId(controller.CurrentStateId);
repository.SaveGameState(state);
```

## System Nodes (Initialization Flow)

```
InitialStateNode → BoardReadyNode → GamePreparedNode → [Game Phases]
```

| Node | Allowed Command | Transition Tag | Purpose |
|------|-----------------|----------------|---------|
| `InitialStateNode` | `InitializeBoardCommand` | `"BoardInitialized"` | Setup spatial model |
| `BoardReadyNode` | `SpawnPropsCommand` | `"PropsSpawned"` | Place static props |
| `GamePreparedNode` | `SpawnAgentsCommand` | `"AgentsSpawned"` | Spawn dynamic agents |

**Order Matters:**
1. Board must exist before placing entities
2. Props (spawn points) must exist before agents
3. Agents require props for spawn positioning
