# TurnForge Backlog

**Last Updated:** 2025-12-25  
**Active Items:** 0 | **Completed:** 7 | **Pending:** 15
 | **Total:** 20

---

## 📖 How to Use This Backlog

### Item Types

- **EPIC** - Large initiative spanning multiple features (weeks/months)
- **FEATURE** - Specific functionality to implement (days/week)  
- **IDEA** - Concept requiring analysis before becoming a feature
- **TASK** - Concrete work item with clear deliverables (hours/days)

### Workflow

```
IDEA → Analysis → Approval → FEATURE/EPIC
FEATURE → Design → Implementation → TASK(s)
TASK → Complete → DONE
```

### Status Indicators

- 🟢 **DONE** - Completed and verified
- 🟡 **IN PROGRESS** - Currently being worked on
- 🔴 **BLOCKED** - Waiting on dependency
- ⚪ **BACKLOG** - Not started, ready to pick up
- 💡 **IDEA** - Needs analysis/decision

### Priority Levels

- **Critical** - Blocking other work, must do now
- **High** - Important for upcoming release
- **Medium** - Valuable but not urgent
- **Low** - Nice to have, future consideration

---

## 🟢 Completed

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

## 🟡 In Progress


### ✅ TASK-001: Modular Documentation Migration

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

---

## ⚪ Ready to Work

### High Priority

#### FEATURE-005: BoardQueryService

**Status:** ⚪ BACKLOG  
**Priority:** High  
**Effort:** 3-5 days  
**Epic:** EPIC-001 (Services)

**Description:**  
Service for board queries with game state integration (occupied positions, neighbors, etc.)

**Acceptance Criteria:**
- [ ] `GetNeighbors(Position)` - Returns valid connected neighbors
- [ ] `Distance(Position, Position)` - Calculates shortest path distance
- [ ] `IsOccupied(Position)` - Checks if agents at position
- [ ] `GetReachable(Agent, int range)` - All positions within move range
- [ ] Unit tests for all methods

**Technical Notes:**
- Delegates to ISpatialModel for base queries
- Adds GameState awareness (agents, blockers)
- Used by movement strategies

**Dependencies:**
- ISpatialModel implementation
- GameStateQueryService (for agent queries)

---

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

#### FEATURE-008: Console Simulator Project

**Status:** ⚪ BACKLOG  
**Priority:** High  
**Effort:** 3-5 days  
**Epic:** EPIC-002 (E2E Validation)

**Description:**  
Interactive console application for manual E2E testing and debugging. Provides REPL interface and script execution mode.

**Acceptance Criteria:**
- [ ] New project: `src/BarelyAlive.Simulator/`
- [ ] Interactive REPL mode (type commands)
- [ ] Script execution mode (run JSON scenarios)
- [ ] ASCII board visualization (optional)
- [ ] Detailed effect/event logging
- [ ] Command history and replay
- [ ] README with usage examples

**Technical Notes:**
- Console application (.NET 8)
- References `BarelyAlive.Rules`
- Uses `BarelyAliveGame` API (same as Godot adapter)
- Reuses FEATURE-007 infrastructure internally

**Example Usage:**
```bash
# Interactive mode
$ simulator interactive
> load mission01.json
> spawn Mike Doug
> move Mike B2
> attack Mike Zombie1

# Script mode
$ simulator run scenarios/test_combat.json
```

**Dependencies:**
- FEATURE-007 (reuses ScenarioRunner/CommandBuilder)

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

#### FEATURE-011: Action Query Service (GetValidMoveDestinations)

**Status:** ⚪ BACKLOG  
**Priority:** High  
**Effort:** 2-3 days  
**Epic:** EPIC-001 (Services)

**Description:**  
Enable UI to query valid move destinations for an agent without executing commands. Supports UI features like highlighting available tiles on agent selection.

**Acceptance Criteria:**
- [ ] `GetValidMoveDestinations(agentId)` method in `IGameStateQuery`
- [ ] Implementation in `GameStateQueryService` with board dependency
- [ ] Basic validation logic (AP, board bounds, neighbors)
- [ ] Unit tests for query service
- [ ] Integration tests with real board scenarios
- [ ] Documentation in implementation plan

