# TurnForge Completed Items

**Description:** This file archives completed features, epics, and tasks moved from the active backlog.

---

### FEATURE-015: Interactive Pipeline System (MVP) ✅

**Status:** 🟢 DONE  
**Completed:** 2025-12-26  
**Priority:** High  
**Effort:** 1 day

**Description:**  
Hybrid Action Resolution System supporting both fast-track execution and interactive pipelines (suspend/resume) for complex actions like combat with dice rolls.

**Deliverables:**
- ✅ `StrategyResult` with `Suspended` status and `InteractionRequest`
- ✅ `ActionContext` updated with SessionId and Variable storage
- ✅ `PipelineStrategy<T>` base class for node-based execution
- ✅ `InteractionRegistry` for session management
- ✅ `SubmitInteractionCommand` for resuming execution
- ✅ Example `InteractiveCombatPipeline` in BarelyAlive
- ✅ Unit tests validating suspend/resume flow

**Major Changes:**
- `IActionStrategy` now returns `StrategyResult` instead of `ActionStrategyResult` (alias created for compat)
- Strategies can now pause execution to ask UI for input

**Test Results:** `InteractiveCombatPipelineTests` passing (3/3)

---


### FEATURE-014: Team & Controller System ✅

**Status:** 🟢 DONE  
**Completed:** 2025-12-26  
**Priority:** High  
**Effort:** 1 day

**Description:**  
Added `Team` and `ControllerId` properties to all GameEntities to identify faction membership and player control. Essential for wargames with opposing factions.

**Deliverables:**
- ✅ `Team` property on `GameEntity` and `BaseGameEntityDefinition`
- ✅ `ControllerId` property on `GameEntity` and `BaseGameEntityDefinition`
- ✅ Override via `GameEntityBuildDescriptor` at spawn time
- ✅ `GetAgentsByTeam()` query
- ✅ `GetAgentsByController()` query
- ✅ `GetValidCombatTargets()` uses Team instead of Category
- ✅ BarelyAlive updated to use Team
- ✅ Documentation at `docs/1-understanding/team-controller.md`

**Files Modified:**
- `GameEntity.cs` - Added Team, ControllerId
- `BaseGameEntityDefinition.cs` - Added Team, ControllerId
- `GameEntityBuildDescriptor.cs` - Added Team?, ControllerId?
- `GenericActorFactory.cs` - Applied descriptor override
- `IGameStateQuery.cs` - Added query methods
- `GameStateQueryService.cs` - Implemented queries
- `BarelyAliveMovementStrategy.cs` - Uses Team
- `Units_BarelyAlive.json` - Added Team/ControllerId

**Test Results:** All movement tests passing (5/5)

---


### IDEA-002: State Confirmation using ACK ✅

**Status:** 🟢 DONE  
**Completed:** 2024-12 (Phase 2 implementation)

**Description:**  
ACK system to ensure UI has processed effects before accepting next command.

**Implementation:**
- CommandAck command implemented
- WaitingForACK flag in engine
- Effect batching before ACK

**Results:**  
✅ Engine waits for UI acknowledgment  
✅ Effects batched per command  
✅ Prevents race conditions

---

### EPIC-001: Services System (Partial) ✅

**Status:** 🟡 IN PROGRESS (1/5 services + 1/1 feature in progress)  
**Priority:** High  
**Effort:** 4-6 weeks

**Vision:**  
TurnForge provides query services callable from Strategies/Appliers for common operations without state mutation.

**Services:**
- ✅ **GameStateQueryService** - Agent/prop queries (DONE)
- 🟡 **Action Queries** - Valid moves/actions for agents (IN PROGRESS - FEATURE-011)
- ⏳ **VisibilityService** - Line of sight calculations
- ⏳ **PathfindingService** - Pathfinding algorithms
- ⏳ **FSMService** - FSM state queries

**Success Criteria:**
- [x] GameStateQueryService implemented with full API
- [/] Action query methods for UI preview (GetValidMoveDestinations)
- [ ] All services registered and accessible via IServiceProvider
- [ ] Services documented in /docs/2-using/services.md
- [ ] Example usage in strategies

**Related Features:**
- FEATURE-011: Action Query Service 🟡
- FEATURE-005: BoardQueryService (extends board queries)

---



### TASK-001: Modular Documentation Migration ✅

**Status:** 🟢 DONE
**Completed:** 2025-12-25
**Effort:** 12 hours

**Summary:**
Migrated monolithic `ENTIDADES.md` into 24 modular guide/reference files in `docs/` folder.
- **Understanding:** Architecture, Entities, FSM, Command Pipeline, etc.
- **Using:** Getting Started, Entities, Components, Commands, etc.
- **Reference:** Interfaces, Command/Decision/Effect Types, Attributes.
- **Examples:** Scenarios, Testing.

**Artifacts:**
- `memorybank/docs/` hierarchy
- `memorybank/ENTIDADES.md` (Deprecated)
- `memorybank/README.md` (Updated navigation)

---

### FEATURE-006: Dynamic Stats Component ✅

