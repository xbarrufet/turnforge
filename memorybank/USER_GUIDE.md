# TurnForge User Guide

**A step-by-step guide for developers customizing TurnForge for their turn-based tactical game.**

---

## Table of Contents

1. [Installation](#1-installation)
2. [Entity Definition](#2-entity-definition)
3. [FSM (Finite State Machine)](#3-fsm-finite-state-machine)
4. [Command Flow and Results](#4-command-flow-and-results)
5. [Strategies](#5-strategies)
6. [Registering and Bootstrapping](#6-registering-and-bootstrapping)

---

## 1. Installation

### Prerequisites

- **.NET 8 or 9 SDK**
- **C# IDE** (Rider, Visual Studio, VS Code)
- **Basic understanding** of C#, LINQ, and object-oriented design

### Project Structure

TurnForge uses a modular architecture with two main projects:

```
YourGame/
├── YourGame.Rules/          # Game logic (no UI dependencies)
│   ├── Core/
│   │   ├── Domain/
│   │   │   ├── Entities/    # Custom entities
│   │   │   ├── Descriptors/ # Custom descriptors
│   │   │   └── Strategies/  # Game-specific strategies
│   │   └── Commands/        # Custom commands
│   └── Game/               # Game bootstrapping
└── YourGame.Godot/         # Godot integration (UI)
    └── Adapter/            # UI adapter
```

### Adding TurnForge to Your Project

**Option 1: NuGet Package** (when available)
```bash
dotnet add package TurnForge.Engine
```

**Option 2: Source Reference** (current)
```xml
<ProjectReference Include="path/to/TurnForge.Engine/TurnForge.Engine.csproj" />
```

### Quick Start

```csharp
using TurnForge.Engine.Core;
using TurnForge.Engine.Infrastructure;

// Create game context
var context = new GameEngineContext(
    repository: new InMemoryGameRepository(),
    propSpawnStrategy: new DefaultPropSpawnStrategy(),
    agentSpawnStrategy: new DefaultAgentSpawnStrategy(),
    logger: new ConsoleLogger()
);

// Build engine
var engine = GameEngineFactory.Build(context);

// Start using!
var result = engine.Runtime.ExecuteCommand(new InitializeBoardCommand(boardDescriptor));
```

---

## 2. Entity Definition

### Entity Hierarchy

TurnForge provides three base entity types:

```
GameEntity (abstract)
├── Agent       # Moving units (players, NPCs, enemies)
├── Prop        # Interactive objects (doors, chests, spawns)
└── Zone        # Board areas (tiles, regions)
```

### Creating Custom Entities

#### Step 1: Create Definition

Definitions contain **default data** loaded from files (JSON, databases, etc.).

```csharp
// File: YourGame.Rules/Core/Domain/Entities/Warrior.cs

public class WarriorDefinition : BaseGameEntityDefinition
{
    public int MaxHealth { get; set; } = 100;
    public int BaseAttack { get; set; } = 15;
    public string WeaponType { get; set; } = "Sword";
    
    public WarriorDefinition(string definitionId, string name, string category)
        : base(definitionId, name, category) { }
}
```

#### Step 2: Create Entity with Attributes

Entities are **runtime instances** with components.

```csharp
[DefinitionType(typeof(WarriorDefinition))]      // Links to definition
[DescriptorType(typeof(WarriorDescriptor))]      // Links to descriptor
public class Warrior : Agent
{
    public Warrior(EntityId id, string definitionId, string name, string category)
        : base(id, definitionId, name, category)
    {
        // Add game-specific components
        AddComponent(new CombatComponent());
        AddComponent(new InventoryComponent());
    }
}
```

> **Key Insight:** Attributes are on the **Entity**, not the Definition. This ensures compile-time safety and DRY principle.

#### Step 3: Create Descriptor (Optional)

Descriptors enable **type-safe spawn processing**.

```csharp
public class WarriorDescriptor : AgentDescriptor
{
    public string Faction { get; set; } = "Alliance";
    public int StartingLevel { get; set; } = 1;
    public List<string> EquippedItems { get; set; } = new();
    
    public WarriorDescriptor(string definitionId) : base(definitionId) { }
}
```

### Real Example: BarelyAlive Survivor

```csharp
// Definition - data from JSON
public class SurvivorDefinition : BaseGameEntityDefinition
{
    public int MaxHealth { get; set; } = 12;
    public string Faction { get; set; } = "Player";
    public int ActionPoints { get; set; } = 3;
}

// Entity - runtime instance
[DefinitionType(typeof(SurvivorDefinition))]
[DescriptorType(typeof(SurvivorDescriptor))]
public class Survivor : Agent
{
    public Survivor(EntityId id, string definitionId, string name, string category)
        : base(id, definitionId, name, category)
    {
        AddComponent(new FactionComponent());
    }
}

// Descriptor - spawn-time customization
public class SurvivorDescriptor : AgentDescriptor
{
    public string Faction { get; set; } = "Player";
    public int ActionPoints { get; set; } = 3;
}
```

### Using Spawn Builder

```csharp
var survivor = SpawnRequestBuilder
    .For("Survivors.Mike")
    .At(playerSpawn)
    .WithProperty("Faction", "Police")      // Maps to descriptor
    .WithProperty("ActionPoints", 4)
    .Build();

engine.ExecuteCommand(new SpawnAgentsCommand(new[] { survivor }));
```

---

## 3. FSM (Finite State Machine)

### Why FSM?

Turn-based games have **clear phases**: Setup → Player Turn → Enemy Turn → EndGame. FSM enforces valid transitions and prevents invalid states.

### Creating Your FSM

#### Step 1: Define States

```csharp
public enum GamePhase
{
    Menu,
    Setup,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}
```

#### Step 2: Define Transitions

```csharp
public enum GameTransition
{
    StartGame,
    BeginPlayerTurn,
    EndPlayerTurn,
    BeginEnemyTurn,
    EndEnemyTurn,
    PlayerWins,
    PlayerLoses,
    ReturnToMenu
}
```

#### Step 3: Create FSM Controller

```csharp
public static class YourGameFlow
{
    public static FsmControllerBuilder<GamePhase, GameTransition> CreateController()
    {
        var controller = new FsmControllerBuilder<GamePhase, GameTransition>()
            .WithInitialState(GamePhase.Menu);

        // Menu transitions
        controller
            .InState(GamePhase.Menu)
            .On(GameTransition.StartGame)
            .TransitionTo(GamePhase.Setup);

        // Setup → Player Turn
        controller
            .InState(GamePhase.Setup)
            .On(GameTransition.BeginPlayerTurn)
            .TransitionTo(GamePhase.PlayerTurn);

        // Player Turn ↔ Enemy Turn
        controller
            .InState(GamePhase.PlayerTurn)
            .On(GameTransition.EndPlayerTurn)
            .TransitionTo(GamePhase.EnemyTurn);

        controller
            .InState(GamePhase.EnemyTurn)
            .On(GameTransition.EndEnemyTurn)
            .TransitionTo(GamePhase.PlayerTurn);

        // Win/Lose conditions
        controller
            .InState(GamePhase.PlayerTurn)
            .On(GameTransition.PlayerWins)
            .TransitionTo(GamePhase.Victory);

        controller
            .InState(GamePhase.EnemyTurn)
            .On(GameTransition.PlayerLoses)
            .TransitionTo(GamePhase.Defeat);

        return controller;
    }
}
```

#### Step 4: Validate FSM

```csharp
var fsm = YourGameFlow.CreateController().Build();
fsm.ValidateFsm(); // Throws if invalid transitions exist
```

### Using FSM in Runtime

```csharp
// Set FSM controller
engine.Runtime.SetFsmController(YourGameFlow.CreateController());

// Query current state
var currentPhase = engine.Runtime.GetCurrentState<GamePhase>();

// Check valid transitions
var canEndTurn = engine.Runtime.CanTransition(GameTransition.EndPlayerTurn);

// Execute transition
engine.Runtime.ExecuteTransition(GameTransition.EndPlayerTurn);
```

### FSM + Commands

Commands can automatically trigger FSM transitions:

```csharp
public class EndTurnCommandHandler : ICommandHandler<EndTurnCommand>
{
    public CommandResponse Handle(EndTurnCommand command, GameState state)
    {
        // Game logic...
        
        // Trigger FSM transition
        return CommandResponse.Success()
            .WithFsmTransition(GameTransition.EndPlayerTurn);
    }
}
```

---

## 4. Command Flow and Results

### Command Architecture

```
User Input → Command → Handler → Decisions → Appliers → GameState
```

### Creating Custom Commands

#### Step 1: Define Command

```csharp
public record AttackCommand(
    EntityId AttackerId,
    EntityId TargetId,
    string WeaponId
) : ICommand;
```

#### Step 2: Create Handler

```csharp
public class AttackCommandHandler : ICommandHandler<AttackCommand>
{
    public CommandResponse Handle(AttackCommand command, GameState state)
    {
        // 1. Validate
        var attacker = state.GetAgent(command.AttackerId);
        var target = state.GetAgent(command.TargetId);
        
        if (attacker == null || target == null)
            return CommandResponse.Failure("Invalid entities");

        // 2. Calculate damage
        var damage = CalculateDamage(attacker, target, command.WeaponId);

        // 3. Create decision
        var decision = new DamageDecision(command.TargetId, damage);

        // 4. Return response
        return CommandResponse.Success()
            .WithDecisions(decision)
            .WithMessage($"{attacker.Name} attacks {target.Name} for {damage} damage!");
    }

    private int CalculateDamage(Agent attacker, Agent target, string weaponId)
    {
        // Your combat logic here
        return 10;
    }
}
```

#### Step 3: Create Decision

```csharp
public record DamageDecision(EntityId TargetId, int Damage) : IDecision;
```

#### Step 4: Create Applier

```csharp
public class DamageApplier : IApplier<DamageDecision>
{
    public ApplierResponse Apply(DamageDecision decision, GameState state)
    {
        var target = state.GetAgent(decision.TargetId);
        if (target == null)
            return ApplierResponse.Failure("Target not found");

        // Apply damage
        var healthComponent = target.GetComponent<IHealthComponent>();
        healthComponent.CurrentHealth -= decision.Damage;

        // Check if dead
        if (healthComponent.CurrentHealth <= 0)
        {
            state.RemoveAgent(decision.TargetId);
            return ApplierResponse.Success()
                .WithMessage($"{target.Name} has been defeated!");
        }

        return ApplierResponse.Success();
    }
}
```

### Executing Commands

```csharp
var command = new AttackCommand(
    AttackerId: warriorId,
    TargetId: enemyId,
    WeaponId: "Sword.Iron"
);

var result = engine.Runtime.ExecuteCommand(command);

if (result.Result.Success)
{
    Console.WriteLine(result.Result.Message);
    
    // Check transaction results
    foreach (var applierResult in result.TransactionResult.Results)
    {
        Console.WriteLine(applierResult.Message);
    }
}
```

### Command Response Properties

```csharp
public class CommandResponse
{
    public bool Success { get; }
    public string Message { get; }
    public string? Error { get; }
    public IReadOnlyList<IDecision> Decisions { get; }
    public GameTransition? FsmTransition { get; }
    
    // Factory methods
    public static CommandResponse Success();
    public static CommandResponse Failure(string error);
    
    // Fluent builders
    public CommandResponse WithMessage(string message);
    public CommandResponse WithDecisions(params IDecision[] decisions);
    public CommandResponse WithFsmTransition(GameTransition transition);
}
```

---

## 5. Strategies

### Spawn Strategies

Spawn strategies process **descriptors** before entity creation, enabling game-specific spawn logic.

#### Default Strategy (Simple)

```csharp
public class DefaultAgentSpawnStrategy : ISpawnStrategy<AgentDescriptor>
{
    public IReadOnlyList<AgentDescriptor> Process(
        IReadOnlyList<AgentDescriptor> descriptors,
        GameState state)
    {
        // Accept as-is
        return descriptors;
    }

    public IReadOnlyList<SpawnDecision<AgentDescriptor>> ToDecisions(
        IReadOnlyList<AgentDescriptor> descriptors)
    {
        return descriptors
            .Select(d => new SpawnDecision<AgentDescriptor>(d))
            .ToList();
    }
}
```

#### Hierarchical Strategy (Advanced)

For **type-specific processing**, extend `BaseSpawnStrategy`:

```csharp
public class YourGameSpawnStrategy : BaseSpawnStrategy
{
    // Level 1: Batch process enemies (distribute across spawn points)
    protected IReadOnlyList<EnemyDescriptor> ProcessBatch(
        IReadOnlyList<EnemyDescriptor> descriptors,
        GameState state)
    {
        var spawnPoints = state.GetProps()
            .Where(p => p.Category == "EnemySpawn")
            .ToList();

        for (int i = 0; i < descriptors.Count; i++)
        {
            descriptors[i].Position = spawnPoints[i % spawnPoints.Count]
                .PositionComponent.CurrentPosition;
        }

        return descriptors;
    }

    // Level 2: Individual warrior processing
    protected WarriorDescriptor ProcessSingle(
        WarriorDescriptor descriptor,
        GameState state)
    {
        // Assign to barracks
        var barracks = state.GetProps()
            .FirstOrDefault(p => p.DefinitionId == "Buildings.Barracks");

        if (barracks != null)
        {
            descriptor.Position = barracks.PositionComponent.CurrentPosition;
        }

        // Set default faction
        descriptor.Faction ??= "Alliance";

        return descriptor;
    }

    // Level 3: Fallback (inherited from BaseSpawnStrategy)
}
```

> **See Also:** [ENTIDADES.md - Hierarchical Spawn Strategy](file:///Users/barrufex/Development/TurnForge/memorybank/ENTIDADES.md#hierarchical-spawn-strategy-advanced) for detailed documentation.

### Other Strategy Types

TurnForge supports custom strategies for:
- **Movement** - Pathfinding, terrain costs
- **Vision** - Line of sight, fog of war
- **AI** - Enemy behavior, decision trees
- **Combat** - Damage calculation, hit chance

---

## 6. Registering and Bootstrapping

### Project Setup

#### Step 1: Create Game Context

```csharp
// File: YourGame.Rules/Game/YourGame.cs

public class YourGame
{
    private readonly TurnForge _turnForge;
    
    public YourGame(IGameLogger? logger = null)
    {
        var safeLogger = logger ?? new ConsoleLogger();
        
        // Create repository
        var repository = new InMemoryGameRepository();
        
        // Create strategies
        var propStrategy = new YourPropSpawnStrategy();
        var agentStrategy = new YourAgentSpawnStrategy(); // Your hierarchical strategy
        
        // Build context
        var context = new GameEngineContext(
            repository,
            propStrategy,
            agentStrategy,
            safeLogger
        );
        
        // Build engine
        _turnForge = GameEngineFactory.Build(context);
        
        // Set FSM
        var fsmController = YourGameFlow.CreateController();
        _turnForge.Runtime.SetFsmController(fsmController);
        
        // Register definitions
        RegisterDefinitions();
    }
    
    private void RegisterDefinitions()
    {
        // Register entity definitions
        _turnForge.GameCatalog.RegisterDefinition(
            new WarriorDefinition("Warriors.Knight", "Knight", "Unit"));
        
        _turnForge.GameCatalog.RegisterDefinition(
            new WarriorDefinition("Warriors.Archer", "Archer", "Unit"));
        
        // ... more definitions
    }
    
    public ITurnForgeRuntime Runtime => _turnForge.Runtime;
    public IGameCatalogApi GameCatalog => _turnForge.GameCatalog;
}
```

#### Step 2: Create Static Factory

```csharp
public static class YourGameFactory
{
    public static YourGame CreateNewGame(IGameLogger? logger = null)
    {
        return new YourGame(logger);
    }
}
```

#### Step 3: Initialize in Your App

```csharp
// Console app
var game = YourGameFactory.CreateNewGame();

// Godot integration
public partial class GameManager : Node
{
    private YourGame _game;
    
    public override void _Ready()
    {
        _game = YourGameFactory.CreateNewGame(new GodotLogger());
    }
}
```

### Registering Custom Handlers

```csharp
// In GameEngineFactory or custom registry
serviceProvider.RegisterHandler<AttackCommand, AttackCommandHandler>();
serviceProvider.RegisterApplier<DamageDecision, DamageApplier>();
```

### Loading Definitions from JSON

```csharp
public class DefinitionLoader
{
    public void LoadFromJson(IGameCatalogApi catalog, string jsonPath)
    {
        var json = File.ReadAllText(jsonPath);
        var dtos = JsonSerializer.Deserialize<List<WarriorDto>>(json);
        
        foreach (var dto in dtos)
        {
            var definition = new WarriorDefinition(
                dto.Id,
                dto.Name,
                "Unit"
            )
            {
                MaxHealth = dto.MaxHealth,
                BaseAttack = dto.BaseAttack
            };
            
            catalog.RegisterDefinition(definition);
        }
    }
}
```

### Complete Bootstrap Example

```csharp
// Program.cs or GameManager.cs

var game = YourGameFactory.CreateNewGame();

// Load definitions from files
var loader = new DefinitionLoader();
loader.LoadFromJson(game.GameCatalog, "Data/warriors.json");
loader.LoadFromJson(game.GameCatalog, "Data/enemies.json");

// Load mission/board
var missionData = File.ReadAllText("Data/mission01.json");
var (spatial, zones, props, agents) = MissionLoader.ParseMissionString(missionData);
var boardDescriptor = new BoardDescriptor(spatial, zones);

// Initialize game
var initResult = game.Runtime.ExecuteCommand(new InitializeBoardCommand(boardDescriptor));
if (!initResult.Result.Success)
{
    Console.WriteLine($"Failed to initialize: {initResult.Result.Error}");
    return;
}

// Spawn props
foreach (var propDesc in props)
{
    game.Runtime.ExecuteCommand(new SpawnPropsCommand(new[] { propDesc }));
}

// Start game
game.Runtime.ExecuteTransition(GameTransition.StartGame);

// Game loop
while (game.Runtime.GetCurrentState<GamePhase>() != GamePhase.Victory)
{
    // Your game logic
    ProcessPlayerInput(game);
    ProcessEnemyAI(game);
}
```

---

## Best Practices

### 1. Separation of Concerns
- **Rules project:** Pure C#, no UI dependencies
- **Godot/UI project:** Only UI logic, delegates to Rules

### 2. Compile-Time Safety
- Use `[DefinitionType]` and `[DescriptorType]` attributes
- Leverage EntityTypeRegistry for type lookups
- Prefer typed strategies over reflection

### 3. Testing
- Unit test handlers and appliers independently
- Integration test command flow end-to-end
- Mock GameState for isolated tests

### 4. Performance
- Cache expensive calculations
- Use readonly collections
- Avoid LINQ in hot paths

### 5. Documentation
- Document custom entities with XML comments
- Explain FSM transitions clearly
- Provide usage examples in code

---

## Next Steps

1. **Explore Examples:** Review [`BarelyAlive.Rules`](file:///Users/barrufex/Development/TurnForge/src/BarelyAlive.Rules) for a complete reference implementation
2. **Read Technical Docs:** [`ENTIDADES.md`](file:///Users/barrufex/Development/TurnForge/memorybank/ENTIDADES.md) for architecture details
3. **Join Community:** Ask questions, share your game!

---

## Quick Reference

### Common Commands
- `InitializeBoardCommand` - Setup spatial board
- `SpawnAgentsCommand` - Spawn units
- `SpawnPropsCommand` - Spawn objects
- `MoveAgentCommand` - Move units
- `CommandAck` - Confirm transactions

### Common Interfaces
- `ICommand` - Command definition
- `ICommandHandler<T>` - Command processing
- `IDecision` - State change intent
- `IApplier<T>` - State modification
- `ISpawnStrategy<T>` - Spawn processing

### Key Classes
- `GameEngineFactory` - Engine builder
- `GameEngineContext` - Dependencies container
- `FsmControllerBuilder` - FSM creator
- `SpawnRequestBuilder` - Fluent spawn API
- `EntityTypeRegistry` - Type lookup

---

**Happy game development with TurnForge!** 🎮
