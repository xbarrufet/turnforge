# 📘 RULES FORGE TECHNICAL DOCUMENTATION

**TurnForge** is a deterministic game engine designed for **Skirmish-style board games** (e.g., XCOM, HeroQuest, D&D Tactics). It decouples game logic (Rules) from visual representation (UI/Godot), ensuring a clean separation of concerns where the simulation is the single source of truth.

---

## 1. High-Level Architecture

TurnForge follows a **CBS (Component-Behaviour-Strategy)** architecture wrapped in a strict **Command-Query Separation (CQS)** flow.

### Core Philosophy
1.  **Deterministic**: The same inputs (Commands + Seed) always produce the same outputs.
2.  **Decoupled**: The Engine knows nothing about the UI. It speaks through specific Output Interfaces (`IEffectSink`, `GameUpdatePayload`).
3.  **Atomic**: State mutations happen only via strictly controlled **Appliers** orchestrated by a central pipeline.
◊
### Integration Pattern (Recommended)
While the Engine is UI-agnostic, we recommend the following integration pattern:
*   **Adapter Layer**: A dedicated project (e.g., `GodotAdapter`) that translates UI inputs to Engine Commands.
*   **UI Model**: The UI should maintain a lightweight, visual-only model that subscribes to `GameUpdatePayload` signals.
*   **Unidirectional**: Never modify the UI Model directly; always go through the Engine.

### The CBS Pattern
*   **Components (Data)**: Pure data containers (e.g., `HealthComponent`, `PositionComponent`). They hold the *State*.
*   **Behaviours (Metadata)**: Tags or logic modifiers attached to entities (e.g., `Fly`, `Aggressive`). They influence *Strategies*.
*   **Strategies (Logic)**: The "Brain". They accept Context + Inputs and produce **Decisions**. They do *not* mutate state directly.

### How TurnForge is expected to be used

TurnForge is built around a **strict separation of concerns** between presentation, game rules, and execution.

![TurnForge Architecture](..docs/architecture.svg)

### Conceptual Model


---

## 2. Architecture & Data Flow

The engine operates in a unidirectional cycle:

```mermaid
graph TD
    UI[Client / UI] -->|1. Command| Bus[Command Bus]
    Bus -->|2. Validate| Handler[CommandHandler]
    Handler -->|3. Calc (Strategy)| Strat[Strategy]
    Strat -->|4. Decision| orch[Orchestrator]
    orch -->|5. Apply| Applier[Applier]
    Applier -->|6. Mutate| State[GameState]
    Applier -->|7. Event| Projector[Projector]
    Projector -->|8. Update| UI
```

### 1. Command Layer (API)
The central entry point to the engine is `IGameEngine.ExecuteCommand(ICommand)`. This interface handles all state mutations safely.
*   **`InitGameCommand`**: Creates the Static World (Board, Zones, Props).
*   **`StartGameCommand`**: Spawns Dynamic Actors (Agents, NPCs) and starts the FSM.
*   **Gameplay Commands**: `MoveUnit`, `Attack`, `EndTurn`.

### 2. Orchestrator (The Execution Heart)
The **Orchestrator** is responsible for executing the **Decisions** produced by Strategies. It acts as a transaction manager using **Appliers**.
*   **Registry**: Maps every `DecisionType` to a specific `IApplier`.
*   **Scheduler**: Manages delayed or phase-dependent decisions (e.g., "Apply Poison `OnTurnStart`").
*   **CommandTransaction**: Bundles all side effects of a command into a single atomic execution unit.

### 3. Appliers (State Mutators)
Appliers are the **only** components allowed to write to `GameState`.
*   **`BuildApplier<T>`**: Handles Entity Creation (e.g., Spawning a Prop).
*   **`UpdateApplier<T>`**: Handles Component modifications (e.g., Reducing Health).

