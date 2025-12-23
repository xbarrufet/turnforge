# TURN FORGE ENTITIES
This document pretends to clarify what is an entity in TurnForge. How is defined and how is built

## Concpets
- *TurnForge Entity*: A TurnForge Entity is a game object that has a unique identifier, a categorization and a set of components.
Ex: Agent, Item, Prop
- *Components*: A component is a data structure that stores data about an entity. The estructure can be shared across entitis and it's teh atomic piece of updates. What a Entity is is reprensented by its components
Ex: HealthComponent {maxHealth, currentHealth}, PositionComponent {tileId?, position?}, InvetoryComponent {inventory?}
- *Definition*: Definition is a blueprint of an entity, It defines the inital state of the entity and of their components. Every definition has an unique id. They are aim to faciliatate the creation of entities from descriptors
> MikeSurvivor :DescriptorID="Survivors.Mike" Name = "Mike", Category = "Survivor", HealthComponent {maxHealth = 1}
- *Descriptor*: Descriptor is a data structure that stores data about an entity ti be creaated. Essentially has a *DefinitionId* + extra attributes It is used to create entities from definitions adding extra information if needed
> MikeSurvivorDescriptor :DefinitionId="Survivors.Mike", PositionComponent {tileId = "aad-ffg"}
- *Factory*: Factory is a class that creates entities from definitions and descriptors

## TurnForge Entities Hiherarchy

TurnForge.Entities hierarche is as follows:
+ GameEntity
    + Actor (entity that lives in the board)
        + Agent (entity that can take actions)
        + Prop (entity that can't take actions)
    + Item (pickup/usable, no position) [Not implemented]

The composition of the entites is
+ GameEntity: {EntutyId, Name, Category, BehaviourComponent}
    + Actor: GameEntity + PositionComponent + HealthComponent
        + Agent: Actor , MovementComponent, ActionTakeComponent
        + Prop: Actor: GameEntity + [to be defined]
    + Item: GameEntity + [to be defined]

## TurnForge Base Components
- *BehaviourComponent*: Stores the list of begaviours. A behaviour is a dynamic data strcuture that can be used to define the behaviuour of an entity. Can be added, removed, etc.. Has a Name and a undefined number of properties
> {name:"Fly", properties:{maxAltitude = 10}}, {name:"Swim", properties:[]}
- *PositionComponent*: Stores the position of the entity.
- *HealthComponent*: Stores the health of the entity.

## Definitions
- Exist a GameEntityDefinition class than implement a *IDefinition*◊ interface
>public abstract class GameEntityDefinition
>{
>    public string DefinitionId { get; set; } = string.Empty;
>    public string Name { get; set; } = string.Empty;
>    public string Category { get; set; } = string.Empty;
>}
- Any custom definition must inherit from GameEntityDefinition


## Enity creation an update process
### Creation
- First of all, we have a *definition* of the entity in the GameCatalog.
- The entity to be created is defined by a *descriptor*, the descriptor has always a definitionId.
- The descriptor can have extra attributes that are not present in the definition.
- The descriptor is analized by an estragegy that decides if the entity is valid to be created in that case created a *BuildDecision*
- The *BuildDecision* is send to the *BuildApplier* that created the entity and return a *EntityCreatedResult*, it uses the information coming from the *definition* and the *descriptor*, in that order. The *definitinId* is stored in teh entitty for tracing purposes, but all the data must be in the Components

### Update
- The atomic piece of the update is a *Component*, no single update is allowed to modify more than one component
- The update request and the entity to be updated are analized by an estragegy that decides if the entity is valid to be updated in that case created a *UpdateDecision*
- *UpdateDecision* can also be generated as a result of certain events
- The *UpdateDecision* is send to the *UpdateApplier* that created the entity and return a *CompoentUpdatedResult*

## How to use the entity system in a game
- All entities classes are abstract son it must be extended to be used
- Extension is done by
1) Extends components
A component can be extended by adding new properties to it or overriding existing ones, in these case, the engine will use the new component internallu
2) Creates new compoentns
A component can be created by interfaceing *IBaseComponent* and implementing the required methods
3) Creates a new Entity
An entity can be created by registering the new entity in the *GameCatalog* with the desired components
Ex: Surivor = EntityFactory.BuilFrom<Agent>().WithComponet<FactionComponent>()
4) Definition
To creates a defintion of a a new entituy you have to create the blueprint of the definition, to set the required fields for this defintiion
> SurvivorDeinition class extends AgentDefinition {
>       [MapToComponent("FactionComponent"), MapToProperty("Faction")]
>        public String Faction {get; set;}
>        [MapToComponent("BehaviourComponent"), MapToProperty("Behaviours")]
>        public List<BaseBehaviour> Behaviours {get; set;}
>        //Autodiscover: the systems check that name belongs to the HealthComponent and the property is 
>        public int MaxHealth {get; set;}
>}
then we canb start adding the entity to the game
>SurvivorDefintion Mike = new SurvivorDefintion();
>Mike.Name = "Mike";
>Mike.Faction = "Survivors";
>Mike.MaxHealth = 10;
>Catalog.AddDefinition(Mike);

5) Descriptors
A descriptor is created by instatntiating the definition class and the and the reqired fields if needd
>MikeDescriptor(String definitionId): AgentDescriptor {
>    [MapToComponent("PositionComponent"), MapToProperty("CurrentPosition")]
>    int position
>    [MapToComponent("BehaviourComponent"), MapToProperty("Behaviours")]
>    List<BaseBehaviour> behaviours
>}
The system will check in compilation time if all the required fields of the Components are filled

## Entity Type Registration Pattern

The system supports **specialized entity types** through decorators and reflection. This allows creating entity subclasses with custom components while maintaining type safety.

### Decorators

#### `[EntityType(typeof(T))]`
Specifies which concrete entity class should be instantiated for a Definition or Descriptor.

```csharp
[EntityType(typeof(Survivor))]
public class SurvivorDefinition : GameEntityDefinition
{
    [MapToComponent(typeof(IHealthComponent), "MaxHealth")]
    public int MaxHealth { get; set; }
    
    [MapToComponent(typeof(IFactionComponent), "Faction")]
    public string Faction { get; set; }
}
```

**Priority:**
1. Descriptor's `[EntityType]` (if present)
2. Definition's `[EntityType]` (if present)  
3. Default type (`Agent` or `Prop`)

### Entity Subclasses

Entities can be extended to include custom components:

```csharp
public class Survivor : Agent
{
    public Survivor(EntityId id, string definitionId, string name, string category) 
        : base(id, name, category, definitionId)
    {
        // Survivors always have Faction component
        AddComponent(new FactionComponent());
    }
}

public class FactionComponent : IFactionComponent
{
    public string Faction { get; set; } = string.Empty;
}
```

### GenericActorFactory

The factory uses reflection to create the correct entity type:

