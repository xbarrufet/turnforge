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
*   **OnEntry Workflows**: System workflows that execute automatically on node entry (preferred).
*   **Resolvers** [Deprecated]: Legacy resolver calls, use OnEntry workflows instead.

### System Workflows (OnEntry)

System workflows execute automatically when entering a node. Unlike interactive workflows, they don't suspend for user input.

```csharp
var endRoundNode = new FsmNode("EndRound")
    .OnEntry(new ResetActionPointsWorkflow())      // Executes first
    .OnEntry(new EvaluateSpawnRulesWorkflow())     // Executes second
    .WithCompletionCondition(_ => true);
```

**Workflow Types:**
| Type | Input | Example |
|------|-------|---------|
| **Interactive** | Waits for user | StartGame, SelectTarget |
| **System** | Automatic | Spawn, ResetAP, DrawCards |

**Execution Order on Node Entry:**
1. OnEntry Workflows execute (in order)
2. Legacy Resolvers execute (for backward compatibility)
3. Check completion condition

### System Workflows & Overlay Transaction

System workflows use the same transactional overlay mechanism as interactive workflows, but **complete immediately** without suspending.

#### How FsmGraph Executes System Workflows

```csharp
FsmGraph.ExecuteNodeEntry()
├─ For each OnEntryWorkflow:
│  ├─ Create SystemWorkflowContext(currentState)
│  ├─ WorkflowOrchestrator.StartWorkflow(workflow, context)
│  │  ├─ InitializeState() → Creates GameStateOverlay
│  │  ├─ Execute all nodes (use context.Overlay)
│  │  └─ Commit overlay → New GameState
│  └─ Update FsmGraph state with committed result
└─ Execute legacy Resolvers (if any)
```

#### Key Differences: System vs Interactive

| Aspect | System Workflows | Interactive Workflows |
|--------|-----------------|----------------------|
| **Suspension** | Never suspends | May suspend for input |
| **Completion** | Immediate | Asynchronous |
| **Overlay** | Created, used, committed in one call | May span multiple resume cycles |
| **Context** | `SystemWorkflowContext` | Custom `WorkflowContext` |

#### Example: Spawn System Workflow

```csharp
public class EvaluateSpawnRulesWorkflow : IWorkflow
{
    public WorkflowStepResult Execute(WorkflowContext context)
    {
        // Get state from context
        var state = context.State;
        
        // Use context's overlay (shared across workflow)
        var view = new GameStateView(state, context.Overlay);
        
        // Evaluate spawn rules and record operations
        spawnOrchestrator.ExecuteSpawns(view, context.Overlay);
        
        return WorkflowStepResult.Success();
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

### 3 Topology
The topology of the tree is the following

RootNode (StartGameCommand)
|
v
StartRoundNode (no commands) until games is over
|
v
TurnNodes (no commands) until all players have played
|
v
EndRoundNode (no commands) until all rounds are over
|
v
EndGameNode (no commands) until game is over

### 4 Builder
Builder().WithRound(new Round<RoundNode>().withTurnNode<TurNode>()..withTurnNode<TurNode>().withEndRoundNode<EndRoundNode>().withEndGameNode<EndGameNode>())
