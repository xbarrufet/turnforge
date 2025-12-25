# TurnForge Backlog

**Last Updated:** 2025-12-25  
**Active Items:** 6 | **Completed:** 2 | **Ideas:** 2 | **Total:** 12

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

**Status:** 🟡 IN PROGRESS (1/4 services done)  
**Priority:** High  
**Effort:** 4-6 weeks

**Vision:**  
TurnForge provides query services callable from Strategies/Appliers for common operations without state mutation.

**Services:**
- ✅ **GameStateQueryService** - Agent/prop queries (DONE)
- ⏳ **VisibilityService** - Line of sight calculations
- ⏳ **PathfindingService** - Pathfinding algorithms
- ⏳ **FSMService** - FSM state queries

**Success Criteria:**
- [x] GameStateQueryService implemented with full API
- [ ] All services registered and accessible via IServiceProvider
- [ ] Services documented in /docs/2-using/services.md
- [ ] Example usage in strategies

**Related Features:**
- FEATURE-005: BoardQueryService (extends board queries)

---

## 🟡 In Progress

### TASK-001: Complete Modular Documentation Migration

**Status:** 🟡 IN PROGRESS  
**Priority:** Medium  
**Effort:** 8-12 hours  
**Owner:** Unassigned

**Context:**  
Documentation restructured into modular format. Content extraction from ENTIDADES.md pending.

**Tasks:** 21 modular files to create
- Phase 2.1: Understanding (8 files)
- Phase 2.2: Using (8 files)
- Phase 2.3: Reference (5 files)
- Phase 2.4: Examples (3 files)

**Current Progress:**
- ✅ Folder structure created
- ✅ Navigation READMEs
- ✅ Root README integrated
- ⏳ Content extraction pending

**Success Criteria:**
- [ ] All 21 files created with content
- [ ] ENTIDADES.md deprecated with notice
- [ ] Cross-references working
- [ ] Examples complete

**How to Resume:**  
See BACKLOG entry for detailed subtasks and /brain/task.md for checklist.

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

#### FEATURE-006: Dynamic Stats Component

**Status:** ⚪ BACKLOG  
**Priority:** Medium  
**Effort:** 1-2 weeks

**Description:**  
Flexible attribute system for game-specific stats (Movement, Combat, etc.) defined via JSON.

**Vision:**  
Allow game designers to define custom stat categories without code changes.

**Proposed Architecture:**

```csharp
public record AttributeValue {
    public string Id { get; init; }         // "MOV", "DMG"
    public string Category { get; init; }   // "Combat", "Meta" (user-defined)
    public int Base { get; init; }
    public string Dice { get; init; }       // "1D6+2"
}

public interface IAttributeComponent : IGameEntityComponent {
    Dictionary<string, AttributeValue> Attributes { get; }
}
```

**Acceptance Criteria:**
- [ ] AttributeValue type created
- [ ] IAttributeComponent interface
- [ ] JSON deserialization support
- [ ] Dice expression evaluator
- [ ] Category-based filtering in strategies
- [ ] UI adapter emits typed payloads by category

**Technical Notes:**
- Engine remains agnostic (just stores strings/numbers)
- Game rules filter by Category
- UI can auto-generate tabs per category

**Advantages:**
- ✅ Decoupling - Engine doesn't know "Magic" vs "Tech"
- ✅ Reusability - Same dice system for all rolls
- ✅ Scalability - UI auto-adapts to categories

**Dependencies:**
- None (net new feature)

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