```csharp
public Agent BuildAgent(AgentDescriptor descriptor)
{
    var definition = catalog.GetDefinition(descriptor.DefinitionID);
    
    // Determine concrete type from [EntityType] decorator
    var entityType = GetEntityType<Agent>(descriptor.GetType(), definition);
    
    // Create instance via reflection
    var agent = CreateEntityInstance<Agent>(entityType, descriptor.DefinitionID, definition);
    
    // Map properties from definition and descriptor to components
    EngineAutoMapper.Map(definition, agent);
    EngineAutoMapper.Map(descriptor, agent);
    
    return agent;
}
```

### Component Lookup by Interface

`GetComponent<T>()` supports finding components by interface or base class:

```csharp
// Registers component with concrete type
entity.AddComponent(new FactionComponent());

// Can retrieve by interface
var faction = entity.GetComponent<IFactionComponent>(); // ✅ Works!
```

**Implementation:**
```csharp
public IGameEntityComponent? GetComponent(Type componentType)
{
    // First, try direct lookup
    if (_components.TryGetValue(componentType, out var component))
        return component;
    
    // If not found, search by interface/base class
    foreach (var kvp in _components)
    {
        if (componentType.IsAssignableFrom(kvp.Key))
            return kvp.Value;
    }
    
    return null;
}
```

---

## FSM and Orchestrator Architecture

TurnForge.Engine uses a **Finite State Machine (FSM)** combined with an **Orchestrator** to control game flow and state mutations. This architecture ensures:
- **Centralized state management:** Only one place mutates state
- **Ordered execution:** Commands and decisions execute in predictable order
- **Phase control:** Game phases (setup, player turn, enemy turn, etc.)
- **Hook support:** OnPhaseStart, OnPhaseEnd for reactive logic

### Core Components

```
┌──────────────┐
│   Command    │ (External input)
└──────┬───────┘
       │
       ▼
┌──────────────────┐
│ GameEngineRuntime│ (Orchestration)
│ - Validates command
│ - Executes handler
│ - Manages FSM transitions
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│  CommandHandler  │ (Business Logic)
│ - Generates IDecision[]
│ - NO state mutation
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│  FsmController   │ (State Machine)
│ - Tracks current phase
│ - Validates transitions
│ - Executes hooks
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│  Orchestrator    │ (State Mutation)
│ - Schedules decisions
│ - Routes to appliers
│ - Updates GameState
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│    Applier       │ (Entity Operations)
│ - Creates entities
│ - Updates components
│ - Generates effects
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│  Updated State + │
│  IGameEffect[]   │
└──────────────────┘
```

---

### Command Execution Flow

When a command arrives at the engine, the following happens:

#### 1. **Command Validation** (GameEngineRuntime)

```csharp
public CommandTransaction ExecuteCommand(ICommand command)
{
    // 1. Check if we're waiting for ACK
    if (_fsmController.WaitingForACK && command is not CommandAck)
    {
        throw new Exception("Waiting for ACK command");
    }
    
    // 2. Validate command allowed in current state
    if (!_fsmController.CurrentState.IsCommandAllowed(command.GetType()))
    {
        throw new Exception($"Command {command.GetType().Name} not allowed in state {_fsmController.CurrentState.Id}");
    }
    
    // Command is valid, proceed...
}
```

**Examples:**
- ❌ Can't spawn agents during "EnemyTurn" phase
- ❌ Can't move agents during "Setup" phase
- ✅ Can spawn agents during "Initialization" phase

#### 2. **Command Execution** (CommandBus → Handler)

```csharp
// 3. Execute the command via CommandBus
var result = _commandBus.Send(command);

// Handler returns CommandResult with IDecision[]
public sealed record CommandResult
{
    public bool Success { get; init; }
    public IReadOnlyCollection<IDecision> Decisions { get; init; } = [];
    public IReadOnlyCollection<string> Tags { get; init; } = [];
}
```

**Key Point:** Handlers do NOT mutate state - they only generate decisions.

#### 3. **Decision Scheduling** (Orchestrator)

```csharp
// 4. If command successful, schedule decisions
if (result.Success)
{
    var state = _repository.LoadGameState();
    _orchestrator.SetState(state);
    
    // Enqueue decisions to scheduler
    if (result.Decisions.Any())
    {
        _orchestrator.Enqueue(result.Decisions);
        state = _orchestrator.CurrentState; // State with updated scheduler
    }
}
```

#### 4. **FSM Reaction** (FsmController.HandleCommand)

```csharp
// 5. FSM reacts to command
var stepResult = _fsmController.HandleCommand(command, state, result);

// FsmController executes scheduled decisions at appropriate times:
// - OnCommandExecutionEnd decisions execute now
// - Other decisions remain scheduled for later
```

#### 5. **State Transition Check**

```csharp
// 6. Check if transition needed
_repository.SaveGameState(stepResult.State);

if (stepResult.TransitionRequested)
{
    // Execute state transition
    var transitionResult = _fsmController.MoveForwardRequest(stepResult.State);
    
    // Transition may trigger:
    // - OnStateEnd decisions (old state)
    // - OnStateStart decisions (new state)
    
    _repository.SaveGameState(transitionResult.State);
}
```

#### 6. **ACK State**

```csharp
// 7. Set waiting for ACK
_fsmController.WaitingForACK = true;

// Game waits for CommandAck before accepting next command
// UI can animate, user can see results
```

---

### Decision Timing System

Decisions can be scheduled to execute at specific moments:

```csharp
public sealed record DecisionTiming(
    DecisionTimingWhen When,
    string? Phase,           // Which phase (null = any)
    DecisionTimingFrequency Frequency
);

public enum DecisionTimingWhen
{
    OnStateStart,            // When entering a phase
    OnStateEnd,              // When leaving a phase
    OnCommandExecutionEnd    // After command completes
}

public enum DecisionTimingFrequency
{
    Single,     // Execute once and remove
    Permanent   // Execute every time condition met
}
```

**Examples:**

```csharp
// Execute immediately after command
var decision = new SpawnDecision<AgentDescriptor>(descriptor)
{
    Timing = DecisionTiming.Immediate  // OnCommandExecutionEnd + Single
};

// Execute when entering "PlayerTurn" phase
var decision = new DrawCardDecision()
{
    Timing = new DecisionTiming(
        When: DecisionTimingWhen.OnStateStart,
        Phase: "PlayerTurn",
        Frequency: DecisionTimingFrequency.Single
    )
};

// Execute every time "EnemyTurn" ends
var decision = new SpawnZombieDecision()
{
    Timing = new DecisionTiming(
        When: DecisionTimingWhen.OnStateEnd,
        Phase: "EnemyTurn",
        Frequency: DecisionTimingFrequency.Permanent  // Every turn!
    )
};
```

---

### Orchestrator (State Mutation Controller)

The Orchestrator is the **ONLY** place where GameState mutates.

#### Applier Registration

