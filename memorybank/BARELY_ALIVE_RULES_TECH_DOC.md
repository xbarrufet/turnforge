# BarelyAlive.Rules Architecture

This document details the file structure and logic of the **BarelyAlive** rules engine. The system is separated into strict layers to ensure that the Engine (logic) and the UI (visualization) are decoupled.

## 📁 Folder Structure

> [!NOTE]
> **UI Integration**: This project acts as an intermediary between Godot and the Engine.
> 1. Receives requests from the UI via `GodotAdapter`.
> 2. Translates Engine responses to `GameUpdatePayload`.
> 3. The UI maintains a simplified "UI Model" that is only updated by processing these payloads.

```text
src/BarelyAlive.Rules/
├── Game/                          # Operational Core (Runtime) (BarelyAliveGame, GameBootstrap)
├── Apis/                          # Contracts and external entry point
│   ├── Handlers/                  # Use case implementation (1 per Command)
│   ├── Messaging/                 # Output DTOs to the UI (GameResponse, payloads)
│   ├── ViewModels/                # Full state projections for queries
│   └── Interfaces/                # API Service Definition (IBarelyAliveApis)
├── Core/                          # Pure business logic
│   ├── Domain/                    # The Zombicide world (Engine Agnostic)
│   │   ├── Entities/              # Domain Classes
│   │   ├── Projectors/            # Translators: Engine -> Messaging/ViewModels
│   │   │   ├── Handlers/          # Specific Projectors (AgentSpawned, etc.)
│   │   │   └── Interfaces/        # IEffectProjector
│   │   ├── ValueObjects/          # Basic Data Structs (Vector)
│   │   └── Descriptors/           # Input Configuration (IDescriptor)
│   ├── Engine/                    # TurnForge Specific Extensions
│   └── Strategies/                # Business Rules (Combat, AI, Spawn)
├── Adapter/                       # Infrastructure and data input
│   ├── Loaders/                   # File Readers (JSON)
│   ├── Mappers/                   # Conversion from JSON DTOs to Descriptors
│   └── Dto/                       # Structures reflecting the JSON file
└── Assets/                        # Data Files (Missions, Config)
```

---

## 🔄 Call Flow: From UI to Engine

Below is the complete lifecycle of an interaction with the system.

### Example: `InitializeGame(missionJson)`

| Step | Component | Action | Responsibility |
| :--- | :--- | :--- | :--- |
| **1** | **UI (Godot/Client)** | Calls `BarelyAliveApis.InitializeGame(json)` | Initiates interaction. Knows nothing of the Engine, only the API. |
| **2** | **API Facade** | Delegates to `InitializeGameHandler.Handle(json)` | Single entry point. Routes the request to the appropriate handler. |
| **3** | **Handler** | 1. Parses JSON (via `MissionLoader`).<br>2. Creates `InitGameCommand` (Engine).<br>3. Calls `_gameEngine.ExecuteCommand()`. | Orchestration. Converts external data into internal Engine commands. |
| **4** | **TurnForge Engine** | Executes logic: creates entities (Board, Zones, Props, Agents). Returns `CommandTransaction`.<br>**Generated Effects**:<br>- `BoardApplierResult` (Board created)<br>- `PropSpawnedEffect` (Props created)<br>- `AgentSpawnedResult` (Agents created) | Pure state logic. Knows nothing of projections or UI. |
| **5** | **Handler** | Receives `Transaction`. Calls `DomainProjector.CreatePayload(transaction)`. | Synchronization point. Decides that the response needs translation for the client. |
| **6** | **DomainProjector** | Iterates over `transaction.Effects`. Searches for an `IEffectProjector` for each effect.<br>- `PropSpawnedProjector`: Activated.<br>- `AgentSpawnedProjector`: Activated.<br>- *BoardApplierResult*: Ignored (UI already has JSON definition). | Dispatcher. Routes each engine effect to its visual translator. |
| **7** | **IEffectProjectors** | Translates `IGameEffect` -> `EntityBuildUpdate` (DTO). | Translation. Converts "ID 5 created at (0,0)" to "Draw a 'Survivor' at (0,0)". |
| **8** | **Handler** | Constructs `GameResponse` with the generated payload. | Packaging. Prepares the standardized final response. |
| **9** | **UI (GodotAdapter)** | Receives `GameResponse`. Emits Signals to update local "UI Model". | Bridge. Decouples business logic from rendering. |
| **10** | **UI (Presenters)** | Listen to Signals and instantiate visual nodes (Agents and Props). | Rendering. Updates visual scene to reflect new state. |