### 4. Finite State Machine (FSM)
Controls the macro-flow of the game (e.g., `DeploymentPhase` -> `PlayerTurn` -> `EnemyTurn` -> `Victory`).
*   The FSM dictates which **Command Handlers** are active.
*   Transitions trigger the **Scheduler** to process pending decisions (e.g., "End of Turn" effects).

---

## 3. Core Components Reference

### Entities (`GameEntity`)
The base unit of the simulation.
*   **`Agent`**: Active units (Players, Enemies). Defined by `AgentDefinition` + Dynamic Behaviours.
*   **`Prop`**: Passive/Interactive objects (Crates, Doors). Defined by `PropDefinition`.
*   **`Board/Zone`**: The spatial graph or grid defining movement rules and connectivity.

### Behaviours
Modular logic pieces attached to entities.
*   **Implementation**: `IActorBehaviour`.
*   **Usage**: injected via `IActorFactory`.
*   **Example**: An Agent might have a base `WalkBehaviour`, but picking up a "Jetpack" item adds a `FlyBehaviour` that overrides movement logic.

### Message & DTO System (`BarelyAlive.Rules`)
The bridge between the Engine and the UI.
*   **`GameUpdatePayload`**: A snapshot of changes sent to the UI after every command.
    *   **`Created`** (`EntityBuildUpdate`): New things to instantiate visually.
    *   **`Updated`** (`EntityStateUpdate`): Changes to existing things (HP bar down, Position change).
    *   **`Events`** (`DomainEvent`): One-shot audiovisual cues ("Hit", "LevelUp", "Explosion").

---

## 4. Implementation Guide: Creating a Game

To build a game using TurnForge (e.g., **BarelyAlive**), you typically follow these steps:

### Step 1: Define the World
Create your **Definitions** and **strategies** for how the world is populated.
*   Implement `IAgentSpawnStrategy`: logic for where players/enemies appear.
*   Implement `IPropSpawnStrategy`: logic for map hazards/covers.

### Step 2: Configure the Engine Context
Usage of `GameEngineFactory` to wire up dependencies.

```csharp
var context = new GameEngineContext(
    new InMemoryGameRepository(), // or your database impl
    new MyCustomAgentSpawnStrategy(),
    new MyCustomPropSpawnStrategy()
);

IGameEngine engine = GameEngineFactory.Build(context);
```

### Step 3: The Lifecycle Hook
1.  **Bootstrap**: Call `engine.Execute(new InitGameCommand(...))`.
    *   *Result*: The Board is built. Zones are visible.
2.  **Start Loop**: Call `engine.Execute(new StartGameCommand(...))`.
    *   *Result*: Agents spawn. The FSM enters the first state (`GameStart`).
3.  **Game Loop**:
    *   Listen for UI input.
    *   Send `MoveCommand`, `AttackCommand`, etc.
    *   Process the `GameUpdatePayload` (from `CommandTransaction`) to update the UI.

### Step 4: Custom Rules
Extend the logic by implementing custom **Strategies** and **Behaviours**.
*   *Example*: Creating a "Sniper" class.
    *   Create `SniperBehaviour`.
    *   Create `SniperAttackStrategy` that checks for `SniperBehaviour` implies infinite range but high AP cost.
    *   Register Strategy in the DI container.

---

## 5. Framework Services

TurnForge provides built-in services to access static game data and metadata.

### Registry Access Service
**Purpose**: Allow external systems (UI, Tools) to query the available game content (Agents, Props) without instantiating them.

*   **Interface**: `TurnForge.Engine.Services.Interfaces.IGameCatalogApi`
*   **Method**: `GetRegisteredAgents()` / `GetRegisteredProps()`
*   **Usage**: Inject `IGameCatalogApi` into your application adapters to populate "Select Character" screens or "Map Editors".

---

## 6. Directory Structure Overview