```csharp
public class TurnForgeOrchestrator : IOrchestrator
{
    private Dictionary<Type, object> _appliers = new();
    public GameState CurrentState { get; private set; }

    // Register applier by decision type
    public void RegisterApplier<TDecision>(IApplier<TDecision> applier) 
        where TDecision : IDecision
    {
        _appliers[typeof(TDecision)] = applier;
    }
}

// Example registration at GameEngineFactory:
orchestrator.RegisterApplier(new AgentSpawnApplier(factory));
orchestrator.RegisterApplier(new MoveApplier());
orchestrator.RegisterApplier(new AttackApplier());
```

#### Decision Execution

```csharp
public IGameEffect[] Apply(IDecision decision)
{
    var decisionType = decision.GetType();
    
    // Find registered applier
    if (_appliers.TryGetValue(decisionType, out var applier))
    {
        // Dynamic dispatch: IApplier<T>.Apply(T, GameState)
        var response = (ApplierResponse)((dynamic)applier).Apply((dynamic)decision, CurrentState);
        
        // Update state (ONLY mutation point!)
        CurrentState = response.GameState;
        
        return response.GameEffects;
    }
    else
    {
        throw new InvalidOperationException($"No applier for {decisionType.Name}");
    }
}
```

**Critical Points:**
- ✅ State updates are **immutable** (`state.WithAgent()` creates new state)
- ✅ Dynamic dispatch finds correct `IApplier<TDecision>`
- ⚠️ Missing applier throws exception (register all decision types!)

#### Scheduling and Execution

```csharp
// Enqueue decisions to scheduler
public void Enqueue(IEnumerable<IDecision> decisions)
{
    CurrentState = CurrentState.WithScheduler(CurrentState.Scheduler.Add(decisions));
}

// Execute scheduled decisions for specific phase/timing
public IGameEffect[] ExecuteScheduled(string? phase, string when)
{
    // Find decisions matching phase and timing
    var toExecute = CurrentState.Scheduler.GetDecisions(d =>
        d.Timing.Phase == phase &&
        d.Timing.When == whenEnum).ToList();
    
    List<IGameEffect> allEffects = [];
    foreach (var decision in toExecute)
    {
        var effects = Apply(decision);
        
        // Remove if single-use
        if (decision.Timing.Frequency == DecisionTimingFrequency.Single)
        {
            CurrentState = CurrentState.WithScheduler(CurrentState.Scheduler.Remove(decision));
        }
        
        allEffects.AddRange(effects);
    }
    
    return allEffects.ToArray();
}
```

---

### Applier Pattern

Appliers execute decisions and mutate state:

```csharp
public interface IApplier<TDecision> where TDecision : IDecision
{
    ApplierResponse Apply(TDecision decision, GameState state);
}

public sealed record ApplierResponse(
    GameState GameState,      // Updated state
    IGameEffect[] GameEffects // Effects for logging/UI
);
```

**Example Applier:**

```csharp
public sealed class MoveApplier : IApplier<MoveDecision>
{
    public ApplierResponse Apply(MoveDecision decision, GameState state)
    {
        // 1. Get agent
        var agent = state.Agents[decision.AgentId];
        
        // 2. Update position (immutable)
        var updatedAgent = agent.WithPosition(decision.TargetPosition);
        
        // 3. Update state (immutable)
        var newState = state.WithAgent(updatedAgent);
        
        // 4. Generate effect
        var effect = new AgentMovedEffect(
            AgentId: decision.AgentId,
            From: agent.PositionComponent.CurrentPosition,
            To: decision.TargetPosition
        );
        
        return new ApplierResponse(newState, [effect]);
    }
}
```

