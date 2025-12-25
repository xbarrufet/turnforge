# Getting Started

This guide walks you through setting up and initializing the TurnForge engine for your game.

## 1. Installation

TurnForge is distributed as a NuGet package/project reference. Add `TurnForge.Engine` to your solution.

## 2. Initialization Flow

To start the game, you need to construct the `GameEngineRuntime` and initialize the state.

### Step 1: Define Your Game Loop (FSM)

Create a state machine structure using the `GameFlowBuilder`.

```csharp
var builder = new GameFlowBuilder();

var root = builder.AddRoot<SystemRootNode>("Root", r => {
    // Phase 1: Setup
    r.AddLeaf<SetupNode>("Setup");
    
    // Phase 2: Game Loop
    r.AddBranch("GameLoop", l => {
        l.AddLeaf<PlayerTurnNode>("PlayerTurn");
        l.AddLeaf<EnemyTurnNode>("EnemyTurn");
    }).SetLoop(true); // Loop back to start of branch
}).Build();
```

### Step 2: Initialize FSM Controller

```csharp
var controller = new FsmController(root, root.Id);

// IMPORTANT: FSM auto-navigates to the first LeafNode.
// We must sync our repository to match this initial state.
var repository = new InMemoryGameRepository(); // Or your implementation
var initialState = repository.LoadGameState()
    .WithCurrentStateId(controller.CurrentStateId);

repository.SaveGameState(initialState);
```

### Step 3: Configure Services & Engine

Register all necessary services and appliers using the `ServiceCollection` or your DI container of choice (TurnForge uses a simple internal registry pattern).

```csharp
var gameCatalog = new InMemoryGameCatalog();
var factory = new GenericActorFactory(gameCatalog);
var orchestrator = new TurnForgeOrchestrator(repository);

// Register Appliers
orchestrator.RegisterApplier(new AgentSpawnApplier(factory));
orchestrator.RegisterApplier(new MoveApplier());
// ... register others

// Create Runtime
var engine = new GameEngineRuntime(
    orchestrator,
    controller,
    new CommandBus(serviceProvider),
    repository
);
```

## 3. Starting the Game

Once initialized, the engine is ready to receive commands.

```csharp
// Example: Send setup commands
engine.ExecuteCommand(new InitializeBoardCommand(boardDescriptor));
engine.ExecuteCommand(new SpawnAgentsCommand(spawnRequests));

// Verify state
var currentState = repository.LoadGameState();
Console.WriteLine($"Current Phase: {currentState.CurrentStateId}");
```
