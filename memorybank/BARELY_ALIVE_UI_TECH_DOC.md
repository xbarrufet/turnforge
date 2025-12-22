# BarelyAlive UI Technical Documentation

This document describes the architecture of the **BarelyAlive.Godot** project, which serves as the visual frontend for the TurnForge Engine.

## 1. High-Level Architecture

The UI project follows a **Passive View** pattern where possible, with a central **Adapter** managing communication with the `TurnForge.Engine`.

### 1,1 Components
- BarelyAlive.Godot: contains the Godot specific code, maintainas a UI Model of the data that is used to render the UI.
- BarelyAlive.Rules: contains the business logic as TurnForge Engine customization and manages the communication with the TurnForge Engine.
- TurnForge.Engine: Turn Base Games engine that handles the game logic and acts as a domain model source of truth.

### Considerations
1) BarelyAlive.Godot-BarelyAlive.Rules communication is centrallized in a class called GodotAdapter
2) The Source of truth is the TurnForge.Engine, BarelyAlive.Godot maintains a local copy of the Model, only the parts needed to render the UI. This is done to avoid the need to send the entire Model to the UI.
4) The updates of the UI Model are only triggered by receiving the response from the TurnForge.Engine trough BAreluAlive.Rules->GodotAdapter
5) The GodotAdapter is a singleton, it is initialized in the BarelyAliveBootstrap and is used by the entire application.
6) The GodotAdapter emits signals to update the UI Model, the UI is updated by connecting to these signals.


The communication flow is as follows:
1- UI generate a Command request to BAreluAlive.Rules that translate it to a TurnForge Command
2- BAreluAlive.Rules send the Command to the TurnForge Engine
3- TurnForge Engine process the Command and return a Transaction
4- BArelyAlive.Rules translate the Transaction to a GameUpdatePayload using DomainProjectors and send it to the UI
5- The GodotAdapter gets the payload an emits the required singal to udoate "the UI Model" update the UI based on the Transaction



### Key Components

*   **`BarelyAliveBootstrap`**: The composition root. Initializes the Engine Context and injects dependencies (`InMemoryGameRepository`, `BarelyAliveActorFactory`).
*   **`GodotAdapter` (Singleton)**: The bridge. Holds the `IGameEngine` instance and exposes methods to start/load missions.
*   **`GameContext` (Singleton Node)**: Holds **Visual State** that spans across scenes or updates (e.g., current Map Texture, Mission Name, Coordinate Conversions).
*   **`MapPresenter`**: A controller responsible for rendering the static board background and handling aspect ratio scaling.

## 2. Directory Structure

```text
src/BarelyAlive.Godot/
├── Scenes/               # Godot .tscn files (Visuals)
├── src/
│   ├── Infrastructure/   # Bootstrapping & Engine Integration
│   │   ├── BarelyAliveBootstrap.cs
│   │   └── MissionLoader.cs
│   ├── controllers/      # UI Logic / Presenters
│   │   ├── GameContext.cs
│   │   ├── MapPresenter.cs
│   │   └── RootController.cs
│   └── model/            # UI-specific data models (currently minimal)
└── TurnForge.GodotAdapter/ # The Bridge Layer
    └── GodotAdapter.cs
```

## 3. Communication Flow

1.  **Bootstrap**: `BarelyAliveBootstrap._Ready()` builds the Engine Context and initializes `GodotAdapter`.
2.  **Mission Loading**: `GodotAdapter.Mission.LoadMissionFromJson()` uses the `MissionAdapter` to delegate to `BarelyAliveApis`.
3.  **Rendering**:
    *   **Map**: `GameContext` receives map data and signals `MapPresenter` to update the `Sprite2D`.
    *   **Entities (Actors/Props)**: *[Analysis Finding: Logic for spawning visual representations of Actors/Props appears missing or not yet located in the main controllers.]*

## 4. Best Practices Assessment

| Category | Status | Notes |
| :--- | :--- | :--- |
| **Separation of Concerns** | ✅ Good | UI logic (`MapPresenter`) is separate from Engine logic. Engine is treated as a library. |
| **Dependency Injection** | ⚠️ Partial | `GodotAdapter` uses `Instance` singleton. `BarelyAliveBootstrap` does manual composition. Consider a light DI container for larger scale (or stick to Godot Node injection). |
| **Signals** | ✅ Good | `GameContext` uses C# Events/Signals (`MissionContextChanged`) to decouple state changes from renderers. |
| **Folder Structure** | ⚠️ Mixed | Folder naming is inconsistent (PascalCase vs camelCase: `Infrastructure` vs `controllers`). Recommendation: Standardize to PascalCase. |

## 5. Refactoring Proposals

### A. Standardize Naming
Rename `src/controllers` to `src/Controllers`, `src/model` to `src/Models`, etc. to match C# conventions.

### B. Implement Entity Presenters
Create a standard flow for handling `GameUpdatePayload`:
1.  `GodotAdapter` receives `GameUpdatePayload` from Engine.
2.  `GodotAdapter` emits a Signal `GameStateUpdated(Payload)`.
3.  A new `WorldPresenter` (or `EntityManager`) listens to this signal.
4.  `WorldPresenter` instantiates/updates/removes Godot Nodes (`CharacterBody2D`, `Sprite2D`) based on the Payload.

### C. Remove Hardcoded Resources
`BarelyAliveBootstrap` has hardcoded paths like `"res://src/resources/Missions/mission01.tres"`. Move these to a Configuration Resource or export variables to the Editor.
