A continuación tienes un **README completo, listo para copy/paste**, coherente con ---

# TurnForge

**Phase-Driven Tactical Engine for Deterministic Turn-Based Games**

---

## Value Proposition

TurnForge is a tactical game engine designed for developers who need **absolute control over turn flow, rules, and state evolution**.

It provides a deterministic, phase-driven execution model that prioritizes:

* Explicit game rules
* Fixed turn structure
* Reproducible simulation
* Strong separation between logic and presentation
* High testability and long-term maintainability

TurnForge deliberately focuses on a specific class of games instead of attempting to support incompatible paradigms.

---

## Core Principles

* **Turn-Based, Not Real-Time**
  All game progression happens through discrete turns and phases.
  There is no continuous simulation loop.

* **Phase-Driven Flow**
  Game flow is explicitly defined using a Finite State Machine (FSM).
  Only commands valid for the current phase may be executed.

* **Deterministic Simulation**
  Game state is immutable and evolves through explicit decisions.
  Given the same inputs, the engine always produces the same outcome.

* **Single Point of Mutation**
  All state changes are applied by a single orchestrator component.
  Game logic never mutates state directly.

* **Headless by Design**
  TurnForge has no rendering, input, or UI assumptions.
  It is intended to be integrated with external frameworks (e.g. Godot, Unity).

---

## What TurnForge Is

TurnForge is a **Phase-Driven Tactical Engine** suited for games that feature:

* Fixed turn order
* Explicit phases
* Discrete actions
* Rule-heavy gameplay
* Deterministic or statistically deterministic resolution
* Strong separation between game rules and presentation

---

## What TurnForge Is Not (Non-Goals)

TurnForge intentionally does **not** support:

* Real-time gameplay (RTS, RTT, action tactics)
* Dynamic or initiative-based turn order
* Speed-based or ATB systems
* Fully reactive, interrupt-driven simulations
* Physics-driven or animation-driven logic

These paradigms conflict with TurnForge’s core invariants and are considered out of scope by design.

---

Perfecto. A continuación te dejo el **concepto de TurnForge formalizado**, con **lenguaje de documentación oficial**, alineado con lo que has definido y eliminando cualquier ambigüedad.

Este texto es **copy/paste-ready** para README o docs de arquitectura.

---

# TurnForge — Conceptual Architecture

TurnForge is built around a **strict separation of concerns** between presentation, game rules, and execution.

![TurnForge Architecture](./docs/architecture.svg)

### Conceptual Model

Each layer has a **clear, non-overlapping responsibility**.

## 1. Game UI Layer

*(Built with general-purpose engines)*

### Responsibility

The Game UI layer is responsible for:

* Rendering (2D / 3D)
* Animations and visual effects
* Player input
* Camera and audio
* UX flow and feedback

### Characteristics

* Built using general-purpose engines such as:

  * Godot
  * Unity
  * Unreal
* **No game rules are implemented here**
* Does not mutate game state
* Interacts with the game only through commands and effects

### Role in the architecture

The UI is a **client of the game rules**, not the owner of them.

---

## 2. Game Rules Specification

*(Using TurnForge abstractions)*

### Responsibility

This layer defines **what the game is**, independently of how it is rendered.

It specifies:

* Game entities and components
* Game rules and constraints
* Turn structure and phases (FSM)
* Commands available to players
* Decision logic and rule resolution
* Victory and failure conditions

### Characteristics

* Written using TurnForge abstractions
* Pure game logic
* Deterministic and testable
* No dependency on rendering, input, or timing
* Expresses *rules*, not *execution*

### Key idea

This layer acts as a **formal specification of the game**, similar to:

* A digital rulebook
* A simulation model
* A rules engine configuration

---

## 3. TurnForge Runtime Engine

### Responsibility

TurnForge itself is responsible for:

* Enforcing turn and phase flow
* Validating commands against the current game state
* Executing decisions in a deterministic order
* Managing immutable game state
* Applying state transitions through a single mutation point
* Emitting effects for the UI layer

### Characteristics

* Headless
* Deterministic
* Phase-driven
* Rule-agnostic
* Does not know about:

  * Rendering
  * Input devices
  * UI concepts
  * Visual timing

### Key idea

TurnForge is the **execution engine** that ensures the game rules are followed exactly as specified.

---

## Data Flow Overview

```
[ Player Input ]
        ↓
[ Game UI ]
        ↓  (Command)
[ TurnForge Runtime ]
        ↓  (Validation + Execution)
[ New Game State + Effects ]
        ↓
[ Game UI reacts to Effects ]
```

Important:

* The UI **never** mutates game state
* The rules specification **never** renders or handles input
* TurnForge is the **only authority** over state evolution

---