**Technical Notes:**
- **Approach:** Option C - Extend existing `GameStateQueryService`
- Constructor needs `GameBoard` injected for neighbor queries
- Validation is **simplified** (doesn't replicate full strategy logic)
- Good enough for 80% of UI preview use cases
- Easily extensible for future action types (attack, etc.)

**API Design:**
```csharp
public interface IGameStateQuery
{
    // Existing...
    Agent? GetAgent(string agentId);
    
    // NEW
    IReadOnlyList<Position> GetValidMoveDestinations(string agentId);
}
```

**Future Extensibility:**
- Phase 2: `GetValidAttackTargets(agentId)`
- Phase 3: Optional `IActionStrategy.Validate()` for 100% accurate validation

**Dependencies:**
- GameBoard (for neighbor queries)
- IActionPointsComponent (for AP checks)

---

#### FEATURE-012: DiceThrow Service

**Status:** ⚪ BACKLOG  
**Priority:** High  
**Effort:** 3-5 days  
**Epic:** EPIC-001 (Services)

**Description:**  
Create a robust dice rolling service supporting standard RPG dice notation with modifiers and comparison operations. Provides deterministic random number generation for skill checks, damage rolls, and action validation.

**Acceptance Criteria:**
- [ ] `DiceThrowType` value object representing dice notation (e.g., "1D20", "2D6+3")
- [ ] Parse dice notation strings into structured format
- [ ] Execute rolls: `Roll(DiceThrowType)` returning result
- [ ] Fluent comparison API: `Roll(1D6).IsHigherOrEqualThan(3)` returns bool
- [ ] Support for:
  - Single/multiple dice: `1D20`, `3D6`
  - Modifiers: `1D6+2`, `2D4-1`
  - Advantage/disadvantage (optional): `2D20 keep highest`
- [ ] Deterministic seeding for tests (inject `Random` or seed)
- [ ] Unit tests with mocked randomness
- [ ] Documentation with dice notation grammar

**API Design:**
```csharp
// Value Object for dice notation
public record DiceThrowType
{
    public int DiceCount { get; init; }      // 2 in "2D6"
    public int DiceSides { get; init; }      // 6 in "2D6"
    public int Modifier { get; init; }       // +3 in "2D6+3"
    
    public static DiceThrowType Parse(string notation); // "1D20+5"
}

// Service interface
public interface IDiceThrowService
{
    DiceRollResult Roll(DiceThrowType diceThrow);
    DiceRollResult Roll(string notation); // Convenience overload
}

// Result with fluent comparisons
public record DiceRollResult
{
    public int Total { get; init; }
    public IReadOnlyList<int> IndividualRolls { get; init; }
    public DiceThrowType DiceThrow { get; init; }
    
    // Fluent comparisons
    public bool IsHigherOrEqualThan(int threshold);
    public bool IsLowerThan(int threshold);
    public bool IsExactly(int value);
}
```

**Example Usage:**
```csharp
// In ActionStrategy
var diceService = serviceProvider.GetRequiredService<IDiceThrowService>();

// Parse notation
var dangerCheck = DiceThrowType.Parse("1D6");
var result = diceService.Roll(dangerCheck);

if (result.IsHigherOrEqualThan(4))
{
    // Passed check
    yield return new ActionDecision(/* ... */);
}
else
{
    // Failed - apply damage
    yield return new ActionDecision(/* apply damage */);
}

// Inline notation
var damage = diceService.Roll("2D6+3");
```

**Technical Notes:**
- **Deterministic Testing:** Inject `Random` instance for predictable tests
- **Value Object:** `DiceThrowType` is immutable, validated on creation
- **Parser:** Use regex or simple parser for dice notation
- **Extensibility:** Support custom dice formulas via strategy pattern
- **Performance:** Cache parsed notation if needed

**Grammar (Dice Notation):**
```
<dice> ::= <count>D<sides>[<modifier>]
<count> ::= positive integer (default 1)
<sides> ::= positive integer
<modifier> ::= [+|-]<value>
```

**Related Features:**
- IDEA-004: Conditional Actions with Dice Roll Validation (primary consumer)
- FEATURE-006: Dynamic Stats Component (stat-based check modifiers)

**Dependencies:**
- None (core service, no game logic dependencies)

---

#### FEATURE-004: Handler/Applier Autodiscovery

**Status:** ⚪ BACKLOG  
**Priority:** High  
**Effort:** 5-7 days

**Description:**  
Reduce boilerplate by auto-discovering and registering handlers, services, and appliers via reflection/attributes.

**Acceptance Criteria:**
- [ ] `[AutoRegister]` attribute for handlers
- [ ] Assembly scanning on startup
- [ ] Auto-registration in DI container
- [ ] Works for: ICommandHandler, IApplier, IService
- [ ] Configuration to enable/disable per assembly

**Technical Notes:**
- Use reflection to scan assemblies
- Respect `[DoNotAutoRegister]` opt-out
- Performance: cache registrations

**Dependencies:**
- None (engine improvement)

---

### Medium Priority

#### FEATURE-002: Specialized Engine Props

**Status:** ⚪ BACKLOG  
**Priority:** Medium  
**Effort:** 3-4 days

**Description:**  
Built-in prop types provided by engine for common use cases.

**Proposed Types:**
- `SpawnPoint` - Agent spawn location with metadata
- `Objective` - Mission objective marker
- `Trigger` - Event trigger zone
- `Blocker` - Movement blocking obstacle

**Acceptance Criteria:**
- [ ] Base classes created in TurnForge.Engine
- [ ] Each type has descriptor + definition
- [ ] Documented in /docs/2-using/entities.md
- [ ] Example usage in BarelyAlive.Rules

**Technical Notes:**
- Extend Prop base class
- Add specialized components as needed
- Keep engine-agnostic (no game logic)

---

#### FEATURE-003: Specialized Engine Descriptors

**Status:** ⚪ BACKLOG  
**Priority:** Medium  
**Effort:** 2-3 days

**Description:**  
Convenience descriptors for common patterns.

**Proposed Types:**
- `PositionedDescriptor<T>` - Auto-includes Position
- `HealthDescriptor<T>` - Auto-includes Health
- `CombatDescriptor<T>` - Health + damage stats

**Acceptance Criteria:**
- [ ] Base descriptor classes created
- [ ] PropertyAutoMapper supports inheritance
- [ ] Documented with examples
- [ ] Used in at least one game project

**Technical Notes:**
- Generic base classes
- Composable (descriptor can extend multiple via interfaces)

**Dependencies:**
- PropertyAutoMapper must handle descriptor inheritance

---


## 💡 Ideas / Analysis Needed

### IDEA-001: Effects As Behaviours

**Status:** 💡 NEEDS ANALYSIS  
**Proposed By:** Initial design discussion

**Problem / Opportunity:**  
Current effects are fire-and-forget. Need temporal effects (poison, buffs, debuffs) that persist across turns.

**Proposal:**  
Effects = IBehaviour + IApplier?

Temporal effects could:
- Apply at specific turn phases
- Stack or override
- Have duration/expiry

**Questions to Answer:**
- [ ] How do temporal effects differ from current IGameEffect?
- [ ] Should they be Decisions with DecisionTiming?
- [ ] Do we need a separate EffectComponent?
- [ ] How does UI represent ongoing effects?

**Next Steps:**
- [ ] Spike: prototype temporal effect with existing system
- [ ] Design doc: proposed new architecture if needed
- [ ] Decision: extend existing vs new system

---

### IDEA-003: Strongly-Typed Decisions

**Status:** 💡 PARTIALLY IMPLEMENTED  
**Proposed By:** Architecture refinement

**Problem / Opportunity:**  
More specific decision types enable type-safe appliers and better validation.

**Current State:**
- ✅ `ActionDecision` - Component updates (implemented)
- ✅ `SpawnDecision<T>` - Entity spawning (implemented)

**Proposal:**  
Add more typed decisions:
- `MovementDecision` - Movement-specific with path info
- `DamageDecision` - Damage calculation results
- `BehaviourUpdateDecision` - Add/remove behaviours
- `EffectDecision` - Apply temporal effects

**Benefits:**
- ✅ Type-safe applier routing
- ✅ Stricter validation per decision type
- ✅ Clearer intent in code

**Questions to Answer:**
- [ ] Do we need these beyond ActionDecision's flexibility?
- [ ] Would this complicate the architecture?
- [ ] Real-world use cases that can't use ActionDecision?

**Next Steps:**
- [ ] Analyze current ActionDecision usage
- [ ] Identify pain points
- [ ] Decide: extend ActionDecision vs new types

---

### IDEA-004: Conditional Actions with Dice Roll Validation

**Status:** 💡 NEEDS ANALYSIS  
**Proposed By:** Xavier Barrufet, 2025-12-25

**Problem / Opportunity:**  
Many tactical games require actions to be validated through skill checks or dice rolls before execution. Example: moving through a "DANGEROUS" area requires passing a die roll to avoid triggering effects (damage, spawn, etc.).

**Current Gap:**  
The engine can validate action legality (AP cost, range, blockers) but has no mechanism for:
- Conditional validation based on random checks
- Area behaviors that trigger validation requirements
- Roll-based success/failure affecting action completion

**Use Cases:**
- **Dangerous Terrain:** Moving through hazardous tiles requires a die roll to avoid damage/effects
- **Skill Checks:** Actions requiring attribute checks (e.g., "Climb" requires STR check)
- **Detection Rolls:** Stealth movement triggering enemy awareness checks
- **Environmental Hazards:** Traversing traps, swimming, climbing, etc.

**Proposal:**  
Extend the action validation pipeline to support:

1. **Area Behaviors on Tiles/Zones**  
   - `DangerousBehavior`, `DifficultTerrainBehavior`, etc.
   - Attached to board positions or props
   - Define validation requirements (dice formula, difficulty)

2. **Action Validation with Dice Resolution**  
   - Strategy checks if action triggers conditional validation
   - Generate `ValidationDecision` requiring dice roll
   - Wait for resolution before completing action
   - Apply consequences on failure (damage, AP loss, action cancel)

3. **Flow Example:**
   ```
   Player: MoveCommand(agent, dangerousTile)
   → Strategy detects DangerousBehavior on tile
   → Emit ValidationRequiredDecision(rollFormula: "1D6", difficulty: 4)
   → UI presents roll to player (or AI auto-rolls)
   → Player rolls → Effect: ValidationResultEvent(success: true/false)
   → Strategy completes or cancels move based on result
   ```

**Questions to Answer:**
- [ ] Should validation be part of IActionStrategy or a separate validation layer?
- [ ] How to handle UI interaction for player-controlled rolls vs AI auto-resolution?
- [ ] Should failed rolls prevent action entirely or apply partial effects?
- [ ] Do we need persistent "area behavior" components on board positions?
- [ ] How to integrate with existing Behavior system (IBehavior)?
- [ ] Should this extend AttributeComponent (FEATURE-006) for stat-based checks?
- [ ] What's the relationship with Effects (IDEA-001)?

**Related Concepts:**
- FEATURE-006: Dynamic Stats Component (for attribute checks)
- IDEA-001: Effects As Behaviours (temporal hazard effects)
- Board system (area behaviors, tile properties)
- Dice resolution system (needs design)

**Next Steps:**
- [ ] Research similar implementations in tactical games
- [ ] Design area behavior component architecture
- [ ] Prototype validation flow with simple dangerous terrain
- [ ] Define dice resolution protocol (command/event flow)
- [ ] Create design document with proposed architecture
- [ ] Get user approval before promoting to FEATURE

---

### IDEA-005: Combat System Implementation

**Status:** 💡 NEEDS ANALYSIS  
**Proposed By:** Xavier Barrufet, 2025-12-25

**Problem / Opportunity:**  
TurnForge currently lacks a built-in combat system. Games like BarelyAlive require attack actions, damage calculation, weapon stats, range checks, and combat effects. A generic, extensible combat system would accelerate game development.

**Current Gap:**  
No standardized approach for:
- Attack commands and validation (range, LoS, weapon requirements)
- Damage calculation (weapon stats, armor, resistances)
- Combat effects (wounds, conditions, knockback)
- Weapon/equipment management
- Attack types (melee, ranged, area of effect)

**Use Cases:**
- **Melee Combat:** Close-range attacks with strength-based damage
- **Ranged Combat:** Distance attacks with ammo consumption and accuracy
- **Area Attacks:** Grenades, explosions affecting multiple targets
- **Special Attacks:** Critical hits, status effects, multi-target
- **Defense Mechanics:** Armor, dodge, cover systems

**Potential Approaches:**

**Option A: Generic ActionStrategy Extension**
- Combat is just another action type
- `AttackCommand` + `AttackStrategy`
- Damage calculation in strategy logic
- Flexible but requires game-specific implementation

**Option B: Combat-Specific Components**
- `IWeaponComponent`, `IArmorComponent`, `IDamageComponent`
- `CombatService` for calculations
- Pre-built combat strategies
- More opinionated, batteries-included

**Option C: Hybrid**
- Core combat primitives in engine (damage types, armor)
- Game-specific combat logic via strategies
- Reusable combat services (range calculation, LoS)

**Questions to Answer:**
- [ ] Should combat be engine-level or game-specific?
- [ ] How to model weapons: as Items (see IDEA-006) or components?
- [ ] Damage types: generic int or typed (Physical, Magic, etc.)?
- [ ] How to integrate with DiceThrowService (FEATURE-012)?
- [ ] Cover/armor system: separate service or behavior?
- [ ] Line of Sight: delegate to VisibilityService or inline?
- [ ] How to represent attack results for UI animation?
- [ ] Multi-target attacks: separate command or parameter?

**Related Concepts:**
- FEATURE-012: DiceThrow Service (damage/hit rolls)
- IDEA-006: Items & Inventory (weapon/armor items)
- FEATURE-005: BoardQueryService (range/LoS queries)
- Effects system (combat conditions, buffs/debuffs)

**Next Steps:**
- [ ] Review BarelyAlive combat requirements
- [ ] Research combat systems in similar engines
- [ ] Define core combat vocabulary (Attack, Damage, Defense)
- [ ] Prototype simple melee attack with current system
- [ ] Create comprehensive design document
- [ ] Get user approval on approach before implementation

---

### IDEA-006: Items as GameEntity with Inventory System

**Status:** 💡 NEEDS ANALYSIS  
**Proposed By:** Xavier Barrufet, 2025-12-25

**Problem / Opportunity:**  
Many tactical games require item management: weapons, equipment, consumables, quest items. Currently TurnForge only supports Agents and Props. Adding Items as a first-class GameEntity would enable inventory, equipment, and loot systems.

**Current Gap:**
- No concept of pickup/drop items
- No inventory management for agents
- No container props (chests, corpses)
- No item trading/transfer between agents
- No equipment slots (weapon, armor, accessories)

**Use Cases:**
- **Weapons & Armor:** Equippable items affecting combat stats
- **Consumables:** Med-kits, ammo, grenades (use once, depletion)
- **Quest Items:** Keys, documents, objectives
- **Loot Containers:** Searchable chests, supply crates, corpses
- **Trading:** Share items between friendly agents
- **Encumbrance:** Item weight affecting movement AP

**Potential Architecture:**

**1. Item as GameEntity**
```csharp
public abstract class Item : GameEntity
{
    // Inherits: Id, Components
}

public interface IItemComponent : IGameEntityComponent
{
    string ItemType { get; }        // "Weapon", "Consumable", "QuestItem"
    int Weight { get; }
    bool IsStackable { get; }
    int StackSize { get; }
}
```

**2. Inventory Component (on Agents)**
```csharp
public interface IInventoryComponent : IGameEntityComponent
{
    IReadOnlyList<string> ItemIds { get; }  // References to Item entities
    int MaxCapacity { get; }
    int CurrentWeight { get; }
}
```

**3. Container Component (on Props)**
```csharp
public interface IContainerComponent : IGameEntityComponent
{
    IReadOnlyList<string> ContainedItemIds { get; }
    bool IsLocked { get; }
    int Capacity { get; }
}
```

**4. Equipment Component (on Agents)**
```csharp
public interface IEquipmentComponent : IGameEntityComponent
{
    Dictionary<string, string?> Slots { get; }  // "MainHand" -> weaponId
    // Slots: "MainHand", "OffHand", "Armor", "Accessory1", "Accessory2"
}
```

**Actions Needed:**
- `PickupItemCommand(agentId, itemId)` - Add item to inventory
- `DropItemCommand(agentId, itemId, position)` - Drop item on board
- `EquipItemCommand(agentId, itemId, slotName)` - Equip to slot
- `UnequipItemCommand(agentId, slotName)` - Move to inventory
- `UseItemCommand(agentId, itemId)` - Consume item (heal, etc.)
- `TransferItemCommand(fromAgentId, toAgentId, itemId)` - Trade
- `SearchContainerCommand(agentId, containerId)` - Loot container

**Questions to Answer:**
- [ ] Should Items exist on the GameBoard at positions like Props?
- [ ] Item state management: separate collection or part of GameState.Entities?
- [ ] How to handle item effects (weapon damage, consumable healing)?
- [ ] Equipment slots: fixed engine slots or game-configurable?
- [ ] Item stacking: how to represent quantity?
- [ ] Equipped items: reference by ID or duplicate stats to agent?
- [ ] Containers: new entity type or prop with container component?
- [ ] Serialization: items as descriptors or inline data?
- [ ] UI representation: separate item panel or integrated into agent view?

**Related Concepts:**
- IDEA-005: Combat System (weapons as items)
- FEATURE-002: Specialized Engine Props (containers as special props)
- FEATURE-006: Dynamic Stats Component (item stat bonuses)
- EPIC-001: Services (inventory query service)

**Next Steps:**
- [ ] Analyze BarelyAlive item requirements (weapons, med-kits)
- [ ] Define item lifecycle (spawn → pickup → equip → use → drop)
- [ ] Decide: Item as Entity vs Item as Component
- [ ] Prototype simple pickup/drop with current system
- [ ] Design equipment slot system
- [ ] Create design document with full architecture
- [ ] Get user approval before promoting to EPIC

---

## 📋 Templates

### EPIC Template

```markdown
## EPIC-XXX: [Title]

**Status:** ⚪ BACKLOG | 🟡 IN PROGRESS | 🟢 DONE | 🔴 BLOCKED  
**Priority:** Critical | High | Medium | Low  
**Effort:** [weeks/months]  
**Owner:** [name/unassigned]

### Vision
[What we're trying to achieve]

### Success Criteria
- [ ] Criterion 1
- [ ] Criterion 2

### Related Features
- FEATURE-XXX
- FEATURE-YYY

### Dependencies
- [None / List items]
```

### FEATURE Template

```markdown
## FEATURE-XXX: [Title]

**Status:** ⚪ BACKLOG | 🟡 IN PROGRESS | 🟢 DONE | 🔴 BLOCKED  
**Priority:** Critical | High | Medium | Low  
**Effort:** [hours/days]  
**Epic:** EPIC-XXX (if applicable)

### Description
[What needs to be built]

### Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2

### Technical Notes
[Implementation approach, APIs affected, etc.]

### Dependencies
- [None / List items]
```

### IDEA Template

```markdown
## IDEA-XXX: [Title]

**Status:** 💡 NEEDS ANALYSIS | 🟢 APPROVED → FEATURE-XXX | 🔴 REJECTED  
**Proposed By:** [name/date]

### Problem / Opportunity
[What problem does this solve?]

### Proposal
[High-level solution approach]

### Questions to Answer
- [ ] Question 1
- [ ] Question 2

### Next Steps
- [ ] Spike/research
- [ ] Create design doc
- [ ] Get approval
```

### TASK Template

```markdown
## TASK-XXX: [Title]

**Status:** ⚪ BACKLOG | 🟡 IN PROGRESS | 🟢 DONE | 🔴 BLOCKED  
**Priority:** Critical | High | Medium | Low  
**Effort:** [hours/days]  
**Owner:** [name/unassigned]

### Description
[Concrete work to complete]

### Steps
- [ ] Step 1
- [ ] Step 2

### Success Criteria
- [ ] Criterion 1

### Dependencies
- [None / List items]
```

---

**End of Backlog**