**Rules:**
- ✅ Appliers receive **immutable** state
- ✅ Appliers return **new** state (never mutate input)
- ✅ Appliers generate **effects** (metadata for UI/logs)
- ❌ Appliers do NOT contain business logic (that's in handlers/strategies)

---

### FSM Execution Order (Critical!)

The order of execution is critical for correctness:

```
┌─────────────────────────────────────────┐
│  1. Command Validation                  │
│     - ACK check                         │
│     - State allows command?             │
└─────────────┬───────────────────────────┘
              │
              ▼
┌─────────────────────────────────────────┐
│  2. Command Execution                   │
│     - Handler generates IDecision[]     │
│     - CommandResult returned            │
└─────────────┬───────────────────────────┘
              │
              ▼
┌─────────────────────────────────────────┐
│  3. Decision Scheduling                 │
│     - Decisions added to Scheduler      │
└─────────────┬───────────────────────────┘
              │
              ▼
┌─────────────────────────────────────────┐
│  4. OnCommandExecutionEnd Decisions  ⚠️ │
│     - Execute immediately                │
│     - State updated                      │
└─────────────┬───────────────────────────┘
              │
              ▼
┌─────────────────────────────────────────┐
│  5. State Transition Check              │
│     - Should phase change?              │
└─────────────┬───────────────────────────┘
              │
          ┌───┴───┐
          │  No   │ → End
          └───────┘
              │
          ┌───┴───┐
          │  Yes  │
          └───┬───┘
              │
              ▼
┌─────────────────────────────────────────┐
│  6. OnStateEnd Decisions                │
│     - Old phase cleanup                 │
└─────────────┬───────────────────────────┘
              │
              ▼
┌─────────────────────────────────────────┐
│  7. State Transition                    │
│     - FSM moves to new phase            │
└─────────────┬───────────────────────────┘
              │
              ▼
┌─────────────────────────────────────────┐
│  8. OnStateStart Decisions              │
│     - New phase initialization          │
└─────────────────────────────────────────┘
```

**⚠️ CRITICAL BUG WARNING:**

**Correct Order:**
```
OnCommandExecutionEnd → Transition Check → OnStateEnd → Transition → OnStateStart
```

**Wrong Order (Bug):**
```
Transition → OnCommandExecutionEnd ❌
```

If OnCommandExecutionEnd executes AFTER transition, decisions will use the NEW state instead of the state when command executed!

---

### Benefits of FSM + Orchestrator Architecture

| Benefit | Description |
|---------|-------------|
| **Single Mutation Point** | Only Orchestrator mutates state → easier debugging |
| **Immutable Updates** | State changes create new instances → time travel debugging |
| **Ordered Execution** | Predictable decision timing → no race conditions |
| **Phase Control** | FSM enforces valid commands per phase → game rules |
| **Hook System** | OnPhaseStart/End for reactive logic → extensible |
| **Testability** | Handlers return data (decisions) → unit testable |
| **Effect Tracking** | All mutations generate effects → UI reactivity |
| **Centralized Logging** | All state changes flow through Orchestrator → audit trail |

### Example: Complete Turn Flow

```csharp
// Phase: "PlayerTurn"
// Available commands: Move, Attack, EndTurn

// 1. Player moves agent
engine.ExecuteCommand(new MoveCommand(agentId, targetPosition));
  → MoveHandler generates MoveDecision
  → Orchestrator applies immediately (OnCommandExecutionEnd)
  → State updated with new position
  → AgentMovedEffect generated
  → UI animates movement

// 2. Player attacks enemy
engine.ExecuteCommand(new AttackCommand(agentId, targetId));
  → AttackHandler generates AttackDecision
  → Orchestrator applies immediately
  → State updated (enemy health reduced)
  → AttackEffect generated
  → UI shows damage

// 3. Player ends turn
engine.ExecuteCommand(new EndTurnCommand());
  → EndTurnHandler generates TransitionDecision
  → Orchestrator executes OnCommandExecutionEnd decisions
  → FSM transition requested
  → OnStateEnd decisions execute (PlayerTurn cleanup)
  → FSM transitions to "EnemyTurn"
  → OnStateStart decisions execute (EnemyTurn setup)
    - Permanent decision: SpawnZombieDecision executes
    - New zombie spawned
  → State saved
  → ACK state activated
```

---

### FSM System Nodes (Game Initialization)

TurnForge defines **System Nodes** that control the game initialization flow. These nodes enforce a strict order of setup operations.

#### Game Initialization Flow

```
┌──────────────┐
│ InitialState │ (Entry point)
└──────┬───────┘
       │ Allows: InitializeBoardCommand
       │ Tag: "BoardInitialized"
       ▼
┌──────────────┐
│ BoardReady   │ (Board configured)
└──────┬───────┘
       │ Allows: SpawnPropsCommand
       │ Tag: "PropsSpawned"
       ▼
┌──────────────┐
│ GamePrepared │ (Props placed)
└──────┬───────┘
       │ Allows: SpawnAgentsCommand
       │ Tag: "AgentsSpawned"
       ▼
┌──────────────┐
│ PlayerTurn   │ (Game ready to play)
└──────────────┘
```

#### Node Definitions

##### `InitialStateNode`
**Purpose:** Entry point for game setup. Allows board initialization.

**Allowed Commands:**
- `InitializeBoardCommand`

**Transition Trigger:**
- Tag: `"BoardInitialized"`

**Code:**
```csharp
public class InitialStateNode : LeafNode
{
    public InitialStateNode()
    {
        AddAllowedCommand<InitializeBoardCommand>();
    }

    public override bool IsCommandValid(ICommand command, GameState state)
    {
        return command is InitializeBoardCommand;
    }

    public override IEnumerable<IFsmApplier> OnCommandExecuted(
        ICommand command, 
        CommandResult result, 
        out bool transitionRequested)
    {
        transitionRequested = result.Tags.Contains("BoardInitialized");
        return Enumerable.Empty<IFsmApplier>();
    }
}
```

---

##### `BoardReadyNode`
**Purpose:** Board is initialized. Ready to spawn static props.

**Allowed Commands:**
- `SpawnPropsCommand`

**Transition Trigger:**
- Tag: `"PropsSpawned"`

**Code:**
```csharp
public class BoardReadyNode : LeafNode
{
    public BoardReadyNode()
    {
        AddAllowedCommand<SpawnPropsCommand>();
    }

    public override bool IsCommandValid(ICommand command, GameState state)
    {
        return command is SpawnPropsCommand;
    }

    public override IEnumerable<IFsmApplier> OnCommandExecuted(
        ICommand command, 
        CommandResult result, 
        out bool transitionRequested)
    {
        transitionRequested = result.Tags.Contains("PropsSpawned");
        return Enumerable.Empty<IFsmApplier>();
    }
}
```

---

##### `GamePreparedNode`
**Purpose:** Props are placed. Ready to spawn dynamic agents.

**Allowed Commands:**
- `SpawnAgentsCommand`

**Transition Trigger:**
- Tag: `"AgentsSpawned"`

**Code:**
```csharp
public class GamePreparedNode : LeafNode
{
    public GamePreparedNode()
    {
        AddAllowedCommand<SpawnAgentsCommand>();
    }

    public override bool IsCommandValid(ICommand command, GameState state)
    {
        return command is SpawnAgentsCommand;
    }

    public override IEnumerable<IFsmApplier> OnCommandExecuted(
        ICommand command, 
        CommandResult result, 
        out bool transitionRequested)
    {
        transitionRequested = result.Tags.Contains("AgentsSpawned");
        return Enumerable.Empty<IFsmApplier>();
    }
}
```

---

#### Initialization Sequence Example

```csharp
// Step 1: Initialize Board
var boardDescriptor = new BoardDescriptor(spatial, zones);
var initBoardCmd = new InitializeBoardCommand(boardDescriptor);
engine.ExecuteCommand(initBoardCmd);  
// → FSM transitions: InitialState → BoardReady

// Step 2: Spawn Props
var spawnPropsCmd = new SpawnPropsCommand(propRequests);
engine.ExecuteCommand(spawnPropsCmd);  
// → FSM transitions: BoardReady → GamePrepared

// Step 3: Spawn Agents
var spawnAgentsCmd = new SpawnAgentsCommand(agentRequests);
engine.ExecuteCommand(spawnAgentsCmd);  
// → FSM transitions: GamePrepared → PlayerTurn

// Game is now ready to play!
```

---

#### Why This Order?

| Step | Why? |
|------|------|
| **1. Board First** | Spatial model must exist before placing entities |
| **2. Props Second** | Static objects (doors, spawn points) define environment |
| **3. Agents Last** | Dynamic entities need props for spawn point detection |

**Example:**
- `PartySpawnPoint` prop → defines where survivors spawn
- `ConfigurableAgentSpawnStrategy` → finds `PartySpawnPoint` → positions agents
- ❌ If agents spawn before props → no spawn point found!

---

## Spawn Strategy

Spawn is the process of creating entities in the game. It is configurable through **SpawnStrategy** implementations.

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

### 1. Spawn Command

```csharp
public record SpawnCommand(
    IReadOnlyList<SpawnRequest> Requests
);

public record SpawnRequest(
    string DefinitionId,                        // Required: entity definition
    int Count = 1,                              // Optional: batch spawn
    Position? Position = null,                  // Optional: strategy decides if null
    Dictionary<string, object>? PropertyOverrides = null  // Optional: override specific properties
);
```

**Example:**
```csharp
var command = new SpawnCommand([
    new SpawnRequest(
        DefinitionId: "Survivors.Mike",
        Count: 1,
        Position: new Position(tile),
        PropertyOverrides: new() { ["Faction"] = "Police" }
    )
]);
```

### 2. Spawn Strategy

Each strategy is **typed to a specific Descriptor**:

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

**Implementation Example:**
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
            // Expand Count (batch spawn)
            for (int i = 0; i < request.Count; i++)
            {
                // Create descriptor of strategy's associated type
                var descriptor = new SurvivorDescriptor(request.DefinitionId)
                {
                    Position = request.Position ?? FindPartySpawnPoint(state),
                    Faction = "Alliance",
                    StartingWeapon = GetMissionWeapon(state.MissionConfig)
                };
                
                // Apply property overrides if present
                if (request.PropertyOverrides != null)
                    ApplyOverrides(descriptor, request.PropertyOverrides);
                
                // Validate position
                if (!IsValidPosition(descriptor.Position, state))
                {
                    LogWarning($"Invalid spawn position for {request.DefinitionId}");
                    continue; // Skip this spawn
                }
                
                decisions.Add(new SpawnDecision<SurvivorDescriptor>(descriptor));
            }
        }
        
        return decisions;
    }
    
    private Position FindPartySpawnPoint(GameState state)
    {
        // Find props with PartySpawn behaviour
        var spawnPoint = state.GetProps()
            .FirstOrDefault(p => p.HasBehaviour<PartySpawnBehaviour>());
        
        return spawnPoint?.PositionComponent.CurrentPosition ?? Position.Empty;
    }
}
```

### 3. Spawn Decision

```csharp
public sealed record SpawnDecision<TDescriptor>(
    TDescriptor Descriptor
) : IDecision
    where TDescriptor : IGameEntityDescriptor
{
    public DecisionTiming Timing { get; init; } = DecisionTiming.Immediate;
    public string OriginId { get; init; } = "System";
}
```

### 4. Spawn Applier

The applier creates the entity using `GenericActorFactory` and adds it to the game state:

```csharp
public class SpawnApplier<TDescriptor> : IApplier<SpawnDecision<TDescriptor>>
    where TDescriptor : IGameEntityDescriptor
{
    private readonly GenericActorFactory _factory;
    
    public ApplierResponse Apply(SpawnDecision<TDescriptor> decision, GameState state)
    {
        // Create entity from descriptor
        var entity = _factory.Build(decision.Descriptor);
        
        // Add to state
        var newState = state.WithEntity(entity);
        
        // Generate effect
        var effect = new EntitySpawnedEffect(
            entity.Id,
            entity.DefinitionId,
            entity.PositionComponent.CurrentPosition
        );
        
        return new ApplierResponse(newState, [effect]);
    }
}
```

### Strategy Registration

Strategies are registered per entity category:

```csharp
// At GameEngineFactory
var survivorStrategy = new SurvivorSpawnStrategy();
var zombieStrategy = new ZombieSpawnStrategy();