**Status:** 🟢 DONE
**Completed:** 2025-12-25
**Priority:** High
**Effort:** 3 days
**User Ref:** Feature 005 (Dynamic Attribute System)

**Description:**
Flexible attribute system that allows entities to possess arbitrary stats (e.g., "Strength", "Agility", "AttackDamage") without hardcoding properties. Attributes can represent fixed integers or random dice formulas.

**Deliverables:**
- ✅ `AttributeComponent` (Immutable)
- ✅ `AttributeValue` struct (Base, Current, Dice)
- ✅ `DiceThrowType` value object and parser
- ✅ `BaseGameEntityDefinition` updated with `Attributes` dictionary
- ✅ `GenericActorFactory` updated to parse and attach attributes
- ✅ Unit tests validating parsing and factory integration

**Files Created/Modified:**
- `src/TurnForge.Engine/Values/AttributeValue.cs`
- `src/TurnForge.Engine/Values/DiceThrowType.cs`
- `src/TurnForge.Engine/Components/AttributeComponent.cs`
- `src/TurnForge.Engine/Entities/Definitions/GameEntityDefinition.cs`
- `src/TurnForge.Engine/Entities/Actors/GenericActorFactory.cs`

---

### FEATURE-007: Test Scenario Infrastructure ✅

**Status:** 🟢 DONE  
**Priority:** High  
**Effort:** 5-7 days  
**Epic:** EPIC-002 (E2E Validation)
**Completed:** 2025-12-25

**Description:**  
Flu ent test infrastructure for creating E2E scenarios programmatically. Enables scenario serialization to JSON for reusable test cases.

**Completed Deliverables:**
- ✅ `ScenarioRunner` with fluent API (Given/When/Then)
- ✅ `CommandBuilder` for creating commands fluently
- ✅ `ScenarioSerializer` for JSON scenario loading/saving
- ✅ Example scenarios in `tests/BarelyAlive.Rules.Tests/Examples/`
- ✅ Working tests validating infrastructure
- ✅ Comprehensive documentation in `Helpers/README.md`

**Files Created:**
- `tests/BarelyAlive.Rules.Tests/Helpers/ScenarioRunner.cs`
- `tests/BarelyAlive.Rules.Tests/Helpers/CommandBuilder.cs`
- `tests/BarelyAlive.Rules.Tests/Helpers/ScenarioSerializer.cs`
- `tests/BarelyAlive.Rules.Tests/Examples/ScenarioRunnerExamples.cs`
- `tests/BarelyAlive.Rules.Tests/Helpers/README.md`

**Test Results:** All 4 example tests passing

---

### FEATURE-009: Movement API for BarelyAlive ✅

**Status:** ✅ DONE  
**Priority:** High  
**Effort:** 2-3 days  
**Epic:** EPIC-002 (E2E Validation)

**Description:**  
Add movement command API to BarelyAlive using Option A (hybrid approach): generic command API with domain-specific query results.

**Completed Deliverables:**
- [x] `MoveCharacter(characterId, targetTile)` in `IBarelyAliveApis` (generic, works for any agent)
- [x] `MoveCharacterHandler` to execute `MoveCommand`
- [x] `AgentMovedProjector` (placeholder for future movement effects)
- [x] `TileReference` DTO for position references
- [x] Updated `BarelyAliveApis`, `TestBootstrap`, `GameBootstrap` 
- [x] All existing tests pass

**Files Created/Modified:**
- Created: `MoveCharacterHandler.cs`, `TileReference.cs`, `AgentMovedProjector.cs`
- Modified: `IBarelyAliveApis.cs`, `BarelyAliveApis.cs`, `GameBootstrap.cs`, `BarelyAliveGame.cs`, `TestBootstrap.cs`, `DomainProjector.cs`

**Dependencies:**
- None (extends existing API)

---

### FEATURE-010: Game State Query API for BarelyAlive ✅

**Status:** ✅ DONE  
**Priority:** High  
**Effort:** 2-3 days  
**Epic:** EPIC-002 (E2E Validation)

**Description:**  
Add game state query API to BarelyAlive with domain-specific separation (Survivors vs Zombies) for easier UI consumption.

**Completed Deliverables:**
- [x] `GetGameState()` returning `GameStateSnapshot`
- [x] `QueryGameStateHandler` to fetch and project state
- [x] Domain-specific DTOs: `SurvivorDto`, `ZombieDto`, `PropDto`, `BoardDto`
- [x] Separate Survivors/Zombies collections in snapshot
- [x] All existing tests pass

**Files Created/Modified:**
- Created: `QueryGameStateHandler.cs`, `GameStateSnapshot.cs` (with SurvivorDto, ZombieDto, PropDto, BoardDto)
- Modified: `IBarelyAliveApis.cs`, `BarelyAliveApis.cs`

**Design Decision:**
- Used **Option A: Hybrid Approach**
- Query returns domain-specific collections (Survivors/Zombies separated)
- Command uses generic method (MoveCharacter works for all)
- Balances domain clarity with API maintainability

**Dependencies:**
- None (extends existing API)

#### FEATURE-006: Dynamic Stats Component

