# TurnForge Completed Items

**Description:** This file archives completed features, epics, and tasks moved from the active backlog.

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

### EPIC-002: BarelyAlive E2E Validation System

**Status:** 🟡 IN PROGRESS (3/4 features done)  
**Priority:** High  
**Effort:** 3-4 weeks

**Vision:**  
Enable rapid E2E validation of BarelyAlive functionality without requiring Godot UI. Provide both programmatic test infrastructure and interactive console simulator for testing game flows, command sequences, and state transitions.

**Success Criteria:**
- [x] Fluent API for scenario creation in tests
- [x] JSON-based scenario serialization
- [x] Command builder for common operations
- [x] Movement API exposed through BarelyAlive
- [x] State query API exposed through BarelyAlive
- [ ] Console simulator for interactive testing
- [ ] Documentation with examples

**Related Features:**
- FEATURE-007: Test Scenario Infrastructure ✅
- FEATURE-009: Movement API ✅
- FEATURE-010: Game State Query API ✅
- FEATURE-008: Console Simulator Project

**Dependencies:**
- None (builds on existing BarelyAlive.Rules.Tests infrastructure)

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
