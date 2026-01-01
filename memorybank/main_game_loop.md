## Main Game Loop

```csharp
// 1. Setup
var fsm = new FsmGraph(rootNode, services, logger);
var state = fsm.Initialize(initialState);  // Executes root node System Action

// 2. Game Loop (called externally, e.g., from UI)
GameStateView UserLaunchCommand(ICommand command) 
{
    // Validate command allowed in current node
    if (!fsm.IsCommandAllowed(command.GetType())) 
        throw new InvalidOperationException("Command not allowed");
    
    // Execute command via Interactive Action
    var transaction = engine.ExecuteCommand(command);
    state = transaction.State;
    
    // Process FSM flow (auto-transitions + System Actions)
    var result = fsm.ProcessFlow(state);
    state = result.State;
    
    if (result.IsGameOver)
        return GameEnd(state);
        
    return state.ToView();
}
```

## FsmGraph.ProcessFlow internals

```csharp
while (currentNode.IsCompleted(state)) 
{
    var next = currentNode.GetNextNode(state);
    if (next == null) { return GameOver; }
    
    currentNode = next;
    // Execute System Actions (OnEntry logic)
    // These run automatically and can mutate state via overlay transactions
    ExecuteSystemAction(currentNode);  
}
// Node not completed -> waiting for user command (Interactive Action)
```

## Key Points
- **System Actions** execute automatically on node entry (inside ProcessFlow). replacing legacy Resolvers.
- **Interactive Actions** are triggered by user commands.
- **GetNextNode(state)** enables dynamic branching based on GameState.
- **IsCompleted** is a pure function over GameState.
- **IsGameOver** = GetNextNode returns null.