services.RegisterSingleton<ISpawnStrategy>("Survivors", survivorStrategy);
services.RegisterSingleton<ISpawnStrategy>("Zombies", zombieStrategy);
```

### Benefits

✅ **Type-safe:** Each strategy knows its descriptor type  
✅ **Flexible:** Position, inventory, and other properties can be mission-specific  
✅ **Testable:** Strategies return descriptors (data), not entities  
✅ **Extensible:** Add new entity types without changing core system  
✅ **Validated:** Strategies can validate spawn conditions before creating decisions  

---

## Spawn Pipeline with FSM Integration

The new spawn system fully integrates with the FSM/Orchestrator to maintain centralized state mutation control.

### Complete Architecture

```
┌─────────────────┐
│ SpawnRequest[]  │ (Command input)
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ CommandHandler  │ (Preprocessor)
│ - BuildDescriptors
│ - Strategy.Process
│ - Strategy.ToDecisions
└────────┬────────┘
         │
         ▼
┌────────────────────┐
│ SpawnDecision<T>[] │ (IDecision)
└────────┬───────────┘
         │
         ▼
┌─────────────────┐
│ FSM/Orchestrator│
│ - Schedules decisions
│ - Routes to applier
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  SpawnApplier   │
│ - Creates entity
│ - Updates state
│ - Generates effects
└────────┬────────┘
         │
         ▼
┌─────────────────────────┐
│ Updated GameState +     │
│ EntitySpawnedEffect[]   │
└─────────────────────────┘
```

### 1. Spawn Request (Input)

The external input to the spawn system:

```csharp
public sealed record SpawnRequest(
    string DefinitionId,
    int Count = 1,
    Position? Position = null,
    Dictionary<string, object>? PropertyOverrides = null
);
```

### 2. Command Handler (Preprocessor + Strategy)

The handler does NOT mutate state - it only generates decisions:

```csharp
public sealed class SpawnAgentsCommandHandler : ICommandHandler<SpawnAgentsCommand>
{
    private readonly ISpawnStrategy<AgentDescriptor> _strategy;
    private readonly IGameCatalog _catalog;
    private readonly IGameRepository _repository;

    public CommandResult Handle(SpawnAgentsCommand command)
    {
        var gameState = _repository.LoadGameState();

        // STEP 1: PREPROCESSOR - Convert SpawnRequests to Descriptors
        var descriptors = BuildDescriptors(command.Requests);

        // STEP 2: STRATEGY - Process descriptors (filter/modify)
        var processedDescriptors = _strategy.Process(descriptors, gameState);

        // STEP 3: TO DECISIONS - Wrap descriptors in spawn decisions
        var decisions = _strategy.ToDecisions(processedDescriptors);

        // STEP 4: Return to FSM (NO state mutation here!)
        return CommandResult.Ok(
            decisions: decisions.Cast<IDecision>().ToArray(),
            tags: "AgentsSpawned"
        );
    }

    private List<AgentDescriptor> BuildDescriptors(IReadOnlyList<SpawnRequest> requests)
    {
        var descriptors = new List<AgentDescriptor>();

        foreach (var request in requests)
        {
            var definition = _catalog.GetDefinition<GameEntityDefinition>(request.DefinitionId);
            if (definition == null) continue;

            // Expand Count (batch spawn)
            for (int i = 0; i < request.Count; i++)
            {
                // DescriptorBuilder: Definition + PropertyOverrides → Descriptor
                var descriptor = DescriptorBuilder.Build<AgentDescriptor>(request, definition);
                descriptors.Add(descriptor);
            }
        }

        return descriptors;
    }
}
```

**Key Points:**
- Handler loads state (read-only)
- Handler preprocesses requests → descriptors
- Strategy filters/modifies descriptors
- Returns decisions to FSM
- **NO state mutation** in handler

### 3. Spawn Strategy (Business Logic)

The strategy provides default implementations - override only if needed:

```csharp
public interface ISpawnStrategy<TDescriptor> 
    where TDescriptor : IGameEntityBuildDescriptor
{
    /// <summary>
    /// Process descriptors (already populated from definitions).
    /// Default: Accept all without changes.
    /// </summary>
    IReadOnlyList<TDescriptor> Process(
        IReadOnlyList<TDescriptor> descriptors,
        GameState state)
    {
        return descriptors; // Default implementation
    }
    
    /// <summary>
    /// Convert descriptors to decisions.
    /// Default: Wrap each in SpawnDecision.
    /// </summary>
    IReadOnlyList<SpawnDecision<TDescriptor>> ToDecisions(
        IReadOnlyList<TDescriptor> descriptors)
    {
        return descriptors
            .Select(d => new SpawnDecision<TDescriptor>(d))
            .ToList();
    }
}
```

**Example Custom Strategy:**

```csharp
public class SurvivorSpawnStrategy : ISpawnStrategy<AgentDescriptor>
{
    public IReadOnlyList<AgentDescriptor> Process(
        IReadOnlyList<AgentDescriptor> descriptors,
        GameState state)
    {
        var filtered = new List<AgentDescriptor>();
        
        foreach (var descriptor in descriptors)
        {
            // Business logic: Validate max survivors
            if (state.GetAgents().Count >= 6)
            {
                LogWarning("Max survivors reached");
                continue;
            }
            
            // Business logic: Assign spawn position if missing
            if (descriptor.Position == null)
            {
                descriptor.Position = FindPartySpawnPoint(state);
            }
            
            filtered.Add(descriptor);
        }
        
        return filtered;
    }
    
