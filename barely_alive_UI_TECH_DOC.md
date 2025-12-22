# BarelyAlive UI Technical Documentation

## Boot & Mission Load Sequence

This diagram illustrates the initialization process when the Godot application starts, specifically focusing on the `BarelyAliveBootstrap`, `MissionSetUpController`, and `TurnForgeAdapter` interactions.

```mermaid
sequenceDiagram
    participant Bootstrap as BarelyAliveBootstrap
    participant Adapter as TurnForgeAdapter
    participant Engine as BarelyAliveGame (Engine)
    participant Setup as MissionSetUpController
    participant MapCtx as MapContext
    participant Presenter as MapPresenter
    
    note over Bootstrap: _Ready()
    Bootstrap->>Adapter: Bootstrap()
    Adapter->>Engine: CreateNewGame()
    Adapter->>Adapter: Init MissionAdapter
    
    Bootstrap->>Setup: SetUpMission(path)
    
    activate Setup
    Setup->>Setup: Load Resource (Mission01.tres)
    Setup->>Setup: Deserialize JSON -> DTO
    
    Setup->>MapCtx: Set(Data, Texture, Areas)
    activate MapCtx
    MapCtx-->>Presenter: Signal: MapContextChanged
    deactivate MapCtx
    
    activate Presenter
    Presenter->>MapCtx: Get MapTexture
    Presenter->>Presenter: Update Sprite2D
    deactivate Presenter
    
    Setup->>Adapter: LoadMission(MissionResource)
    deactivate Setup
    
    activate Adapter
    Adapter->>Engine: LoadMissionInEngineAsync()
    Adapter-->>Adapter: Signal: GameInitialized(success)
    deactivate Adapter
```

### Key Components

1.  **BarelyAliveBootstrap**: Entry point. Initializes the Adapter and triggers the auto-load of the default mission.
2.  **TurnForgeAdapter**: Singleton bridge between Godot and the C# Game Engine.
3.  **MissionSetUpController**: Orchestrates the loading of a mission. It updates the UI model (`MapContext`) *before* asking the engine to load the logic, ensuring the visual static properties (Map Texture) are available immediately.
4.  **MapContext**: Holds the state of the current map (Texture, Dimensions, Areas) for the UI. Emits signals when data changes.
5.  **MapPresenter**: Passive view component that listens to `MapContext` and updates the actual Godot Nodes (Sprite2D).
