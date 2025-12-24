# TurnForge.Engine Reference

**Technical documentation for the TurnForge game engine. Target audience: developers and AI agents.**

*It is a work in progress and is not yet ready for production use.*
---

## Table of Contents
0. [Context](#overview)
1. [Architecture Overview](#architecture-overview)
2. [Entity System](#entity-system)
3. [FSM (Finite State Machine)](#fsm-finite-state-machine)
4. [Command & Decision Flow](#command--decision-flow)
5. [Orchestrator](#orchestrator)
6. [Commands](#commands)
7. [Spatial & Board](#spatial--board)
8. [Integration Patterns](#integration-patterns)

---

## Context

This document is a reference for the TurnForge game engine. It is intended to be used by developers and AI agents to understand the architecture of the engine and how to use it.
Turnforge is a turn-based tactical game engine. Its based in a set of componentns to manage the flow and entitenes of a turn based game implementation.
- FSM: Finite State Machine to manage the turn flow
- Entities: Expandable entities with components to manage the game state. Each component is focus in a especifc data domain (Health, Position, etc). 
- Commands: Actions that can be executed by the player or AI agents
- Strategies: Configurable strategies to customize the behaviour of the commands 

## Architecture Overview

TurnForge.Engine implements a **Command-Decision-Applier** pattern with FSM-controlled state transitions.

### Core Principles

| Principle | Implementation |
|-----------|----------------|
| **Single Mutation Point** | Only `Orchestrator` mutates `GameState` |
| **Immutable State** | All state changes create new instances |
| **Ordered Execution** | Commands → Decisions → Appliers → Effects |
| **Phase Control** | FSM validates commands per game phase |
| **Decoupled Logic** | Handlers generate decisions, Appliers execute them |

### Data Flow

```
Command → Validation → Handler → Decision → Scheduler → Applier → State + Effects
```

**Key Classes:**
- `GameEngineRuntime`: Entry point, orchestrates all components
- `FsmController`: State machine, validates commands, manages transitions
- `TurnForgeOrchestrator`: Routes decisions to appliers, mutates state
- `CommandBus`: Dispatches commands to handlers
- `GameState`: Immutable game state snapshot

---

## Entity System

### Entity Hierarchy

```
GameEntity (abstract)
├── Actor (has position, health)
│   ├── Agent (can take actions)
│   └── Prop (static environmental object)
└── Item (pickupable, no position) [NOT IMPLEMENTED]
```

### Components

Components are atomic pieces of entity data. All entity mutations operate at component level.

**Base Components:**
- `BehaviourComponent`: Dynamic behaviors (e.g., "Fly", "Swim")
- `PositionComponent`: Tile position on board
- `HealthComponent`: Current/max health
- `MovementComponent`: Movement points (Agents only)
- `ActionTakeComponent`: Action points (Agents only)

**Component Example:**
```csharp
public interface IHealthComponent : IGameEntityComponent
{
    int MaxHealth { get; set; }
    int CurrentHealth { get; set; }
}
```

### Definition-Descriptor Pattern (Auto-Mapping)

**Definition** = Blueprint (Data Source)
```csharp
[EntityType(typeof(Survivor))]
public class SurvivorDefinition : AgentDefinition
{
    // FT-004: Implicit Mapping
    // Matches properties in IHealthComponent by name/type automatically.
    public int MaxHealth { get; set; } = 10;
    
    // Explicit mapping for different names
    [MapToComponent(typeof(IFactionComponent), "FactionName")]
    public string Team { get; set; } = "Survivors";
}
```

**Descriptor** = Spawn Request (Overrides)
```csharp
public class SurvivorDescriptor : AgentDescriptor
{
    public SurvivorDescriptor(string definitionId) : base(definitionId) { }
    
    // Automatically maps to IPositionComponent.CurrentPosition if matched
    // Or used by Strategy/Factory logic
    public Position Position { get; set; }
}
```

**Property Discovery (FT-004):**
1. **Implicit (Default):** Properties on Definition/Descriptor map to Component properties with same name and type.
2. **Explicit (`[MapToComponent]`):** Redirects mapping to specific specific component/property.
3. **Opt-Out (`[DoNotMap]`):** Attributes on Component properties prevent external setting.


**Factory Process:**
1. Handler creates `Descriptor` from command
2. `SpawnStrategy` validates and builds `SpawnDecision<Descriptor>`
3. `SpawnApplier` fetches `Definition`, creates entity, maps properties
4. Result: Entity in `GameState` with merged Definition + Descriptor data

### Component Lookup

Supports lookup by interface or concrete type:

```csharp
entity.AddComponent(new FactionComponent()); // Register as concrete

// Lookup by interface (auto-resolved)
var faction = entity.GetComponent<IFactionComponent>(); // ✅ Works
```

**Implementation:** Fallback search if direct key not found.

---

## FSM (Finite State Machine)

The FSM controls game flow through phases (setup, player turn, enemy turn, etc.).


### Node Types

| Type | Nature | Can Handle Commands | Behavior | Example |
|------|--------|---------------------|----------|---------|
| **BranchNode** | Structural | ❌ No | Grouping only. Passes control to children. | `SystemRootNode` |
| **PassNode** | Background | ❌ No | Executes logic, applies decisions, auto-advances. | `SetupNode` |
| **LeafNode** | Interactive | ✅ Yes | Stops recursion. Waiting for user/command input. | `PlayerTurnNode` |

**Rule:** Recursion continues through `BranchNode` and `PassNode` until a `LeafNode` (or a `LaunchCommand` result) is reached.

### Recursive Navigation & Phases

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

### Auto-Navigation

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

### State Synchronization

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

### System Nodes (Initialization Flow)

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

---

## Command & Decision Flow

### Execution Steps

1. **Command Validation** - FSM checks command allowed in current state
2. **Handler Execution** - Handler generates `IDecision[]` (NO state mutation)
3. **Decision Scheduling** - Decisions enqueued to `Scheduler`
4. **OnCommandExecutionEnd** - Immediate decisions execute
5. **Transition Check** - FSM checks if state should advance
6. **OnStateEnd** - Cleanup decisions for old state
7. **Transition** - FSM moves to new state
8. **OnStateStart** - Init decisions for new state
9. **ACK Wait** - Engine awaits UI acknowledgment

### Command Validation

```csharp
// FSM checks if command allowed
if (!_fsmController.CurrentState.IsCommandAllowed(command.GetType()))
    throw new Exception($"Command {command} not allowed in state {CurrentState}");
```

**Example:** `MoveCommand` only allowed during `PlayerTurnNode`, not during `SetupNode`.

### Decision Timing

Decisions execute at specific moments:

```csharp
public enum DecisionTimingWhen
{
    OnStateStart,            // Entering a phase
    OnStateEnd,              // Leaving a phase
    OnCommandExecutionEnd    // Immediately after command
}
```

**Example:**
```csharp
var decision = new SpawnDecision<AgentDescriptor>(descriptor)
{
    Timing = DecisionTiming.Immediate // Execute now
};

var recurringDecision = new DrawCardDecision()
{
    Timing = new DecisionTiming(
        When: DecisionTimingWhen.OnStateStart,
        Phase: "PlayerTurn",
        Frequency: DecisionTimingFrequency.Permanent // Every turn
    )
};
```

### ACK System

After each command, engine sets `WaitingForACK = true`. No commands accepted until `CommandAck` received.

**Purpose:** Allows UI to animate effects before accepting next input.

---

## Orchestrator

**The ONLY place where `GameState` mutates.**

### Applier Registration

```csharp
orchestrator.RegisterApplier(new AgentSpawnApplier(factory));
orchestrator.RegisterApplier(new MoveApplier());
orchestrator.RegisterApplier(new AttackApplier());
```

### Decision Execution

```csharp
public IGameEffect[] Apply(IDecision decision)
{
    // Find registered applier by decision type
    var applier = _appliers[decision.GetType()];
    
    // Execute (dynamic dispatch)
    var response = ((dynamic)applier).Apply((dynamic)decision, CurrentState);
    
    // Update state (ONLY mutation point)
    CurrentState = response.GameState;
    
    return response.GameEffects;
}
```

### Applier Pattern

```csharp
public interface IApplier<TDecision> where TDecision : IDecision
{
    ApplierResponse Apply(TDecision decision, GameState state);
}

public record ApplierResponse(GameState GameState, IGameEffect[] GameEffects);
```

**Rules:**
- ✅ Receives immutable state
- ✅ Returns new state (never mutates input)
- ✅ Generates effects for UI/logging
- ❌ No business logic (that's in handlers/strategies)

**Example:**
```csharp
public class MoveApplier : IApplier<MoveDecision>
{
    public ApplierResponse Apply(MoveDecision decision, GameState state)
    {
        var agent = state.Agents[decision.AgentId];
        var updatedAgent = agent.WithPosition(decision.To);
        var newState = state.WithAgent(updatedAgent);
        
        var effect = new AgentMovedEffect(decision.AgentId, decision.From, decision.To);
        return new ApplierResponse(newState, [effect]);
    }
}
```

### Scheduler

Manages delayed/recurring decisions:

```csharp
// Enqueue decisions
CurrentState = CurrentState.WithScheduler(Scheduler.Add(decisions));

// Execute scheduled decisions matching phase/timing
var toExecute = Scheduler.GetDecisions(d => 
    d.Timing.Phase == phase && d.Timing.When == when);

foreach (var decision in toExecute)
{
    var effects = Apply(decision);
    
    // Remove if single-use
    if (decision.Timing.Frequency == Single)
        Scheduler.Remove(decision);
}
```

---
## COMMANDS

Commands are the primary way to interact with the TurnForge engine. They represent user or AI intentions and are validated, processed, and executed through the Command-Decision-Applier pipeline.

### Command Structure

```csharp
public interface ICommand { Type CommandType { get; } }
```

All commands must:
- Be immutable (use `record` types)
- Contain all necessary data
- Not perform business logic (that's in handlers)

---

### Spawn Command

The Spawn Command system handles entity creation with a flexible, type-safe pipeline.

**Pipeline Flow:**
```
SpawnRequest → DescriptorBuilder → Descriptor → Strategy → Decision → Applier → Entity
```

**Key Principle:** Entities are **ONLY** created via Spawn. Updates are done via Components.

---

#### SpawnRequest (User Input)

The `SpawnRequest` is the user-facing API for requesting entity spawns.

```csharp
public sealed record SpawnRequest(
    string DefinitionId,  // Required
    int Count = 1,        // Optional: batch spawn
    Position? Position = null,  // Optional: strategy decides if null
    Dictionary<string, object>? PropertyOverrides = null,
    IEnumerable<IGameEntityComponent>? ExtraComponents = null
);
```

**Usage (Direct):**
```csharp
var request = new SpawnRequest(
    "Survivors.Mike",
    Position: spawnPoint,
    PropertyOverrides: new() { ["Health"] = 12 }
);
```

**Usage (Fluent Builder - Recommended):**
```csharp
var request = SpawnRequestBuilder
    .For("Survivors.Mike")
    .At(spawnPoint)
    .WithProperty("Health", 12)
    .WithProperty("Faction", "Police")
    .Build();
```

---

#### SpawnRequestBuilder (Fluent API)

 **Purpose:** Improve developer experience with IntelliSense-driven API.

**Features:**
- ✅ Fluent method chaining
- ✅ Type-safe generic overloads  
- ✅ Implicit conversion to `SpawnRequest`
- ✅ Validation on build

**Methods:**
- `For(string definitionId)` - Entry point
- `At(Position position)` - Set spawn position
- `WithCount(int count)` - Set batch count
- `WithProperty(string key, object value)` - Add property override
- `WithProperty<T>(string key, T value)` - Type-safe override
- `WithComponent<T>(T component)` - Add extra component
- `WithComponents(params IGameEntityComponent[])` - Add multiple
- `Build()` - Create final `SpawnRequest`

**Example - Complex Boss:**
```csharp
// Before (verbose)
var boss = new SpawnRequest(
    "Enemies.DragonBoss",
    Position: bossSpawn,
    PropertyOverrides: new Dictionary<string, object> {
        ["Health"] = 1000,
        ["PhaseCount"] = 5
    }
);

// After (fluent, discoverable)
var boss = SpawnRequestBuilder
    .For("Enemies.DragonBoss")
    .At(bossSpawn)
    .WithProperty("Health", 1000)
    .WithProperty("PhaseCount", 5)
    .Build();
```

**Example - Batch with LINQ:**
```csharp
var zombies = Enumerable.Range(0, 10)
    .Select(i => SpawnRequestBuilder
        .For("Enemies.Zombie")
        .At(spawnPoints[i])
        .WithProperty("Health", 10 + (i * 5))
        .Build())
    .ToList();
```

---

#### DescriptorBuilder (Preprocessor)

**Purpose:** Convert `SpawnRequest` to typed descriptors automatically.

**Process:**
1. Create descriptor instance (respects `[DescriptorType]` attribute)
2. Apply property overrides via reflection
3. Copy position and extra components

```csharp
// Automatic mapping: Dictionary → Typed Properties
var descriptor = DescriptorBuilder.Build<AgentDescriptor>(request, definition);
// PropertyOverrides["Health"] → descriptor.Health
```

---

#### Descriptors (Internal Type Safety)

Descriptors are **internal transformation artifacts**.

```csharp
public class GameEntityBuildDescriptor : IGameEntityBuildDescriptor
{
    public string DefinitionID { get; set; }
    public List<IGameEntityComponent> ExtraComponents { get; init; } = new();
    public Position? Position { get; set; }
}

public class AgentDescriptor : GameEntityBuildDescriptor { }
public class PropDescriptor : GameEntityBuildDescriptor { }
```

**Key Point:** Users do **not** define custom descriptors. Used internally for type safety.

---

#### Spawn Strategy

**Purpose:** Process descriptors and determine spawn conditions.

```csharp
public interface ISpawnStrategy<TDescriptor> 
{
    IReadOnlyList<TDescriptor> Process(
        IReadOnlyList<TDescriptor> descriptors,
        GameState state);
        
    IReadOnlyList<SpawnDecision<TDescriptor>> ToDecisions(
        IReadOnlyList<TDescriptor> descriptors);
}
```

**Responsibilities:**
- Assign spawn positions (if not provided)
- Validate spawn conditions
- Filter invalid spawns
- Modify descriptors based on game state

**Example:**
```csharp
public class ConfigurableAgentSpawnStrategy : ISpawnStrategy<AgentDescriptor>
{
    public IReadOnlyList<AgentDescriptor> Process(
        IReadOnlyList<AgentDescriptor> descriptors,
        GameState state)
    {
        foreach (var descriptor in descriptors)
        {
            var spawnPoint = FindSpawnPoint(descriptor.DefinitionID, state);
            descriptor.Position = spawnPoint;
        }
        return descriptors;
    }
}
```

---

#### Spawn Commands

```csharp
public sealed record SpawnAgentsCommand(
    IReadOnlyList<SpawnRequest> Requests
) : ICommand;

public sealed record SpawnPropsCommand(
    IReadOnlyList<SpawnRequest> Requests
) : ICommand;
```

---

#### Command Handler

```csharp
public class SpawnAgentsCommandHandler : ICommandHandler<SpawnAgentsCommand>
{
    public CommandResult Handle(SpawnAgentsCommand command)
    {
        //  1. Preprocessor: Request → Descriptor
        var descriptors = BuildDescriptors(command.Requests);
        
        // 2. Strategy: Process/Filter
        var processed = _strategy.Process(descriptors, gameState);
        
        // 3. ToDecisions: Wrap in decisions
        var decisions = _strategy.ToDecisions(processed);
        
        // 4. Return to FSM (applier will create entities)
        return CommandResult.Ok(decisions.Cast<IDecision>().ToArray());
    }
}
```

---

#### Spawn Applier

```csharp
public class AgentSpawnApplier : ISpawnApplier<AgentDescriptor>
{
    public ApplierResponse Apply(SpawnDecision<AgentDescriptor> decision, GameState state)
    {
        var agent = _factory.BuildAgent(decision.Descriptor);
        var newState = state.WithAgent(agent);
        var effect = new EntitySpawnedEffect(agent.Id, agent.DefinitionId, descriptor.Position);
        
        return ApplierResponse.Ok(newState, effect);
    }
}
```

---

#### Complete Spawn Flow Example

```csharp
// 1. Create request using fluent builder
var request = SpawnRequestBuilder
    .For("Enemies.DragonBoss")
    .At(bossSpawn)
    .WithProperty("Health", 1000)
    .Build();

// 2. Execute command
var result = engine.ExecuteCommand(new SpawnAgentsCommand(new[] { request }));

/*
INTERNAL FLOW:
3. CommandHandler → DescriptorBuilder.Build<AgentDescriptor>()
4. Maps PropertyOverrides: descriptor.Health = 1000
5. Strategy.Process() validates/modifies
6. Strategy.ToDecisions() wraps in SpawnDecision
7. Applier.Apply() creates entity via factory
8. Entity added to GameState
*/
```

---

#### Design Principles

**1. User Simplicity**
- Users only interact with `SpawnRequest` (or fluent builder)
- No custom descriptors needed
- Dictionary provides flexibility

**2. Engine Type Safety**
- Descriptors provide typed processing
- Custom descriptors via `[DescriptorType]`
- Reflection-based mapping

**3. Separation of Concerns**
- Request = User input
- Descriptor = Engine artifact
- Strategy = Business logic
- Applier = Entity creation

---

### Hierarchical Spawn Strategy (Advanced)

For game-specific spawn logic, TurnForge provides a hierarchical strategy system with compile-time type safety.

---

#### Entity Type Registry

**Purpose:** Runtime type lookup for Definition→Entity→Descriptor relationships.

**Initialization:**
```csharp
// Automatic initialization when GenericActorFactory is created
EntityTypeRegistry.Initialize();

// Scans all loaded assemblies for entities with [DefinitionType] attribute
```

**Lookup Methods:**
```csharp
// Get entity type from definition type
Type? entityType = EntityTypeRegistry.GetEntityType(typeof(SurvivorDefinition));
// Returns: typeof(Survivor)

// Get descriptor type from entity type
Type? descriptorType = EntityTypeRegistry.GetDescriptorType(typeof(Survivor));
// Returns: typeof(SurvivorDescriptor)

// Get complete chain
var (entityType, descriptorType) = EntityTypeRegistry.GetTypeChain(typeof(SurvivorDefinition));
```

---

#### Entity Type Attributes

**Single Source of Truth:** All type relationships are declared on the **Entity** class.

##### DefinitionTypeAttribute

Links Entity → Definition (compile-time verified).

```csharp
[DefinitionType(typeof(SurvivorDefinition))]  // ✅ Compile error if missing
public class Survivor : Agent
{
    // ...
}
```

**Benefits:**
- Compile-time safety (missing Definition type = build error)
- Automatic registry population
- No redundant attributes on Definition classes

##### DescriptorTypeAttribute

Links Entity → Descriptor for type-specific spawn processing.

```csharp
[DescriptorType(typeof(SurvivorDescriptor))]  // ✅ Compile error if missing
public class Survivor : Agent
{
    // ...
}
```

**Complete Example:**
```csharp
// Entity - Owns ALL type relationships
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

// Definition - Clean, no attributes needed
public class SurvivorDefinition : BaseGameEntityDefinition
{
    public string Faction { get; set; } = "Player";
    public int MaxHealth { get; set; } = 12;
    
    public SurvivorDefinition(string definitionId, string name, string category)
        : base(definitionId, name, category) { }
}

// Descriptor - Custom properties for spawn-time processing
public class SurvivorDescriptor : AgentDescriptor
{
    public string Faction { get; set; } = "Player";
    public int ActionPoints { get; set; } = 3;
    
    // ⚠️ REQUIRED: Single-parameter constructor for DescriptorBuilder reflection
    public SurvivorDescriptor(string definitionId) : base(definitionId) { }
}
```

> [!IMPORTANT]
> **Custom Descriptor Constructor Requirement**
> 
> All custom descriptors **MUST** provide a single-parameter constructor `(string definitionId)` for compatibility with `DescriptorBuilder`'s reflection-based instantiation.
> 
> **Why?** `DescriptorBuilder.CreateDescriptor()` uses `Activator.CreateInstance(descriptorType, definitionId)` to instantiate descriptors dynamically.
> 
> **Pattern:**
> ```csharp
> public class CustomDescriptor : PropDescriptor
> {
>     public Color Color { get; set; }
>     
>     // ✅ Required: Default constructor for DescriptorBuilder
>     public CustomDescriptor(string definitionId) : base(definitionId)
>     {
>         Color = Color.Red; // Default value
>     }
>     
>     // ✅ Optional: Additional constructors for explicit usage
>     public CustomDescriptor(string definitionId, Color color) : base(definitionId)
>     {
>         Color = color;
>     }
> }
> ```
> 
> **Error if missing:** `Constructor on type 'YourDescriptor' not found.`

```

---

#### BaseSpawnStrategy (3-Level Hierarchy)

**Purpose:** Type-specific spawn processing with hierarchical method resolution.

**Processing Levels (first match wins):**

1. **ProcessBatch<T>(List<T>, GameState)** - Batch processing for same type
2. **ProcessSingle(T, GameState)** - Individual type-specific processing
3. **ProcessSingle<T>(T, GameState)** - Default fallback (accepts as-is)

**Method Resolution:**
- Uses reflection to find matching methods
- Results cached for performance
- Excludes base class methods to avoid false positives

##### Example Implementation

```csharp
public class BarelyAliveSpawnStrategy : BaseSpawnStrategy
{
    // Level 1: Batch process zombie spawns (distribute across spawn points)
    protected IReadOnlyList<ZombieSpawnDescriptor> ProcessBatch(
        IReadOnlyList<ZombieSpawnDescriptor> descriptors,
        GameState state)
    {
        // Find all zombie spawn props
        var zombieSpawns = state.GetProps()
            .OfType<ZombieSpawn>()
            .Select(p => p.PositionComponent?.CurrentPosition)
            .Where(pos => pos != null && pos != Position.Empty)
            .ToList();

        if (zombieSpawns.Count == 0) return descriptors;

        // Distribute zombies across available spawn points (round-robin)
        for (int i = 0; i < descriptors.Count; i++)
        {
            if (descriptors[i].Position == null || descriptors[i].Position == Position.Empty)
            {
                descriptors[i].Position = zombieSpawns[i % zombieSpawns.Count];
            }
        }

        return descriptors;
    }

    // Level 2: Individual process survivors (assign to player spawn)
    protected SurvivorDescriptor ProcessSingle(
        SurvivorDescriptor descriptor,
        GameState state)
    {
        // Find player spawn point
        var playerSpawn = state.GetProps()
            .FirstOrDefault(p => p.Category == "Spawn" && p.DefinitionId.Contains("Player"));

        if (playerSpawn != null && playerSpawn.PositionComponent != null)
        {
            descriptor.Position = playerSpawn.PositionComponent.CurrentPosition;
        }

        // Ensure faction is set
        if (string.IsNullOrEmpty(descriptor.Faction))
        {
            descriptor.Faction = "Player";
        }

        return descriptor;
    }

    // Level 3: Default fallback (inherited from BaseSpawnStrategy)
    // Accepts all other descriptor types as-is
}
```

##### Usage Example

```csharp
// 1. Create spawn requests using fluent builder
var survivors = new[]
{
    SpawnRequestBuilder.For("Survivors.Mike").At(playerSpawn).Build(),
    SpawnRequestBuilder.For("Survivors.Doug").At(playerSpawn).Build()
};

var zombies = Enumerable.Range(0, 5)
    .Select(i => SpawnRequestBuilder
        .For("Spawn.Zombie")
        .WithProperty("Order", i + 1)
        .Build())
    .ToArray();

// 2. Execute spawn command
engine.ExecuteCommand(new SpawnAgentsCommand(survivors.Concat(zombies).ToList()));

/*
INTERNAL FLOW:
3. CommandHandler → DescriptorBuilder looks up entity types via EntityTypeRegistry
4. Creates SurvivorDescriptor for survivors, ZombieSpawnDescriptor for zombies
5. Strategy.Process() groups by type:
   - Survivors → ProcessSingle(SurvivorDescriptor) for each
   - Zombies → ProcessBatch(List<ZombieSpawnDescriptor>) all at once
6. Descriptors wrapped in SpawnDecisions
7. Appliers create entities
*/
```

---

#### Creating Custom Entities with Type-Specific Spawn

**Step 1: Create Custom Descriptor**

```csharp
// File: BarelyAlive.Rules/Core/Domain/Descriptors/BossDescriptor.cs
public class BossDescriptor : AgentDescriptor
{
    public int PhaseCount { get; set; } = 3;
    public int HealthPerPhase { get; set; } = 500;
    public List<string> AbilityIds { get; set; } = new();
    
    public BossDescriptor(string definitionId) : base(definitionId) { }
}
```

**Step 2: Create Entity with Attributes**

```csharp
// File: BarelyAlive.Rules/Core/Domain/Entities/Boss.cs
[DefinitionType(typeof(BossDefinition))]
[DescriptorType(typeof(BossDescriptor))]
public class Boss : Agent
{
    public Boss(EntityId id, string definitionId, string name, string category)
        : base(id, definitionId, name, category)
    {
        AddComponent(new PhaseComponent());
        AddComponent(new BossAIComponent());
    }
}
```

**Step 3: Create Definition**

```csharp
public class BossDefinition : BaseGameEntityDefinition
{
    public int PhaseCount { get; set; } = 3;
    public int BaseHealth { get; set; } = 1500;
    
    public BossDefinition(string definitionId, string name, string category)
        : base(definitionId, name, category) { }
}
```

**Step 4: Add Type-Specific Processing to Strategy**

```csharp
public class BarelyAliveSpawnStrategy : BaseSpawnStrategy
{
    // Existing methods...

    // NEW: Boss-specific processing
    protected BossDescriptor ProcessSingle(
        BossDescriptor descriptor,
        GameState state)
    {
        // Find boss arena
        var bossArena = state.GetProps()
            .FirstOrDefault(p => p.Category == "BossArena");

        if (bossArena != null)
        {
            descriptor.Position = bossArena.PositionComponent.CurrentPosition;
        }

        // Scale health based on player count
        var playerCount = state.GetAgents().Count(a => a.Category == "Survivor");
        descriptor.HealthPerPhase = 500 + (playerCount * 100);

        return descriptor;
    }
}
```

**Step 5: Use It**

```csharp
// Spawn boss with fluent builder
var boss = SpawnRequestBuilder
    .For("Bosses.DragonKing")
    .WithProperty("PhaseCount", 5)
    .WithProperty("AbilityIds", new List<string> { "FireBreath", "TailSwipe", "Flight" })
    .Build();

engine.ExecuteCommand(new SpawnAgentsCommand(new[] { boss }));

// Strategy automatically calls ProcessSingle(BossDescriptor) with type safety!
```

---

#### Hierarchical Strategy Benefits

**Compile-Time Safety:**
- ✅ Missing types cause build errors
- ✅ IntelliSense support in strategy methods
- ✅ Refactoring-safe (rename detection)

**Performance:**
- ✅ Method resolution cached
- ✅ Reflection only at startup
- ✅ Zero runtime overhead after warmup

**Flexibility:**
- ✅ Batch processing for performance
- ✅ Individual processing for custom logic
- ✅ Fallback for generic types

**Maintainability:**
- ✅ Single source of truth (Entity class)
- ✅ No redundant attributes
- ✅ Clear type relationships

---

#### When to Use Hierarchical Strategy

**Use hierarchical strategy when:**
- ✅ You have game-specific spawn logic (specific positions, validation, etc.)
- ✅ You need type-safe access to custom descriptor properties
- ✅ You want batch processing for performance (distribute across spawn points)
- ✅ You have multiple entity types with different spawn rules

**Use default strategy when:**
- ❌ Spawn positions come from SpawnRequest directly
- ❌ No game-specific validation needed
- ❌ All entities spawn the same way

---

## Spatial & Board

### Spatial Model

**Interface:** `ISpatialModel` (abstraction for different board types)

**Implementations:**
- `ConnectedGraphSpatialModel` - Graph-based (nodes + edges)
- `GridSpatialModel` - 2D grid [NOT IMPLEMENTED]

### ConnectedGraphSpatialModel

Backed by `MutableTileGraph`:

```csharp
public class MutableTileGraph
{
    private Dictionary<TileId, HashSet<TileId>> _connections;
    
    public void AddTile(TileId tile);
    public void Connect(TileId from, TileId to);
    public IReadOnlyList<TileId> GetNeighbors(TileId tile);
    public int GetDistance(TileId from, TileId to); // BFS pathfinding
}
```

### GameBoard

```csharp
public class GameBoard
{
    public ISpatialModel SpatialModel { get; }
    public IReadOnlyDictionary<TileId, ZoneId> TileZones { get; }
    public IReadOnlyDictionary<ZoneId, Zone> Zones { get; }
}
```

**Zones** group tiles with shared properties (e.g., "Dark", "Indoor").

### Board Initialization

```csharp
// 1. Create spatial descriptor
var spatial = new SpatialDescriptor(
    TileIds: new HashSet<TileId> { tile1, tile2, tile3 },
    Connections: new[] { (tile1, tile2), (tile2, tile3) }
);

// 2. Create zone descriptors
var zones = new ZoneDescriptor[]
{
    new(ZoneId.New(), "DarkZone", new[] { tile1, tile2 }, 
        Behaviours: new[] { new DarkZoneBehaviour() })
};

// 3. Initialize board
var boardDescriptor = new BoardDescriptor(spatial, zones);
engine.ExecuteCommand(new InitializeBoardCommand(boardDescriptor));
```

---

## Integration Patterns

### Custom Entity Types

1. **Define Entity Subclass:**
```csharp
public class Survivor : Agent
{
    public Survivor(EntityId id, string definitionId, string name, string category)
        : base(id, name, category, definitionId)
    {
        AddComponent(new FactionComponent());
    }
}
```

2. **Create Definition:**
```csharp
[EntityType(typeof(Survivor))]
public class SurvivorDefinition : AgentDefinition
{
    [MapToComponent(typeof(IFactionComponent), "Faction")]
    public string Faction { get; set; } = "Survivors";
}
```

3. **Register in Catalog:**
```csharp
catalog.AddDefinition(new SurvivorDefinition 
{ 
    DefinitionId = "Survivors.Mike",
    Name = "Mike",
    MaxHealth = 10 
});
```

### Custom Commands

1. **Define Command:**
```csharp
public record AttackCommand(EntityId AttackerId, EntityId TargetId) : ICommand
{
    public Type CommandType => typeof(AttackCommand);
}
```

2. **Create Handler:**
```csharp
public class AttackHandler : ICommandHandler<AttackCommand>
{
    public CommandResult Handle(AttackCommand command)
    {
        var decision = new AttackDecision(command.AttackerId, command.TargetId);
        return CommandResult.Ok([decision]);
    }
}
```

3. **Create Applier:**
```csharp
public class AttackApplier : IApplier<AttackDecision>
{
    public ApplierResponse Apply(AttackDecision decision, GameState state)
    {
        var target = state.Agents[decision.TargetId];
        var damaged = target.WithHealth(target.Health - decision.Damage);
        var newState = state.WithAgent(damaged);
        
        return new ApplierResponse(newState, 
            [new AgentDamagedEffect(decision.TargetId, decision.Damage)]);
    }
}
```

4. **Register:**
```csharp
services.Register<ICommandHandler<AttackCommand>>(new AttackHandler());
orchestrator.RegisterApplier(new AttackApplier());
```

5. **Allow in FSM Node:**
```csharp
public class PlayerTurnNode : LeafNode
{
    public PlayerTurnNode()
    {
        AddAllowedCommand<MoveCommand>();
        AddAllowedCommand<AttackCommand>(); // ← Add here
    }
}
```

### Testing Patterns

**FSM Tests:**
```csharp
[SetUp]
public void Setup()
{
    var builder = new GameFlowBuilder();
    var root = builder.AddRoot<SpyBranch>("Root", r => {
        r.AddLeaf<SpyNode>("Child1");
        r.AddLeaf<SpyNode>("Child2");
    }).Build();
    
    controller = new FsmController(root, root.Id);
    
    // IMPORTANT: Sync repository with auto-navigated state
    var state = repository.LoadGameState()
        .WithCurrentStateId(controller.CurrentStateId);
    repository.SaveGameState(state);
}

[Test]
public void MoveForward_TransitionsCorrectly()
{
    // Controller auto-navigated to Child1
    var state = repository.LoadGameState();
    Assert.That(state.CurrentStateId, Is.EqualTo(child1.Id));
    
    // Transition Child1 → Child2
    var result = controller.MoveForwardRequest(state);
    repository.SaveGameState(result.State);
    
    Assert.That(repository.LoadGameState().CurrentStateId, Is.EqualTo(child2.Id));
}
```

**Applier Tests:**
```csharp
[Test]
public void SpawnApplier_CreatesEntity()
{
    var descriptor = new SurvivorDescriptor("Survivors.Mike") { Position = position };
    var decision = new SpawnDecision<SurvivorDescriptor>(descriptor);
    
    var response = applier.Apply(decision, GameState.Empty());
    
    Assert.That(response.GameState.Agents, Has.Count.EqualTo(1));
    Assert.That(response.GameEffects, Has.Length.EqualTo(1));
}
```

---

## Common Pitfalls

| Issue | Symptom | Solution |
|-------|---------|----------|
| **Not syncing repository after FSM init** | `CurrentStateId` is null | Always sync after `FsmController` creation |
| **Expecting BranchNode start state** | Tests fail unexpectedly | Account for auto-navigation to first leaf |
| **Commands on BranchNode** | Commands rejected | Ensure FSM is at LeafNode before sending commands |
| **Missing applier registration** | Runtime exception | Register all decision types in orchestrator |
| **State mutation in handler** | Inconsistent state | Handlers return decisions only, never mutate |
| **Skipping decision timing** | Effects execute at wrong time | Set correct `DecisionTiming` for each decision |
| **Wrong spawn strategy type** | Type mismatch errors | Match strategy generic to descriptor type |

---

## Quick Reference

### Key Interfaces

```csharp
// Commands
public interface ICommand { Type CommandType { get; } }
public interface ICommandHandler<TCommand> 
{ 
    CommandResult Handle(TCommand command); 
}

// Decisions
public interface IDecision 
{ 
    DecisionTiming Timing { get; } 
}

// Appliers
public interface IApplier<TDecision> 
{ 
    ApplierResponse Apply(TDecision decision, GameState state); 
}

// Entities
public interface IGameEntityComponent { }
public interface IGameEntityDescriptor 
{ 
    string DefinitionID { get; } 
}

// Spawn
public interface ISpawnStrategy<TDescriptor>
{
    IReadOnlyList<SpawnDecision<TDescriptor>> Process(
        IReadOnlyList<SpawnRequest> requests, 
        GameState state
    );
}
```

### Immutable State Methods

```csharp
state.WithAgent(agent);           // Add/update agent
state.WithProp(prop);             // Add/update prop
state.WithCurrentStateId(nodeId); // Update FSM state
state.WithBoard(board);           // Set board
state.WithScheduler(scheduler);   // Update scheduler

agent.WithPosition(position);     // Update position
agent.WithHealth(health);         // Update health
agent.WithComponent<T>(component); // Update component
```

---

**End of Reference**