    // ToDecisions uses default implementation
}
```

### 4. FSM Orchestrator (State Mutation Controller)

The Orchestrator schedules and applies decisions:

```csharp
public class TurnForgeOrchestrator : IOrchestrator
{
    private Dictionary<Type, object> _appliers = new();
    public GameState CurrentState { get; private set; }

    // Register appliers by decision type
    public void RegisterApplier<TDecision>(IApplier<TDecision> applier) 
        where TDecision : IDecision
    {
        _appliers[typeof(TDecision)] = applier;
    }

    // Apply a single decision
    public IGameEffect[] Apply(IDecision decision)
    {
        var decisionType = decision.GetType();
        
        if (_appliers.TryGetValue(decisionType, out var applier))
        {
            // Dynamic dispatch to IApplier<T>.Apply(T, GameState)
            var response = (ApplierResponse)((dynamic)applier).Apply((dynamic)decision, CurrentState);
            
            // Update state (ONLY place state mutates)
            CurrentState = response.GameState;
            
            return response.GameEffects;
        }
        else
        {
            throw new InvalidOperationException($"No applier for {decisionType.Name}");
        }
    }
}
```

**Registration Example:**

```csharp
// At GameEngineFactory
var orchestrator = new TurnForgeOrchestrator();
var factory = services.Resolve<GenericActorFactory>();

// Register spawn appliers
orchestrator.RegisterApplier(new AgentSpawnApplier(factory));
orchestrator.RegisterApplier(new PropSpawnApplier(factory));
```

### 5. Spawn Applier (Entity Creation)

The applier creates entities and mutates state:

```csharp
public sealed class AgentSpawnApplier : IApplier<SpawnDecision<AgentDescriptor>>
{
    private readonly GenericActorFactory _factory;
    
    public ApplierResponse Apply(SpawnDecision<AgentDescriptor> decision, GameState state)
    {
        // 1. Create agent from descriptor
        var agent = _factory.BuildAgent(decision.Descriptor);
        
        // 2. Add to state (immutable update)
        var newState = state.WithAgent(agent);
        
        // 3. Generate effect with metadata
        var effect = new EntitySpawnedEffect(
            entityId: agent.Id,
            definitionId: agent.DefinitionId,
            entityType: "Agent",
            category: agent.Category,
            position: agent.PositionComponent.CurrentPosition
        );
        
        return new ApplierResponse(newState, [effect]);
    }
}
```

### 6. EntitySpawnedEffect (Metadata)

Effects carry metadata about what happened:

```csharp
public sealed record EntitySpawnedEffect : IGameEffect
{
    public EntityId EntityId { get; init; }
    public string DefinitionId { get; init; }
    public string EntityType { get; init; }  // "Agent" | "Prop"
    public string Category { get; init; }
    public Position Position { get; init; }
}
```

---

## Complete Execution Flow

### Step-by-Step Example

```csharp
// 1. External code creates spawn request
var command = new SpawnAgentsCommand([
    new SpawnRequest(
        DefinitionId: "Survivors.Mike",
        Count: 2,
        Position: spawnTile
    )
]);

// 2. Command sent to engine
var result = engine.ExecuteCommand(command);

// 3. Handler preprocesses
//    SpawnRequest → AgentDescriptor (via DescriptorBuilder)

// 4. Strategy filters/modifies
//    - Validates max survivors
//    - Assigns positions
//    - Returns filtered descriptors

// 5. Strategy creates decisions
//    AgentDescriptor → SpawnDecision<AgentDescriptor>

// 6. Handler returns to FSM
//    CommandResult.Ok(decisions, "AgentsSpawned")

// 7. FSM schedules decisions
//    - Decisions with Timing.Immediate execute now
//    - Others scheduled for OnCommandExecutionEnd

// 8. Orchestrator applies decisions
//    For each SpawnDecision<AgentDescriptor>:
//      - Looks up AgentSpawnApplier
//      - Calls applier.Apply(decision, state)

// 9. Applier creates entity
//    - GenericActorFactory.BuildAgent(descriptor)
//    - Maps definition + descriptor → components
//    - Returns Agent entity

// 10. Applier updates state
//     - state.WithAgent(newAgent)
//     - Returns new immutable state

// 11. Applier generates effect
//     - EntitySpawnedEffect with metadata
//     - Returned to FSM

// 12. FSM updates CurrentState
//     - CurrentState = response.GameState

// 13. Effects logged/broadcast
//     - UI can react to EntitySpawnedEffect
//     - Animations triggered
//     - Logs written
```

---

## FSM Command Execution Order

Critical for correctness:

```
1. Command Validation
   ├─ FSM checks if command allowed in current state
   └─ Example: Can't spawn during combat phase

2. Command Execution
   ├─ Handler generates decisions
   └─ Returns CommandResult with IDecision[]

3. Decision Scheduling
   ├─ Decisions with Timing.Immediate → Execute now
   ├─ Decisions with Timing.OnCommandExecutionEnd → Schedule
   └─ Decisions with custom phase/timing → Schedule

4. Immediate Decisions Applied
   ├─ Orchestrator.Apply() for each immediate decision
   ├─ State updated after each applier
   └─ Effects collected

5. OnCommandExecutionEnd Decisions Applied  ⚠️
   ├─ **CRITICAL:** Applied AFTER immediate decisions
   ├─ State is already updated from step 4
   └─ Can depend on updated state

6. State Transition Check
   ├─ FSM checks if phase should change
   └─ OnPhaseStart/OnPhaseEnd hooks fire

7. Phase Transition Decisions Applied
   ├─ OnPhaseStart decisions execute
   └─ OnPhaseEnd decisions execute