## Why This Architecture Exists

This separation solves common problems in tactical games:

* Rules leaking into UI code
* Inconsistent turn flow
* Hard-to-test game logic
* Non-deterministic bugs
* Tight coupling between visuals and gameplay

By isolating rules and execution:

* Games become easier to test
* Replays and simulations become trivial
* AI can reason about the game without the UI
* The same rules can power multiple frontends

---

## Mental Model

You can think of TurnForge as:

* A **rules engine**, not a renderer
* A **simulation core**, not a game loop
* A **deterministic referee**, not a visual engine

Or, in simpler terms:

> **The UI asks what it wants to do.
> TurnForge decides if it is allowed and what happens.**

---

## Summary

TurnForge enforces a clean architectural contract:

* **Game UI**: presentation and interaction
* **Game Rules Specification**: formal definition of the game
* **TurnForge**: deterministic execution and enforcement

This makes TurnForge especially suited for:

* Tactical games
* Wargames
* Boardgame adaptations
* Rule-heavy, phase-driven systems

And explicitly unsuited for:

* Real-time games
* Animation-driven logic
* Initiative-based or reactive turn systems

---

Si quieres, el siguiente paso natural sería:

* Convertir este esquema en un **diagrama arquitectónico oficial (SVG)**
* Añadir un **example project walkthrough**
* Documentar el **contract UI ↔ TurnForge (Commands / Effects)**


## Supported Game Categories

The following categories are ordered by **architectural fit** and **required engine extension**.

---

### 1. Tactical Competitive Wargames

**Best fit — minimal additional development**

Games with:

* Player-based turns
* Explicit phase structure
* High rule density
* Statistical probability resolution
* Mostly complete information

**Examples**

* Warhammer 40,000
* Digital tabletop wargames
* Scenario-based competitive tactics

---

### 2. Tactical Cooperative Boardgames

**Excellent fit — low additional development**

Games with:

* Cooperative play
* Fixed round structure
* Procedural pressure
* Predictable activation flow

**Examples**

* Zombicide
* Cooperative dungeon crawlers
* Digital boardgame adaptations

---

### 3. Tactical Puzzle / Deterministic Strategy

**Excellent fit — engine-strength category**

Games with:

* Full information
* Deterministic outcomes
* Optimization-focused gameplay

**Examples**

* Into the Breach-like games
* Turn-based tactical puzzles

---

### 4. Scenario-Based Strategy / Wargames

**Very good fit — low additional development**

Games with:

* Alternating fixed turns
* Simple but meaningful spatial tactics
* Clear phase order

**Examples**

* Advance Wars-like games
* Light digital wargames

---

### 5. Tactical RPG (Classic SRPG)

**Good fit — moderate complexity**

Games with:

* Fixed turn order
* Discrete actions
* Persistent consequences
* Minimal reactive systems

**Examples**

* Fire Emblem-like games
* Classic turn-based tactical RPGs

---

### 6. Dungeon Crawlers (Round-Based Tactical)

Games with:

* Structured rounds
* Persistent status effects
* Predefined activation order

**Constraint:**
Turn order must remain fixed and explicit.

**Examples**

* Gloomhaven (tactical layer)
* Tactical RPG dungeon scenarios

---

## Game Spectrum (Visual Overview)

```
Deterministic / Fixed Flow ────────────────────────── Reactive / Permeable Turns

|  Tactical Puzzles  |  Boardgames  |  Wargames  |  XCOM-like  |
|       ✔✔✔          |     ✔✔✔      |    ✔✔✔     |      ✖     |

          ↑
      TurnForge
```

TurnForge deliberately occupies the **left-center** of the tactical spectrum.

---

## Architecture Overview

TurnForge uses a **Command → Decision → Applier** execution model controlled by a **Finite State Machine**.

```
Command → Validation → Handler → Decision → Scheduler → Applier → New State + Effects
```

Key characteristics:

* Commands express intent
* Decisions express logical outcomes
* Appliers perform the only allowed state mutation
* Effects are emitted for UI or logging

---

## Integration Philosophy

TurnForge is intended to be:

* Embedded into game-specific projects
* Used as a deterministic simulation core
* Integrated with external engines for rendering and input

It does not prescribe:

* UI frameworks
* Rendering pipelines
* Asset workflows

---

## Why TurnForge Exists

Many tactical games fail not because of visuals or performance, but because:

* Rules become implicit
* Turn flow becomes brittle
* State mutation becomes uncontrolled
* Testing becomes impractical

TurnForge addresses these problems by making **rules, flow, and state explicit and enforceable**.

---

## License

[Add license information here]

---

## Status

TurnForge is under active development.
The API and feature set are evolving, but the core architectural principles are considered stable.

---
