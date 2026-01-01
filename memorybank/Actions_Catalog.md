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
**Purpose:** Initializes the game session, including players, board, mission data, and initial entity deployment.

**Context Data:**
-   `StartGameActionContext`: Tracks player names, map selection, pending deployments.

**Nodes:**
1.  **ProcessPlayerDataNode**:
    -   **Inputs:** `AddPlayerInput` (Name, Agents, Optional Positions), `ConfirmPlayersInput`.
    -   **Action:** Creates `AddPlayerOperation`, validation.
2.  **ProcessBoardDataNode**:
    -   **Inputs:** `SelectMapInput` (MapId, BoardDefinition, MissionData).
    -   **Action:** Creates board (`CreateBoardOperation`) and sets mission data. Resolves `null` agent positions using `MissionData.PlayerSpawnZones`.
3.  **DeployEntitiesNode**:
    -   **Inputs:** None (automatic processing of pending data).
    -   **Action:** Deploys all pending agents and props using `IEntityApplier`. Records operations.
4.  **BuildGameNode**:
    -   **Action:** Finalizes initialization (if creating root FSM node).

**Output:**
-   A fully initialized `GameState` ready for the first turn.