**Status:** 🟢 DONE
**Completed:** 2025-12-25
**Priority:** High
**Effort:** 3 days
**User Ref:** Feature 005 (Dynamic Attribute System)

**Description:**
Flexible attribute system that allows entities to possess arbitrary stats (e.g., "Strength", "Agility", "AttackDamage") without hardcoding properties. Attributes can represent fixed integers or random dice formulas.

**Deliverables:**
- ✅ `AttributeComponent` (Immutable)
- ✅ `AttributeValue` struct (Base, Current, Dice)
- ✅ `DiceThrowType` value object and parser
- ✅ `BaseGameEntityDefinition` updated with `Attributes` dictionary
- ✅ `GenericActorFactory` updated to parse and attach attributes
- ✅ Unit tests validating parsing and factory integration

**Files Created/Modified:**
- `src/TurnForge.Engine/Values/AttributeValue.cs`
- `src/TurnForge.Engine/Values/DiceThrowType.cs`
- `src/TurnForge.Engine/Components/AttributeComponent.cs`
- `src/TurnForge.Engine/Entities/Definitions/GameEntityDefinition.cs`
- `src/TurnForge.Engine/Entities/Actors/GenericActorFactory.cs`

---

#### FEATURE-007: Test Scenario Infrastructure

**Status:** 🟢 DONE  
**Priority:** High  
**Effort:** 5-7 days  
**Epic:** EPIC-002 (E2E Validation)
**Completed:** 2025-12-25

**Description:**  
Flu ent test infrastructure for creating E2E scenarios programmatically. Enables scenario serialization to JSON for reusable test cases.

**Completed Deliverables:**
- ✅ `ScenarioRunner` with fluent API (Given/When/Then)
- ✅ `CommandBuilder` for creating commands fluently
- ✅ `ScenarioSerializer` for JSON scenario loading/saving
- ✅ Example scenarios in `tests/BarelyAlive.Rules.Tests/Examples/`
- ✅ Working tests validating infrastructure
- ✅ Comprehensive documentation in `Helpers/README.md`

**Files Created:**
- `tests/BarelyAlive.Rules.Tests/Helpers/ScenarioRunner.cs`
- `tests/BarelyAlive.Rules.Tests/Helpers/CommandBuilder.cs`
- `tests/BarelyAlive.Rules.Tests/Helpers/ScenarioSerializer.cs`
- `tests/BarelyAlive.Rules.Tests/Examples/ScenarioRunnerExamples.cs`
- `tests/BarelyAlive.Rules.Tests/Helpers/README.md`

**Test Results:** All 4 example tests passing

---

#### FEATURE-009: Movement API for BarelyAlive

**Status:** ✅ DONE  
**Priority:** High  
**Effort:** 2-3 days  
**Epic:** EPIC-002 (E2E Validation)

**Description:**  
Add movement command API to BarelyAlive using Option A (hybrid approach): generic command API with domain-specific query results.

**Completed Deliverables:**
- [x] `MoveCharacter(characterId, targetTile)` in `IBarelyAliveApis` (generic, works for any agent)
- [x] `MoveCharacterHandler` to execute `MoveCommand`
- [x] `AgentMovedProjector` (placeholder for future movement effects)
- [x] `TileReference` DTO for position references
- [x] Updated `BarelyAliveApis`, `TestBootstrap`, `GameBootstrap` 
- [x] All existing tests pass

**Files Created/Modified:**
- Created: `MoveCharacterHandler.cs`, `TileReference.cs`, `AgentMovedProjector.cs`
- Modified: `IBarelyAliveApis.cs`, `BarelyAliveApis.cs`, `GameBootstrap.cs`, `BarelyAliveGame.cs`, `TestBootstrap.cs`, `DomainProjector.cs`

**Dependencies:**
- None (extends existing API)

---

#### FEATURE-010: Game State Query API for BarelyAlive

**Status:** ✅ DONE  
**Priority:** High  
**Effort:** 2-3 days  
**Epic:** EPIC-002 (E2E Validation)

**Description:**  
Add game state query API to BarelyAlive with domain-specific separation (Survivors vs Zombies) for easier UI consumption.

**Completed Deliverables:**
- [x] `GetGameState()` returning `GameStateSnapshot`
- [x] `QueryGameStateHandler` to fetch and project state
- [x] Domain-specific DTOs: `SurvivorDto`, `ZombieDto`, `PropDto`, `BoardDto`
- [x] Separate Survivors/Zombies collections in snapshot
- [x] All existing tests pass

**Files Created/Modified:**
- Created: `QueryGameStateHandler.cs`, `GameStateSnapshot.cs` (with SurvivorDto, ZombieDto, PropDto, BoardDto)
- Modified: `IBarelyAliveApis.cs`, `BarelyAliveApis.cs`

**Design Decision:**
- Used **Option A: Hybrid Approach**
- Query returns domain-specific collections (Survivors/Zombies separated)
- Command uses generic method (MoveCharacter works for all)
- Balances domain clarity with API maintainability

**Dependencies:**
- None (extends existing API)

---