```

**Bug Prevention:**
> ⚠️ **IMPORTANT:** On CommandExecutionEnd decisions must execute BEFORE state transitions, not after. Otherwise they'll use stale state.

**Correct Order:**
```
Immediate → OnCommandExecutionEnd → State Transition → OnPhaseStart
```

**Wrong Order (Bug):**
```
Immediate → State Transition → OnCommandExecutionEnd ❌
```

---

## Spawn System Benefits

### Separation of Concerns

| Component | Responsibility |
|-----------|----------------|
| **SpawnRequest** | External input (data) |
| **CommandHandler** | Preprocessing + orchestration |
| **DescriptorBuilder** | Definition + overrides → Descriptor |
| **Strategy** | Business logic (filter/validate) |
| **SpawnDecision** | Scheduled work (data) |
| **FSM/Orchestrator** | State mutation control |
| **SpawnApplier** | Entity creation + state update |
| **GenericActorFactory** | Entity instantiation |

### Flexibility

✅ **Default behavior:** No strategy needed - system auto-creates descriptors  
✅ **Custom logic:** Override `Process()` for filtering/validation  
✅ **Type-safe:** Each strategy typed to its descriptor  
✅ **Testable:** Strategies return data, not entities  
✅ **Extensible:** Add entity types without changing core  

### State Control

✅ **Single mutation point:** Only Orchestrator mutates state  
✅ **Immutable updates:** `state.WithAgent()` creates new state  
✅ **Ordered execution:** FSM controls decision timing  
✅ **Effect tracking:** Metadata in `EntitySpawnedEffect`  
✅ **Hooks support:** OnPhaseStart/OnPhaseEnd integrate cleanly  

---

## COMMANDS

TurnForge provides commands as the **primary interface** for game logic execution. Commands represent player or system actions that trigger state changes through the FSM→Handler→Orchestrator pipeline.

### Command Architecture

```
┌──────────────┐
│   Command    │  (Input - What to do)
└──────┬───────┘
       │
       ▼
┌──────────────┐
│   Handler    │  (Logic - How to do it)
│ Returns:     │
│ - Success    │
│ - Decisions  │
└──────┬───────┘
       │
       ▼
┌──────────────┐
│ Orchestrator │  (Execution - Apply changes)
│ - Applies    │
│   Decisions  │
│ - Updates    │
│   State      │
└──────────────┘
```

All commands implement `ICommand`:

```csharp
public interface ICommand
{
    Type CommandType { get; }
}
```

### Available Commands

#### `CommandAck`
**Purpose:** Acknowledges the completion of a command transaction, allowing the next command to be processed.

**Usage:**
```csharp
engine.ExecuteCommand(new CommandAck());
```

**Flow:**
1. Game executes a command
2. Sets `WaitingForACK = true`
3. UI displays results/animations
4. User sends `CommandAck`
5. System resets and accepts next command

**Rationale:** Prevents command spam and allows UI to animate/display results before accepting new input.

---

#### `SpawnAgentsCommand`  
**Purpose:** Spawns agents (playable/AI characters) using the spawn pipeline.

**Parameters:**
- `IReadOnlyList<SpawnRequest> Requests` - List of agent spawn requests

**SpawnRequest Structure:**
```csharp
public record SpawnRequest(
    string DefinitionId,                               // Required: "Survivors.Mike"
    int Count = 1,                                     // Optional: batch spawn
    Position? Position = null,                         // Optional: null = strategy decides
    Dictionary<string, object>? PropertyOverrides = null,  // Optional: override descriptor properties
    IEnumerable<IGameEntityComponent>? ExtraComponents = null  // Optional: inject custom components/behaviors
);
```

**Usage:**
```csharp
var command = new SpawnAgentsCommand([
    new SpawnRequest(
        DefinitionId: "Survivors.Mike",
        Count: 1,
        Position: new Position(spawnTile),
        PropertyOverrides: new() { ["Faction"] = "Police" },
        ExtraComponents: [new StealthBehaviour()]
    )
]);

var result = engine.ExecuteCommand(command);
```

**Pipeline:**
1. Handler receives `SpawnRequest[]`
2. `DescriptorBuilder` creates `AgentDescriptor` from definition + request
3. Strategy validates and processes descriptors → `SpawnDecision<AgentDescriptor>[]`
4. Applier creates entities using `GenericActorFactory`
5. State updated with new agents

**Flexibility:**
- ✅ Position can be null → strategy decides spawn point
- ✅ PropertyOverrides customize specific descriptor properties
- ✅ ExtraComponents inject dynamic behaviors from mission files
- ✅ Count enables batch spawn (e.g., spawn 5 zombies at once)

---

#### `InitializeBoardCommand`  
**Purpose:** Initializes the game board with spatial model and zones. This is the first command executed during game setup, creating the foundational playing field before any entities spawn.

**Parameters:**
- `BoardDescriptor Descriptor` - Board configuration (spatial model + zones)

**Usage:**
```csharp
var boardDescriptor = new BoardDescriptor(
    Spatial: new SpatialDescriptor(GridType.Hex, Width: 10, Height: 10),
    Zones: [
        new ZoneDescriptor(
            Id: new ZoneId("spawn-zone"),
            Bound: new RectangleBound(x: 0, y: 0, width: 3, height: 3),
            Behaviours: [new SafeZoneBehaviour()]
        ),
        new ZoneDescriptor(
            Id: new ZoneId("danger-zone"),
            Bound: new RectangleBound(x: 7, y: 7, width: 3, height: 3),
            Behaviours: [new DangerousZoneBehaviour()]
        )
    ]
);

var command = new InitializeBoardCommand(boardDescriptor);
var result = engine.ExecuteCommand(command);
```

**Pipeline:**
1. Handler receives `BoardDescriptor`
2. `BoardFactory` creates `GameBoard` with `ISpatialModel` and `Zone[]`
3. Decision carries the constructed board → `InitializeBoardDecision(board)`
4. Applier adds board to state → `state.WithBoard(board)`
5. Returns `BoardInitializedEffect` with metadata (zone count, spatial type)

**Key Characteristics:**
- ✅ **Singleton operation**: Only executed once per game
- ✅ **Zones have behaviors**: Zones are `GameEntity` with `BehaviourComponent` (e.g., Dangerous, Indoor)
- ✅ **FSM integration**: Triggers transition from `InitialState` → `BoardReady`
- ✅ **Effect generation**: UI can react to `BoardInitializedEffect`

**Typical Flow:**
```
1. InitializeBoardCommand → Board created
2. SpawnPropsCommand → Props placed on board
3. SpawnAgentsCommand → Agents enter board
```

---

#### `SpawnPropsCommand`  
**Purpose:** Spawns props (static/interactive objects) using the spawn pipeline.

**Parameters:**
- `IReadOnlyList<SpawnRequest> Requests` - List of prop spawn requests

**Usage:**
```csharp
var command = new SpawnPropsCommand([
    new SpawnRequest(
        DefinitionId: "Props.Door",
        Position: new Position(doorTile),
        PropertyOverrides: new() { ["IsLocked"] = true }
    )
]);
```

**Similar to `SpawnAgentsCommand`** but creates `PropDescriptor` and uses `PropSpawnStrategy`.

---

### Command Best Practices

| ✅ Do | ❌ Don't |
|-------|----------|
| Use commands for all state changes | Directly mutate GameState |
| Let handlers generate decisions | Put mutation logic in handlers |
| Return CommandResult with decisions | Return entities or modified state |
| Validate in handler before decisions | Validate in applier |
| Use tags for FSM reactions | Use magic strings |

---

## SERVICES

TurnForge provides **non-command services** for **read-only operations** that don't modify game state. These services enable queries and information retrieval without triggering the command pipeline.

### Game Catalog API

The **Game Catalog** is the central registry for entity definitions. It stores blueprints that define entity properties and serves them during spawn/creation.

#### Interface: `IGameCatalogApi`

```csharp
public interface IGameCatalogApi
{
    void RegisterDefinition<T>(string definitionId, T definition) 
        where T : GameEntityDefinition;
    
