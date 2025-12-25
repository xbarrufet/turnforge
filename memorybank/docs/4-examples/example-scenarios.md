# Example Scenarios

Walkthroughs of common gameplay flows.

## Scenario 1: Setup and Spawn

**Goal:** Initialize a board and spawn two teams.

```csharp
// 1. Initialize Board
var boardDesc = new BoardDescriptor(
    Spatial: new SpatialDescriptor(Tiles, Connections),
    Zones: new[] { new ZoneDescriptor("SpawnZone") }
);
engine.ExecuteCommand(new InitializeBoardCommand(boardDesc));

// 2. Spawn Survivors (Team A)
var survivorReq = SpawnRequestBuilder.For("Survivors.Rick")
    .At(new Position(0, 0))
    .Build();
engine.ExecuteCommand(new SpawnAgentsCommand([survivorReq]));

// 3. Spawn Zombies (Team B)
var zombieReq = SpawnRequestBuilder.For("Zombies.Walker")
    .At(new Position(5, 5))
    .Build();
engine.ExecuteCommand(new SpawnAgentsCommand([zombieReq]));
```

## Scenario 2: Simple Turn Loop

**Goal:** Execute a player turn followed by an AI turn.

```csharp
// FSM defined as: PlayerTurnNode -> EnemyTurnNode -> (Loop)

// 1. Player Turn
// User selects unit and moves
var moveCmd = new MoveCommand(
    AgentId: "agent-rick",
    TargetPosition: new Position(0, 1),
    HasCost: true
);
engine.ExecuteCommand(moveCmd); // Generates effects (Move animation)

// Player ends turn
engine.ExecuteCommand(new EndTurnCommand());

// 2. Transition
// FSM auto-navigates to EnemyTurnNode

// 3. Enemy Turn (AI)
// AI System calculates move
var aiMove = new MoveCommand(
    AgentId: "agent-walker",
    TargetPosition: new Position(5, 4),
    HasCost: true
);
engine.ExecuteCommand(aiMove);

// AI ends turn
engine.ExecuteCommand(new EndTurnCommand());

// Loop back to PlayerTurnNode
```
