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
6. [Spawn System](#spawn-system)
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

### Definition-Descriptor Pattern

**Definition** = Blueprint (stored in catalog)
```csharp
[EntityType(typeof(Survivor))]
public class SurvivorDefinition : AgentDefinition
{
    [MapToComponent(typeof(IHealthComponent), "MaxHealth")]
    public int MaxHealth { get; set; } = 10;
    
    [MapToComponent(typeof(IFactionComponent), "Faction")]
    public string Faction { get; set; } = "Survivors";
}
```

**Descriptor** = Spawn request (includes position, overrides)
```csharp
public class SurvivorDescriptor : AgentDescriptor
{
    public SurvivorDescriptor(string definitionId) : base(definitionId) { }
    
    [MapToComponent(typeof(IPositionComponent), "CurrentPosition")]
    public Position Position { get; set; }
}
```

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

## Spawn System

### Architecture

```
SpawnCommand (SpawnRequest[])
    ↓
Strategy.Process(requests, state)
    ↓
SpawnDecision<TDescriptor>[]
    ↓
SpawnApplier.Apply(decision, state)
    ↓
EntitySpawnedEffect
```

### SpawnRequest

```csharp
public record SpawnRequest(
    string DefinitionId,                        // Required
    int Count = 1,                              // Batch spawn
    Position? Position = null,                  // Strategy decides if null
    Dictionary<string, object>? PropertyOverrides = null
);
```

### Spawn Strategy

Each strategy is typed to a specific descriptor:

```csharp
public interface ISpawnStrategy<TDescriptor> 
    where TDescriptor : IGameEntityDescriptor
{
    IReadOnlyList<SpawnDecision<TDescriptor>> Process(
        IReadOnlyList<SpawnRequest> requests,
        GameState state
    );
}
```

**Responsibilities:**
- Expand `Count` (batch spawn)
- Determine position (if not provided)
- Validate spawn conditions
- Create typed descriptors
- Apply property overrides

**Example:**
```csharp
public class SurvivorSpawnStrategy : ISpawnStrategy<SurvivorDescriptor>
{
    public IReadOnlyList<SpawnDecision<SurvivorDescriptor>> Process(
        IReadOnlyList<SpawnRequest> requests,
        GameState state)
    {
        var decisions = new List<SpawnDecision<SurvivorDescriptor>>();
        
        foreach (var request in requests)
        {
            for (int i = 0; i < request.Count; i++)
            {
                var descriptor = new SurvivorDescriptor(request.DefinitionId)
                {
                    Position = request.Position ?? FindPartySpawnPoint(state),
                    Faction = "Alliance"
                };
                
                if (request.PropertyOverrides != null)
                    ApplyOverrides(descriptor, request.PropertyOverrides);
                
                if (!IsValidPosition(descriptor.Position, state))
                    continue; // Skip invalid spawn
                
                decisions.Add(new SpawnDecision<SurvivorDescriptor>(descriptor));
            }
        }
        
        return decisions;
    }
    
    private Position FindPartySpawnPoint(GameState state)
    {
        var spawnProp = state.GetProps()
            .FirstOrDefault(p => p.HasBehaviour<PartySpawnBehaviour>());
        return spawnProp?.PositionComponent.CurrentPosition ?? Position.Empty;
    }
}
```

### Spawn Applier

```csharp
public class SpawnApplier<TDescriptor> : IApplier<SpawnDecision<TDescriptor>> 
    where TDescriptor : IGameEntityDescriptor
{
    public ApplierResponse Apply(SpawnDecision<TDescriptor> decision, GameState state)
    {
        var definition = _catalog.GetDefinition(decision.Descriptor.DefinitionID);
        var entity = _factory.Build(decision.Descriptor);
        
        var newState = state.WithAgent((Agent)entity); // or WithProp
        var effect = new EntitySpawnedEffect(entity.Id, decision.Descriptor.Position);
        
        return new ApplierResponse(newState, [effect]);
    }
}
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

**End of Reference**