---

## 📦 Call Payloads

This section serves as a reference for **mocking** responses when developing the UI without the Engine connected.

### Base Object: `GameResponse`
All calls return this structure:
```json
{
  "TransactionId": "guid-uuid-string",
  "Success": true,
  "Error": null,
  "Payload": { ... } // See details below
}
```

### 1. `InitializeGame` / `LoadGame`
**Payload Type**: `GameUpdatePayload`
**Description**: List containing all initial dynamic entities (Agents and Props). The Board (Map) is not sent as it is static relative to the mission JSON.

```json
{
  "Created": [
    {
      "EntityId": "101",
      "Type": "Survivor",  // Mapped from AgentType
      "DefinitionId": "Amy",
      "Position": { "X": 2, "Y": 5 },
      "State": {}
    },
    {
      "EntityId": "202",
      "Type": "Prop",      // Mapped from PropType
      "DefinitionId": "Door",
      "Position": { "X": 3, "Y": 5 },
      "State": {}
    }
  ],
  "Updated": [],
  "Events": []
}
```

### 2. `MoveAgent` (Future Example)
**Payload Type**: `GameUpdatePayload`
**Description**: State update of an existing entity.

```json
{
  "Created": [],
  "Updated": [
    {
      "EntityId": "101",
      "Component": "Position",
      "NewValue": { "X": 3, "Y": 5 },
      "Delta": null
    },
    {
      "EntityId": "101",
      "Component": "ActionPoints",
      "NewValue": 2,
      "Delta": -1
    }
  ],
  "Events": []
}
```

---

## 🏷️ Enums and Responsibilities

These enums define the shared vocabulary between Rules and UI.

| Enum | Location | Responsibility | Example Values |
| :--- | :--- | :--- | :--- |
| **Pending** | `Core/Domain/Entities` | (Pending implementation) Define factions. | `Survivor`, `Zombie`, `Neutral` |
| **Pending** | `Core/Domain/Entities` | (Pending implementation) Game states. | `PlayerTurn`, `EnemyTurn`, `Victory`, `Defeat` |

*(Note: Currently most types are managed as strings or `ValueObjects` in the refactored code. This section will be expanded as Enums are formalized in the Domain.)*

---

## 🛠️ Services & Queries

Beyond the main game loop, the system provides auxiliary services for UI meta-data.

### 1. `GetAvailableSurvivors()`

**Purpose**: Allows the UI to query the Engine's Registry for all available character definitions (Agents) to populate selection screens.

**Call Flow**:
1. **UI**: Calls `GodotAdapter.GetAvailableSurvivors()`.
2. **Adapter**: Calls `BarelyAliveApis.GetAvailableSurvivors()`.
3. **Facade**: Creates `GetRegisteredSurvivorsQuery`.
4. **Handler**: `GetSurvivorsHandler` executes.
    - Gets all definitions from `GameCatalogApi.GetRegisteredAgents()`.
    - Maps `AgentDefinition` (Engine) to `SurvivorDefinition` (Rule DTO).
5. **Returns**: `List<SurvivorDefinition>`.

**Return Payload (SurvivorDefinition)**:
```csharp
public sealed record SurvivorDefinition(
    string Id,          // Unique ID (e.g. "Amy")
    string Name,        // Display Name
    string Description, // Fluff text
    int MaxHealth,      // Base Stats
    int MaxMovement     // Base Stats
);
```