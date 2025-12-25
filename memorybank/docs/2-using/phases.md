# Configuring Game Phases (FSM)

The Finite State Machine (FSM) defines the structure of your game loop.

## The GameFlowBuilder

Use the builder to define your tree of states.

```csharp
var builder = new GameFlowBuilder();

builder.AddRoot<SystemRootNode>("Root", root => 
{
    // 1. Setup Phase (PassNode - auto advances)
    root.AddLeaf<SetupNode>("Setup");

    // 2. Main Game Loop (BranchNode with loop)
    root.AddBranch("GameLoop", loop => 
    {
        // Player's Turn (LeafNode - waits for input)
        loop.AddLeaf<PlayerTurnNode>("PlayerInput");
        
        // Enemy Turn (LeafNode - waits for AI/System)
        loop.AddLeaf<EnemyTurnNode>("EnemyAI");
        
    }).SetLoop(true); // Iterate children forever
});
```

## Node Types

### LeafNode (Interactive)
Stops execution and waits for a command. Use this for player input or specific AI steps.
- **Must** define allowed commands.

```csharp
public class PlayerTurnNode : LeafNode
{
    public PlayerTurnNode()
    {
        AddAllowedCommand<MoveCommand>();
        AddAllowedCommand<EndTurnCommand>();
    }
}
```

### PassNode (Automatic)
Executes `OnStart`, applies decisions, and immediately moves to the next node.
- Use for setup, cleanup, or instant transitions.

### BranchNode (Structural)
Groups other nodes. Can track iteration counts.

## State Transitions

Transitions happen:
1. When a `LeafNode` receives a command that returns a `ChangeStateDecision` (manual).
2. When a `PassNode` completes its work (automatic).
3. When a branch loops.
