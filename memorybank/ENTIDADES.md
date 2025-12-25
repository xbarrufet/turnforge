# TurnForge.Engine Reference

**Technical documentation for the TurnForge game engine.**
**Target audience:** Developers and AI agents building turn-based tactical games.

*Work in progress - evolving with the engine.*

---

## How to Use This Document

This reference is organized into **3 parts**:

- **Part I: Understanding TurnForge** - Learn how the engine works internally (architecture, patterns, flows)
- **Part II: Using TurnForge** - Practical API guide for building your game (commands, strategies, components)
- **Part III: API Reference** - Quick lookup for interfaces, types, and signatures

💡 **New to TurnForge?** Start with Part I to understand concepts, then Part II for implementation.
🔍 **Building a feature?** Jump to Part II for the relevant API section.
⚡ **Need a signature?** Part III has quick reference tables.

---

## Table of Contents

### Part I: Understanding TurnForge (How It Works)

1. [Overview & Core Principles](#part-i-overview--core-principles)
2. [Architecture Deep Dive](#architecture-deep-dive)
   - [Command-Decision-Applier Pattern](#command-decision-applier-pattern)
   - [FSM State Machine](#fsm-finite-state-machine)
   - [Orchestrator Role](#orchestrator)
   - [Immutable State Philosophy](#immutable-state)
3. [Execution Flow](#execution-flow)
   - [Command Lifecycle](#command-lifecycle)
   - [Decision Scheduling](#decision-scheduling)
   - [FSM Transitions](#fsm-transitions)
   - [ACK System](#ack-system)
4. [Internal Systems](#internal-systems)
   - [Action System Pipeline](#action-system-pipeline)
   - [Spawn System Pipeline](#spawn-command)
   - [Board & Spatial Internals](#board--spatial-system)
   - [Effects Propagation](#effects-system)
   - [Factory System](#factory-system)

### Part II: Using TurnForge (Public API)

1. [Getting Started](#getting-started)
2. [Entity System API](#entity-system-api)
3. [Command System API](#command-system-api)
4. [Strategy System API](#strategy-system-api)
5. [Component API](#component-api)
6. [FSM Configuration](#fsm-configuration)
7. [Services API](#services-api)
8. [Extension Points](#extension-points)

### Part III: API Reference (Quick Lookup)

1. [Core Interfaces](#core-interfaces-reference)
2. [Command Types](#command-types-reference)
3. [Strategy Interfaces](#strategy-interfaces-reference)
4. [Component Interfaces](#component-interfaces-reference)
5. [Effect Types](#effect-types-reference)

---

# Part I: Understanding TurnForge

*This section explains how TurnForge works internally. Read this to understand the architecture, design patterns, and execution model.*

---

## Part I: Overview & Core Principles

TurnForge is a turn-based tactical game engine built on immutable state and clear separation of concerns.

**What is TurnForge?**

A C# engine for creating tactical turn-based games (skirmish wargames, tactical RPGs, etc.). It provides:
- **FSM-driven game flow** - Manage phases, turns, and state transitions declaratively
- **Component-based entities** - Flexible agent/prop system with reusable components
- **Command pattern** - Separate user intent from execution logic
- **Strategy extensibility** - Customize game rules without modifying the engine

**Core Components:**
- **FSM** - Finite State Machine managing turn flow and valid actions per phase
- **Entities** - Agents (active) and Props (static) with component-based data
- **Commands** - User/AI intentions (Move, Attack, Spawn, etc.)
- **Strategies** - Configurable business logic for processing commands
- **Decisions** - Intent-to-execution bridge (what should happen)
- **Appliers** - State mutators (make it happen)
- **Effects** - UI notifications (show what happened)

---

## Architecture Deep Dive

TurnForge implements a **Command-Decision-Applier** pattern with FSM-controlled state transitions.

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

## Board & Spatial System

The Board system provides spatial management and position validation for the game.

### ISpatialModel

Interface defining how positions relate to each other and movement rules.

```csharp
public interface ISpatialModel
{
    /// <summary>
    /// Check if a position exists and is valid on the board.
    /// </summary>
    bool IsValidPosition(Position position);
    
    /// <summary>
    /// Get neighboring positions (e.g., adjacent tiles).
    /// </summary>
    IEnumerable<Position> GetNeighbors(Position position);
    
    /// <summary>
    /// Check if an actor can move to target position (pathfinding, obstacles, etc.).
    /// </summary>
    bool CanMove(Actor actor, Position target);
    
    /// <summary>
    /// Calculate distance between two positions.
    /// </summary>
    int Distance(Position from, Position to);
    
    /// <summary>
    /// Enable/disable connections for dynamic board changes.
    /// </summary>
    void EnableConnection(Position from, Position to);
    void DisableConnection(Position from, Position to);
}
```

**Implementation Examples:**
- **GridSpatialModel** - Square/hex grid with cardinal/diagonal movement
- **GraphSpatialModel** - Node-based with custom connections
- **HybridSpatialModel** - Mixed grid + custom connections

---

### GameBoard

Central board entity managing zones and delegating spatial queries.

```csharp
public sealed class GameBoard : GameEntity
{
    public GameBoard(ISpatialModel spatialModel);
    
    // Zone Management
    public void AddZone(Zone zone);
    public IReadOnlyList<Zone> Zones { get; }
    public IEnumerable<Zone> GetZonesAt(Position position);
    
    // Spatial Queries (delegates to ISpatialModel)
    public bool IsValid(Position position);
    public IEnumerable<Position> GetNeighbors(Position position);
    public int Distance(Position from, Position to);
    public bool CanMoveActor(Actor actor, Position target);
}
```

**Usage:**
```csharp
// In ActionCommandHandler, context provides board
public ActionStrategyResult Execute(MoveCommand command, IActionContext context)
{
    // Validate position
    if (!context.Board.IsValid(command.TargetPosition))
        return ActionStrategyResult.Failed("Invalid position");
    
    // Check movement rules
    var agent = _query.GetAgent(command.AgentId);
    if (!context.Board.CanMoveActor(agent, command.TargetPosition))
        return ActionStrategyResult.Failed("Cannot move to position");
    
    // Calculate distance for cost
    var distance = context.Board.Distance(agent.Position, command.TargetPosition);
}
```

---

### Zone

Logical area grouping positions (spawn zones, objectives, etc.).

```csharp
public sealed class Zone : GameEntity
{
    public string ZoneType { get; set; }  // "SpawnZone", "ObjectiveZone", etc.
    public IReadOnlyList<Position> Positions { get; }
    
    public Zone(EntityId id, string definitionId, string name, string category);
    
    public bool Contains(Position position);
    public void AddPosition(Position position);
}
```

**Example:**
```csharp
// Create spawn zone
var spawnZone = new Zone(EntityId.New(), "spawn_survivor_1", "Survivor Start", "SpawnZone");
spawnZone.AddPosition(Position.FromTile(tileId1));
spawnZone.AddPosition(Position.FromTile(tileId2));

gameBoard.AddZone(spawnZone);

// Query zones at position
var zones = gameBoard.GetZonesAt(position);
var isSpawnPoint = zones.Any(z => z.ZoneType == "SpawnZone");
```

---

## Effects System

Effects communicate state changes to the UI for animation and feedback.

### IGameEffect

Base interface for all game effects.

```csharp
public interface IGameEffect
{
    /// <summary>
    /// Origin/source of this effect (Command, PhaseTransition, etc.).
    /// Enables UI to filter and display effects appropriately.
    /// </summary>
    EffectOrigin Origin { get; }
    
    /// <summary>
    /// Timestamp when effect was generated (UTC).
    /// Enables replay, debugging, and audit logs.
    /// </summary>
    DateTime Timestamp { get; }
    
    /// <summary>
    /// Human-readable description of the effect.
    /// Used for logging and debugging without inspecting effect type.
    /// </summary>
    string Description { get; }
}
```

---

### Common Effect Types

All effects inherit from `GameEffect : IGameEffect` base record.

#### ComponentsUpdatedEffect
Entity components changed (position, health, AP, etc.).

```csharp
public record ComponentsUpdatedEffect(
    EntityId EntityId,
    Type[] UpdatedComponentTypes
) : GameEffect(EffectOrigin.Command, "Components updated");
```

**Generated by:** `ActionDecisionApplier`

#### EntitySpawnedEffect
New entity created and added to state.

```csharp
public record EntitySpawnedEffect(
    EntityId EntityId,
    string DefinitionId,
    Position? SpawnPosition
) : GameEffect(EffectOrigin.Command, "Entity spawned");
```

**Generated by:** `AgentSpawnApplier`, `PropSpawnApplier`

#### FSMTransitionEffect
Game phase changed.

```csharp
public record FSMTransitionEffect(
    string FromStateId,
    string ToStateId,
    string TransitionTag
) : GameEffect(EffectOrigin.PhaseTransition, "State transition");
```

**Generated by:** `FsmController`

---

### Effect Usage Pattern

```csharp
// In Applier
public ApplierResponse Apply(SomeDecision decision, GameState state)
{
    // 1. Mutate state
    var newState = state.WithAgent(updatedAgent);
    
    // 2. Create effect(s)
    var effect = new ComponentsUpdatedEffect(
        agent.Id,
        new[] { typeof(BasePositionComponent), typeof(BaseActionPointsComponent) }
    );
    
    // 3. Return both
    return new ApplierResponse(newState, new[] { effect });
}

// In UI Layer
engine.OnEffectGenerated += (effect) =>
{
    switch (effect)
    {
        case ComponentsUpdatedEffect componentEffect:
            AnimateComponentChange(componentEffect.EntityId, componentEffect.UpdatedComponentTypes);
            break;
        case EntitySpawnedEffect spawnEffect:
            ShowSpawnAnimation(spawnEffect.EntityId, spawnEffect.SpawnPosition);
            break;
    }
};
```

---

## Factory System

Factories create entities from definitions and descriptors.

### GenericActorFactory

Creates entities with automatic component mapping.

```csharp
public sealed class GenericActorFactory : IActorFactory
{
    public GenericActorFactory(IGameCatalog gameCatalog);
    
    public Agent BuildAgent(AgentDescriptor descriptor);
    public Prop BuildProp(PropDescriptor descriptor);
}
```

**Entity Creation Process:**
1. **Load Definition** - Fetch from catalog by `descriptor.DefinitionID`
2. **Determine Type** - Use `EntityTypeRegistry` or `[EntityType]` attribute
3. **Create Instance** - Reflection with constructor(EntityId, definitionId, name, category)
4. **Map Properties** - `PropertyAutoMapper.Map(definition, entity)` then `Map(descriptor, entity)`
5. **Set Position** - Direct assignment `entity.PositionComponent.CurrentPosition = descriptor.Position`
6. **Add Extra Components** - Append `descriptor.ExtraComponents`

**Example:**
```csharp
// Create factory
var factory = new GenericActorFactory(gameCatalog);

// Build agent
var descriptor = new AgentDescriptor("Survivors.Mike")
{
    Position = spawnPosition,
    ExtraComponents = new[] { new BerserkComponent() }
};

var agent = factory.BuildAgent(descriptor);
// → Agent created with components from definition + descriptor + extras
```

---

### PropertyAutoMapper

Automatically maps properties from Definition/Descriptor to Entity components.

**Mapping Rules:**
1. **Implicit** - Property name/type matches component property → auto-map
2. **Explicit** - `[MapToComponent(typeof(IComponent), "PropertyName")]` → forced mapping
3. **Opt-Out** - `[DoNotMap]` on component property → skip

**Process:**
```csharp
// Step 1: Map from definition
PropertyAutoMapper.Map(definition, agent);
// MaxHealth → IHealthComponent.MaxHealth
// Faction → IFactionComponent.FactionName (via [MapToComponent])

// Step 2: Map from descriptor (overrides)
PropertyAutoMapper.Map(descriptor, agent);
// Position → IPositionComponent.CurrentPosition
```

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

## Action Commands

Action commands represent user/AI intentions to perform game actions (move, attack, etc.). They follow the Command-Strategy-Decision-Applier pattern.

### MoveCommand

Command to move an agent from current position to a target position.

```csharp
public sealed record MoveCommand(
    string AgentId,          // Entity to move
    bool HasCost,            // Whether movement costs AP
    Position TargetPosition  // Destination
) : IActionCommand
{
    public Type CommandType => typeof(MoveCommand);
}
```

**Usage:**
```csharp
var moveCmd = new MoveCommand(
    agentId: "agent-123",
    hasCost: true,  // Will consume AP
    targetPosition: Position.FromTile(targetTile)
);

var result = engine.ExecuteCommand(moveCmd);
```

**Flow:**
1. `ActionCommandHandler` validates and routes to strategy
2. `IActionStrategy<MoveCommand>` (e.g., `BasicMoveStrategy`) validates and calculates cost
3. Returns `ActionDecision` with position + AP updates
4. `ActionDecisionApplier` applies component changes to state

---

## Action System Pipeline

The Action System implements the Command-Strategy-Decision-Applier pattern for processing player/AI actions.

### Pipeline Overview

```
ActionCommand → ActionCommandHandler → IActionStrategy → ActionStrategyResult
     ↓                   ↓                    ↓                   ↓
 (MoveCommand)    (validates AP)      (calculates cost)    (ActionDecision[])
                                              ↓
                                      ActionDecisionApplier
                                              ↓
                                    GameState + IGameEffect[]
```

**Key Principle:** Strategies generate decisions (what to do), Appliers execute them (state mutation).

---

### ActionCommandHandler

Generic command handler that routes action commands to their strategies.

```csharp
public sealed class ActionCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : IActionCommand
{
    public ActionCommandHandler(
        IActionStrategy<TCommand> strategy,
        IGameStateQuery queryService,
        IGameRepository repository);
    
    public CommandResult Handle(TCommand command);
}
```

**Responsibilities:**
1. Load current game state
2. Validate entity has ActionPoints (if `HasCost = true`)
3. Create `ActionContext` with state + board
4. Execute strategy
5. Return decisions to FSM or error

**Pre-Validation:**
```csharp
// Handler checks BEFORE strategy execution:
if (command.HasCost)
{
    var agent = queryService.GetAgent(command.AgentId);
    var apComponent = agent.GetComponent<IActionPointsComponent>();
    
    if (apComponent.CurrentActionPoints <= 0)
        return CommandResult.Fail("Agent has no ActionPoints");
}
```

**Registration:**
```csharp
// In game setup
var moveHandler = new ActionCommandHandler<MoveCommand>(
    new BasicMoveStrategy(queryService),
    queryService,
    repository
);

commandBus.RegisterHandler(moveHandler);
```

---

### IActionContext

Provides immutable context to strategies during execution.

```csharp
public interface IActionContext
{
    /// <summary>
    /// Current game state (immutable snapshot).
    /// </summary>
    GameState State { get; }
    
    /// <summary>
    /// Game board with spatial model for position validation.
    /// </summary>
    GameBoard Board { get; }
}
```

**Usage in Strategy:**
```csharp
public ActionStrategyResult Execute(MoveCommand command, IActionContext context)
{
    // Access board for validation
    if (!context.Board.IsValid(command.TargetPosition))
        return ActionStrategyResult.Failed("Invalid position");
    
    // Access state for queries (via QueryService preferred)
    var agents = context.State.GetAgents();
}
```

---

### ActionDecision

Represents a set of component updates to apply to an entity.

```csharp
public sealed record ActionDecision : IDecision
{
    /// <summary>
    /// ID of entity to update (Agent or Prop).
    /// </summary>
    public string EntityId { get; init; }
    
    /// <summary>
    /// Components to add/update on the entity.
    /// Key = component type, Value = component instance.
    /// </summary>
    public IReadOnlyDictionary<Type, IGameEntityComponent> ComponentUpdates { get; init; }
    
    /// <summary>
    /// When this decision should be executed.
    /// </summary>
    public DecisionTiming Timing { get; init; }
    
    /// <summary>
    /// ID of command/source that originated this decision.
    /// </summary>
    public string OriginId { get; init; }
}
```

**Direct Construction** (not recommended):
```csharp
var decision = new ActionDecision(
    entityId: "agent-123",
    componentUpdates: new Dictionary<Type, IGameEntityComponent>
    {
        [typeof(BasePositionComponent)] = new BasePositionComponent(newPos),
        [typeof(BaseActionPointsComponent)] = new BaseActionPointsComponent(2)
    },
    timing: DecisionTiming.Immediate,
    originId: "move-command"
);
```

---

### ActionDecisionBuilder

Fluent API for building action decisions (recommended approach).

```csharp
public sealed class ActionDecisionBuilder
{
    public ActionDecisionBuilder ForEntity(string entityId);
    public ActionDecisionBuilder UpdateComponent<T>(T component) where T : IGameEntityComponent;
    public ActionDecisionBuilder WithTiming(
        DecisionTimingWhen when,
        string? phase = null,
        DecisionTimingFrequency frequency = DecisionTimingFrequency.Single);
    public ActionDecisionBuilder WithOrigin(string originId);
    public ActionDecision Build();
}
```

**Usage Example:**
```csharp
// In strategy
var decision = new ActionDecisionBuilder()
    .ForEntity(command.AgentId)
    .UpdateComponent(new BasePositionComponent(command.TargetPosition))
    .UpdateComponent(new BaseActionPointsComponent(maxAP)
    {
        CurrentActionPoints = currentAP - cost
    })
    .WithOrigin(command.AgentId)
    .Build();

return ActionStrategyResult.Success(decision)
    .WithMetadata(new ActionMetadata { ActionPointsCost = cost });
```

**Component Immutability Pattern:**
> [!IMPORTANT]
> Always create NEW component instances with updated values. Never mutate existing components.
>
> ```csharp
> // ✅ CORRECT: Create new instance
> var updatedAP = new BaseActionPointsComponent(apComponent.MaxActionPoints)
> {
>     CurrentActionPoints = apComponent.CurrentActionPoints - cost
> };
>
> // ❌ WRONG: Mutate existing
> apComponent.CurrentActionPoints -= cost;  // Breaks immutability!
> ```

---

### ActionDecisionApplier

Applies ActionDecisions to GameState by updating entity components.

```csharp
public sealed class ActionDecisionApplier : IApplier<ActionDecision>
{
    public ApplierResponse Apply(ActionDecision decision, GameState state);
}
```

**Process:**
1. Find entity (Agent or Prop) by `decision.EntityId`
2. Apply each component update via `entity.AddComponent()`
3. Update state with modified entity (`state.WithAgent()` / `state.WithProp()`)
4. Generate `ComponentsUpdatedEffect` for UI

**Implementation Notes:**
- Uses **mutable pattern** for entity components (AddComponent overwrites)
- Uses **immutable pattern** for GameState (WithAgent returns new state)
- Gracefully ignores decisions for missing entities

**Effect Generation:**
```csharp
// Applier creates effect for UI
var effect = new ComponentsUpdatedEffect(
    entityId,
    decision.ComponentUpdates.Keys.ToArray()  // Which components changed
);
```

**Registration:**
```csharp
// In game setup
orchestrator.RegisterApplier(new ActionDecisionApplier());
```

---

### Complete Action Flow Example

```csharp
// 1. User/AI creates command
var moveCmd = new MoveCommand(
    agentId: "agent-123",
    hasCost: true,
    targetPosition: targetTile
);

// 2. Engine routes to handler
var handler = commandBus.GetHandler<MoveCommand>();
var result = handler.Handle(moveCmd);

/* INSIDE HANDLER:
 * 3. Handler validates AP
 * 4. Creates context
 * 5. Calls strategy.Execute()
 */

/* INSIDE STRATEGY (BasicMoveStrategy):
 * 6. Validates target position
 * 7. Calculates cost (1 AP)
 * 8. Builds decision with ActionDecisionBuilder
 * 9. Returns ActionStrategyResult.Success(decision)
 */

/* BACK IN ENGINE:
 * 10. FSM receives decisions from CommandResult
 * 11. Scheduler queues decisions
 * 12. Orchestrator calls ActionDecisionApplier.Apply()
 */

/* INSIDE APPLIER:
 * 13. Updates entity components (position, AP)
 * 14. Creates new GameState with updated entity
 * 15. Generates ComponentsUpdatedEffect
 */

// 16. UI receives effect and animates
```

---

## Strategies

Strategies contain game-specific business logic for processing commands. They validate, calculate costs, and generate decisions WITHOUT mutating state.

### IActionStrategy<TCommand>

Base interface for all action strategies.

```csharp
public interface IActionStrategy<TCommand> where TCommand : IActionCommand
{
    /// <summary>
    /// Execute strategy logic for the command.
    /// Returns success with decisions, or failure with validation errors.
    /// </summary>
    ActionStrategyResult Execute(TCommand command, IActionContext context);
}
```

### BasicMoveStrategy

Default movement strategy with fixed 1 AP cost.

```csharp
public sealed class BasicMoveStrategy : IActionStrategy<MoveCommand>
{
    public BasicMoveStrategy(IGameStateQuery query);
    
    public ActionStrategyResult Execute(MoveCommand command, IActionContext context);
}
```

**Validation Rules:**
- Agent must exist
- Target position must be valid on board
- Agent must not already be at target position
- Agent must have sufficient AP (if `HasCost = true`)

**Cost Calculation:**
- Fixed 1 AP per move (override in custom strategies for game-specific rules)

**Example Custom Strategy (Zombicide):**
```csharp
public class BarelyAliveMovementStrategy : IActionStrategy<MoveCommand>
{
    public ActionStrategyResult Execute(MoveCommand command, IActionContext context)
    {
        // Custom logic: Survivors pay 1 + zombies at starting position
        var agent = _query.GetAgent(command.AgentId);
        var zombiesAtStart = _query.GetAgentsAt(agent.Position)
            .Count(a => a.Category == "Zombie");
        
        var cost = 1 + zombiesAtStart;
        
        // Build decision with custom cost
        var decision = new ActionDecisionBuilder()
            .ForEntity(command.AgentId)
            .UpdateComponent(new BasePositionComponent(command.TargetPosition))
            .UpdateComponent(UpdatedAPComponent(agent, cost))
            .Build();
        
        return ActionStrategyResult.Success(decision)
            .WithMetadata(new ActionMetadata { ActionPointsCost = cost });
    }
}
```

---

## Services

Services provide query and utility functionality without mutating state.

### GameStateQueryService

Provides read-only queries over game state for use in strategies, handlers, and decision logic.

```csharp
public sealed class GameStateQueryService : IGameStateQuery
{
    public GameStateQueryService(GameState state);
    
    // Agent Queries
    Agent? GetAgent(string agentId);
    IReadOnlyList<Agent> GetAllAgents();
    IReadOnlyList<Agent> GetAgentsAt(Position position);
    IReadOnlyList<Agent> GetAgentsByCategory(string category);  // Case-insensitive
    
    // Prop Queries
    Prop? GetProp(string propId);
    IReadOnlyList<Prop> GetPropsAt(Position position);
    
    // Position Queries
    bool IsPositionOccupied(Position position);
    int CountAgentsAt(Position position, string? category = null);
}
```

**Usage Examples:**
```csharp
var query = new GameStateQueryService(gameState);

// Find specific agent
var agent = query.GetAgent("agent-123");

// Get all agents at a position
var agentsAtTile = query.GetAgentsAt(Position.FromTile(tileId));

// Filter by category (case-insensitive)
var zombies = query.GetAgentsByCategory("Zombie");
var survivors = query.GetAgentsByCategory("survivor");  // Works!

// Check if position is occupied
if (query.IsPositionOccupied(targetPos))
{
    // Position blocked
}

// Count specific category at position
var zombieCount = query.CountAgentsAt(position, "Zombie");
```

**Design Notes:**
- **Immutable**: QueryService doesn't mutate state
- **Stateless**: Can be created per operation or cached
- **Filtering**: Category queries use case-insensitive string comparison
- **Performance**: Returns `IReadOnlyList` to prevent accidental mutations

---

## Components

Components are the atomic data units attached to entities. All entity state is stored in components.

### IActionPointsComponent

Manages an entity's action points for turn-based actions.

```csharp
public interface IActionPointsComponent : IGameEntityComponent
{
    /// <summary>
    /// Maximum action points for this entity.
    /// </summary>
    int MaxActionPoints { get; set; }
    
    /// <summary>
    /// Current available action points.
    /// </summary>
    int CurrentActionPoints { get; set; }
    
    /// <summary>
    /// Check if entity can afford a specific AP cost.
    /// </summary>
    bool CanAfford(int cost);
    
    /// <summary>
    /// Restore action points to maximum.
    /// </summary>
    void Restore();
}
```

**Base Implementation:**
```csharp
public sealed class BaseActionPointsComponent : IActionPointsComponent
{
    public int MaxActionPoints { get; set; }
    public int CurrentActionPoints { get; set; }
    
    public BaseActionPointsComponent(int maxActionPoints)
    {
        MaxActionPoints = maxActionPoints;
        CurrentActionPoints = maxActionPoints;
    }
    
    public bool CanAfford(int cost) => CurrentActionPoints >= cost;
    
    public void Restore() => CurrentActionPoints = MaxActionPoints;
}
```

**Usage in Strategies:**
```csharp
// Check if agent can afford movement
var apComponent = agent.GetComponent<IActionPointsComponent>();
if (apComponent != null && !apComponent.CanAfford(movementCost))
{
    return ActionStrategyResult.Failed("Insufficient Action Points");
}

// Update AP after move (create new component - immutability)
var updatedAP = new BaseActionPointsComponent(apComponent.MaxActionPoints)
{
    CurrentActionPoints = apComponent.CurrentActionPoints - movementCost
};

var decision = new ActionDecisionBuilder()
    .UpdateComponent(updatedAP)
    .Build();
```

**Component Mutation Pattern:**
> [!IMPORTANT]
> Components should be treated as immutable in strategies. Always create NEW instances with updated values rather than modifying existing components.
> 
> **Why?** Ensures state immutability and allows proper undo/replay functionality.

---

**End of Reference**
