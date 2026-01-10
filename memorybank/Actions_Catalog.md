# Action Catalog

This document details the core actions in TurnForge, distinguishing between **System Actions** (automatic, background) and **Interactive Actions** (user-driven, commands).

## System Actions

System actions execute automatically on FSM node entry or other triggers. They are non-blocking and modify state atomically via the Overlay system.

### 1. EvaluateSpawnRulesAction

**Trigger:** `TurnPhases.Spawn` (OnEntry)
**Purpose:** Evaluates mission rules to spawn entities (NPCs, items) dynamically based on the current game state.

**Context Data:**
- `SpawnActionContext`: Access to `ISpawnRule` list and `GameStateView`.

**Nodes:**
1.  **EvaluateRulesNode**:
    -   Iterates through all registered `ISpawnRule`.
    -   Calls `rule.ShouldTrigger(state)`.
    -   If true, collects `SpawnInstruction`s.
2.  **ProcessSpawnsNode** (Implied/Planned):
    -   Converts `SpawnInstruction` into `SpawnOperation`s.
    -   Records operations to the `GameStateOverlay`.

**Output:**
-   Atomic state update adding new entities to the board.

---

## Interactive Actions

Interactive actions are initiated by USER commands (e.g., `StartGameCommand`, `MoveCommand`). They may suspend execution to wait for further inputs.

### 2. StartGameAction

**Command:** `StartGameCommand`
**Purpose:** Initializes the game session, including players, board topology, zones, connections, mission data, and entity deployment.

**Parameters:**
```csharp
public record StartGameParams(
    List<AddPlayerInput> PlayerInputs,      // Player configuration
    List<PropDeploymentInput> PropInputs,   // Props to deploy
    BoardDataInput BoardData,                // Board topology + zones + connections
    MissionDataInput MissionData             // Mission configuration
) : IActionParameters;
```

**Context Data:**
-   `StartGameActionContext`: Tracks player names, map selection, pending deployments (agents, props, zones, connections).

**Nodes:**

1.  **ProcessPlayerDataNode**:
    -   **Inputs:** `AddPlayerInput` (PlayerId, Name, Team, Agents, ActionPool), `ConfirmPlayersInput`.
    -   **Action:** 
        - Validates player data
        - Creates `AddPlayerOperation` for each player
        - Stores pending agent deployments
    -   **Completion:** When `ConfirmPlayersInput` is received

2.  **ProcessBoardDataNode**:
    -   **Inputs:** `BoardDataInput` (MapId, BoardDefinition, Zones, Connections).
    -   **Action:** 
        - Creates board topology (`CreateBoardOperation`)
        - Stores zones in `PendingZoneDeployments`
        - Stores connections in `PendingConnectionDeployments`
        - Stores props in `PendingPropDeployments`
    -   **BoardDataInput Structure:**
        ```csharp
        public record BoardDataInput(
            string MapId,
            IBoardDefinition BoardDefinition,           // Topology (graph edges)
            IReadOnlyList<BoardZoneDefinition> Zones,   // Zones with traits
            IReadOnlyList<BoardConnectionDefinition> Connections  // Connection props
        );
        ```
    -   **Completion:** When board data is processed

3.  **DeployEntitiesNode**:
    -   **Inputs:** None (automatic processing of pending data).
    -   **Action:** 
        - Deploys all pending agents using `ISpawnService`
        - Deploys all pending props using `ISpawnService`
        - Deploys all pending zones using `ISpawnService`
        - Deploys all pending connections using `ISpawnService`
        - Records all spawn operations to overlay
    -   **Completion:** When all entities are deployed

4.  **BuildGameNode**:
    -   **Action:** Finalizes initialization (sets initial FSM node if needed).
    -   **Completion:** Immediately

**Output:**
-   A fully initialized `GameState` with:
    - Players registered
    - Board topology created
    - Zones spawned with traits
    - Connections spawned with metadata
    - Agents deployed
    - Props deployed
    - Ready for first turn

**Example Usage:**
```csharp
// Create board data
var boardDef = ParchisBoardFactory.CreateDescriptor();
var zones = ParchisZoneFactory.CreateZones();
var connections = ParchisConnectionFactory.CreateConnections();

var boardInput = new BoardDataInput(
    "parchis_standard",
    boardDef,
    zones,
    connections
);

// Create parameters
var startParams = new StartGameParams(
    playerInputs,
    propInputs,
    boardInput,
    missionData
);

// Execute action
var result = GameEngineExtensions.ExecuteAction(
    engine, 
    CoreActions.StartGameActionId, 
    startParams
);
```

---