    T GetDefinition<T>(string definitionId) 
        where T : GameEntityDefinition;
    
    bool TryGetDefinition<T>(string definitionId, out T? definition) 
        where T : GameEntityDefinition;
    
    IEnumerable<T> GetAllDefinitions<T>() 
        where T : GameEntityDefinition;
}
```

---

#### `RegisterDefinition<T>(definitionId, definition)`

**Purpose:** Registers an entity definition in the catalog.

**Usage:**
```csharp
var mikeDef = new SurvivorDefinition
{
    DefinitionId = "Survivors.Mike",
    Name = "Mike",
    Category = "Survivor",
    MaxHealth = 10,
    Faction = "Alliance"
};

catalog.RegisterDefinition("Survivors.Mike", mikeDef);
```

**Rules:**
- ✅ DefinitionId must be unique
- ✅ Called during game setup/initialization
- ❌ Cannot register duplicate IDs (throws exception)

---

#### `GetDefinition<T>(definitionId)`

**Purpose:** Retrieves a specific definition by ID.

**Usage:**
```csharp
var mikeDef = catalog.GetDefinition<SurvivorDefinition>("Survivors.Mike");

Console.WriteLine($"Name: {mikeDef.Name}, Health: {mikeDef.MaxHealth}");
```

**Behavior:**
- Returns `T` if found
- Throws exception if not found
- Type-safe: ensures returned type matches `T`

**Use Cases:**
- Spawn pipeline: `DescriptorBuilder` fetches definition
- UI: Display definition properties in character selection
- Validation: Check if definition exists before spawn

---

#### `TryGetDefinition<T>(definitionId, out definition)`

**Purpose:** Safe retrieval that doesn't throw exceptions.

**Usage:**
```csharp
if (catalog.TryGetDefinition<SurvivorDefinition>("Survivors.Unknown", out var def))
{
    // Found
    Console.WriteLine(def.Name);
}
else
{
    // Not found - handle gracefully
    Console.WriteLine("Definition not found");
}
```

**Benefits:**
- ✅ No exceptions
- ✅ Clean error handling
- ✅ Useful for optional lookups

---

#### `GetAllDefinitions<T>()`

**Purpose:** Retrieves all definitions of a specific type.

**Usage:**
```csharp
// Get all survivor definitions
var allSurvivors = catalog.GetAllDefinitions<SurvivorDefinition>();

foreach (var survivor in allSurvivors)
{
    Console.WriteLine($"Available: {survivor.Name} (HP: {survivor.MaxHealth})");
}
```

**Filtering Example:**
```csharp
// Get all survivors with high health
var tankSurvivors = catalog.GetAllDefinitions<SurvivorDefinition>()
    .Where(s => s.MaxHealth >= 8)
    .ToList();
```

**Use Cases:**
- **Character Selection UI:** Display all available survivors
- **Balance Analysis:** Analyze distribution of entity stats
- **Mission Generation:** Pick random enemies from pool
- **Validation:** Ensure catalog has required definitions

**Performance:**
- Filters in-memory registry (fast)
- Returns `IEnumerable<T>` (supports LINQ)
- No database/IO overhead

---

### Service vs Command

| Service (Read) | Command (Write) |
|----------------|-----------------|
| Queries data | Mutates state |
| No FSM interaction | Goes through FSM |
| Instant response | Asynchronous decisions |
| No ACK required | Blocks until ACK |
| Examples: GetDefinition, GetAllDefinitions | Examples: SpawnAgentsCommand, StartGameCommand |

**When to use Services:**
- 🔍 Displaying UI (character stats, available items)
- 🔍 Validation (check if definition exists)
- 🔍 Analysis (count entities, filter by criteria)

**When to use Commands:**
- ✏️ Creating entities
- ✏️ Updating components
- ✏️ Triggering game events

---

### Example: Character Selection Flow

```csharp
// 1. SERVICE: Get available survivors for UI
var survivors = catalog.GetAllDefinitions<SurvivorDefinition>();

// Display to user
foreach (var s in survivors)
{
    Console.WriteLine($"{s.DefinitionId}: {s.Name} (HP {s.MaxHealth})");
}

// 2. User selects "Survivors.Mike" and "Survivors.Amy"

// 3. SERVICE: Validate selections exist
if (!catalog.TryGetDefinition<SurvivorDefinition>("Survivors.Mike", out _))
{
    Console.WriteLine("Invalid selection!");
    return;
}

// 4. COMMAND: Spawn selected survivors
var spawnRequests = new SpawnRequest[] {
    new("Survivors.Mike"),
    new("Survivors.Amy")
};

var command = new StartGameCommand(spawnRequests);
var result = engine.ExecuteCommand(command);

// 5. SERVICE: Confirm spawns (read current state)
Console.WriteLine($"Game started with {result.Decisions.Count} agents");
```

---

### Catalog Internal Architecture

```
┌─────────────────────────────────────────┐
│         IGameCatalogApi                 │  (Public Interface)
│  - RegisterDefinition<T>                │
│  - GetDefinition<T>                     │
│  - TryGetDefinition<T>                  │
│  - GetAllDefinitions<T>                 │
└───────────────┬─────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────┐
│         GameCatalogApi                  │  (Implementation)
│  Delegates to IGameCatalog              │
└───────────────┬─────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────┐
│         IGameCatalog                    │  (Core Interface)
│  Manages definition storage              │
└───────────────┬─────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────┐
│      InMemoryGameCatalog                │  (Concrete Storage)
│  - Dictionary<string, GameEntityDef>    │
│  - Type filtering                       │
│  - Thread-safe (if needed)              │
└─────────────────────────────────────────┘
```

**Rationale:**
- `IGameCatalogApi`: Public contract for games
- `GameCatalogApi`: Simple delegation layer
- `IGameCatalog`: Internal abstraction
- `InMemoryGameCatalog`: Fast in-memory storage (can be replaced with DB/file-based catalog)

---

### Future Services (Planned)

| Service | Description |
|---------|-------------|
| **IGameStateQueryApi** | Read current game state (agents, props, board) |
| **IPathfindingService** | Calculate paths between positions |
| **IVisionService** | Calculate line-of-sight, visible tiles |
| **IInventoryQueryApi** | Query agent inventories |
| **IMissionLoaderService** | Load mission definitions from files |

---


