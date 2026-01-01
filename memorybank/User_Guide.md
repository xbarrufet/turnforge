# TurnForge Engine User Guide

## Overview
TurnForge is a generic engine for turn-based games. This guide explains how to use the public API to build and run games.

---

## Core API: IGameEngine

### ExecuteAction
This is the **primary API** for triggering game actions from UI.

```csharp
ActionTransaction ExecuteAction(ActionId actionId, Dictionary<string, object>? parameters = null);
```

**Example:**
```csharp
var result = engine.ExecuteAction(ParchisActions.Move, new Dictionary<string, object>
{
    { "Roll", 5 },
    { "PlayerId", PlayerId.From("RED") }
});

if (result.Status == ActionStatus.Completed)
{
    // Process events for animations
    foreach (var evt in result.Events) { /* animate */ }
}
```

---

## Configuration & Initialization

TurnForge provides a high-level **Fluent API** to configure and initialize the engine without needing to manually instantiate internal components.

### 3. Initialize Engine (Cartridge Flow)
The recommended way to start a game is to build an "empty" engine (loaded with definitions) and then execute the built-in `StartGame` action.

```csharp
// 1. Prepare Definitions (Board, Mission, Entity Definitions)
var boardDef = ParchisBoardFactory.CreateDescriptor();
var missionDef = ParchisMissionFactory.CreateMissionForPlayers(playerColors);
var definitions = new List<BaseGameEntityDefinition> { boardDef, missionDef };

// 2. Build Engine (StartGame is pre-registered)
var engine = GameEngineFactory.Create(fsmRootNode)
    .WithDefinitions(definitions)
    .Build();

// 3. Start Game - Single Call Mode
var transaction = engine.ExecuteAction(new ActionId("StartGame"), new Dictionary<string, object>
{
    { "BoardId", boardDef.DefinitionId },
    { "MissionId", missionDef.DefinitionId },
    { "PlayerIds", new List<string> { "P1", "P2" } }
});
```

### 3.1 Interactive Mode (Optional)
For games with lobby/setup UI, use interactive inputs:

```csharp
// Step 1: Add Players
engine.ProvideInput(new AddPlayerInput(playerId, "Player 1", agentDescriptors));

// Step 2: Select Map/Mission
engine.ProvideInput(new SelectMapInput("map_1", boardDef, missionDef));

// Step 3: Confirm
engine.ProvideInput(new ConfirmPlayersInput());
```

### 4. Legacy Customization (Obsolete)
Previously, you could set specific components directly during factory build. This is now deprecated in favor of the action-driven approach.
```csharp
// DEPRECATED
.WithBoardDefinition(board)
.WithInitialEntities(entities)
```

### TurnForge Facade

The `TurnForge` object returned by `Build()` acts as a convenient facade for the most common operations:

```csharp
// Execute Action
turnForge.ExecuteAction(MyActions.Move, parameters);

// Execute Command
turnForge.ExecuteCommand(new MyCommand());

// Check Status
var status = turnForge.GetStatus(); // WaitingForStart, InProgress, GameOver

// Reset Game
turnForge.ResetGame(); // Clears state, resets FSM
```

---

## Action Registration

Games must register their actions during bootstrap:

```csharp
// Define action IDs
public static class ParchisActions
{
    public static readonly ActionId Move = new("parchis_move");
}

// Register with engine
public static void Register(IActionRegistry registry)
{
    registry.Register(ParchisActions.Move, ParchisMoveActionFactory.Create);
}
```

---

## ActionTransaction Result

| Property | Type | Description |
|----------|------|-------------|
| `ActionId` | `ActionId` | ID of executed action |
| `Status` | `ActionStatus` | Completed, Suspended, Failed |
| `Events` | `IReadOnlyList<IGameEvent>` | Events for UI (moves, spawns) |
| `ErrorMessage` | `string?` | Error details if failed |
| `IsGameOver` | `bool` | True if game ended |

---

## Game State Access

Use `GameStateView` to query game state (respects pending changes):

```csharp
var view = new GameStateView(state, overlay);

// Generic queries
var entity = view.GetEntity(entityId);
var position = view.GetPosition(entityId);

// With game-specific extensions (Parchis example)
using Parchis.Rules.Extensions;

var pawns = view.GetPawns(playerId);
var pawnsInSpawn = view.GetPawnsInSpawn(playerId, "red");
bool occupied = view.IsTileOccupied(tileId);
```

---

## Typical Game Loop (UI Perspective)

```csharp
// 1. Setup
ParchisActionRegistration.Register(engine.ActionRegistry);

// 2. Game loop
while (!gameOver)
{
    var roll = RollDice();
    
    var result = engine.ExecuteAction(ParchisActions.Move, new()
    {
        { "Roll", roll },
        { "PlayerId", currentPlayer }
    });
    
    // 3. Animate events
    foreach (var evt in result.Events)
    {
        await AnimateEvent(evt);
    }
    
    // 4. Check game over
    if (result.IsGameOver)
    {
        gameOver = true;
    }
}
```

---

## Available Actions (Parchis)

| Action ID | Parameters | Description |
|-------------|------------|-------------|
| `parchis_move` | Roll, PlayerId | Execute move with dice result |

---

## Creating Custom Actions

```csharp
public static IAction Create()
{
    var node1 = new MyFirstNode();
    var node2 = new MySecondNode();
    
    node1.SetNextNode(node2);
    
    return ActionBuilder.Create("my_action")
        .AddNode(node1)
        .AddNode(node2)
        .Build();
}
```

Each node implements:
```csharp
public override ActionStepResult Execute(ActionContext context)
{
    // Read parameters
    var value = context.Get<int>("MyParam");
    
    // Record state changes
    context.Overlay.Record(new MoveOperation(entityId, newPosition));
    
    // Return result
    return ActionStepResult.Success();
}
```