*   **`TurnForge.Engine`**: The core framework. FSM, Orchestrator, Base Entities.
*   **`BarelyAlive.Rules`**: The implementation of your specific game.
    *   `Adapter/Dto`: Data definitions for IO.
    *   `Apis/Messaging`: The output event definitions (`EntityStateUpdate`, etc.).
    *   `Core/Domain`: Your game-specific logic (Strategies, Projectors).
    *   `Game`: The entry point and configuration for your ruleset.

---

## 7. Engine Internals & Bootstrap

This section details the lifecycle of the Engine initialization, specifically for `BarelyAliveGame`.

### 7.1. BarelyAliveGame Initialization
The entry point is `BarelyAliveGame.CreateNewGame(IGameLogger logger)`. This static factory method performs three critical steps:

1.  **Context Construction** (`GameBootstrap`):
    *   Creates `GameEngineContext` with:
        *   `InMemoryGameRepository`: Holds the runtime state (`GameState`).
        *   `ConfigurablePropSpawnStrategy`: Defines how props are placed.
        *   `ConfigurableAgentSpawnStrategy`: Defines where agents spawn (finding `PartySpawn`/`ZombieSpawn` props).
        *   `IGameLogger`: Injected logger (Console or Godot).
2.  **Engine Build** (`GameEngineFactory`):
    *   Constructs `GameEngineRuntime` using the context.
    *   Wires up the `Orchestrator`, `CommandBus`, and `GameCatalogApi`.
3.  **FSM Configuration**:
    *   Calls `BarelyAliveGameFlow.CreateController()` (see section 7.3).
    *   Sets the controller on the runtime via `SetFsmController`.
4.  **Definition Registration**:
    *   Parses `Units_BarelyAlive.json` and registers all `AgentDefinition` and `PropDefinition` into the `GameCatalog` (see section 7.2).

### 7.2. Entity Registration Process
Entities are not hardcoded but loaded from data.
1.  **JSON Loading**: `BarelyAliveGame` reads `Units_BarelyAlive.json`.
2.  **DTO Mapping**:
    *   `AgentDto` -> `AgentDefinition`: Maps Name, Category, Health, Movement.
    *   `PropDto` -> `PropDefinition`: Maps TypeId, Health.
3.  **Catalog Registration**:
    *   `_turnForge.GameCatalog.RegisterAgentDefinition(...)`
    *   `_turnForge.GameCatalog.RegisterPropDefinition(...)`
    This allows `InitGameCommand` and `StartGameCommand` to refer to entities by string IDs ("Amy", "Walker", "BarelyAlive.Spawn") without knowing their logic.

### 7.3. FSM Creation (`BarelyAliveGameFlow`)
The game enforces a strict sequence of initial phases using a `GameFlowBuilder` which wraps the user definitions in a System structure.

**Hierarchy**:
*   **SystemRoot** (Hidden Branch)
    *   `InitialState` (System Node) -> Handles `InitGameCommand`
    *   `GamePrepared` (System Node) -> Handles `StartGameCommand`
    *   **Root** (User Branch)
        *   `Gameplay` (Leaf) -> Handles User Commands

**Initialization Logic**:
1.  The `BarelyAliveGameFlow.CreateController()` builds this tree, defining only the `Gameplay` node.
2.  The `FsmController` automatically descends from `SystemRoot` to the first leaf: `InitialState`.

**Mandatory Flow**:
1.  **State: InitialState** (System)
    *   **Allowed Command**: `InitGameCommand`.
    *   **Action**: Loads Map, Zones, and Props.
    *   **Transition**: On success + ACK, auto-advances to `GamePrepared`.
2.  **State: GamePrepared** (System)
    *   **Allowed Command**: `StartGameCommand`.
    *   **Action**: Spawns Agents based on strategies.
    *   **Transition**: On success, auto-advances to `Gameplay` (via `Root`).
3.  **State: Gameplay** (User)
    *   **Allowed Commands**: `MoveCommand`, `AttackCommand`, etc.
    *   **Action**: Main game loop defined by the user.

This structure allows the Engine to enforce initialization logic transparently before handing control over to the game-specific rules.
