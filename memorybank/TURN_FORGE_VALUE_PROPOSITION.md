A continuación te entrego el **texto listo para incorporar como sección oficial de la documentación de TurnForge**, con **Value Proposition**, **alcance explícito**, **categorías ordenadas por encaje** y **ejemplos concretos**.

El tono y el contenido están pensados para **repositorio público / documentación técnica**, no como texto de marketing.

---

# TurnForge — Supported Game Categories

## Value Proposition

**TurnForge is a phase-driven tactical engine for deterministic, turn-based games with fixed flow.**

TurnForge is designed for developers who need:

* Absolute control over game flow and rules
* Explicit phases and turn structure
* Fully deterministic simulation
* Strong separation between game rules and presentation
* High testability, reproducibility, and replayability

TurnForge **intentionally does not support**:

* Real-time gameplay
* Variable or dynamic turn order (initiative-based systems)

This deliberate focus allows TurnForge to excel at a specific class of games instead of attempting to cover incompatible paradigms.

---

## Design Scope (Explicit)

TurnForge supports games that are:

* Turn-based
* Phase-driven
* Deterministic or statistically deterministic
* Governed by explicit rules and finite state machines
* Based on discrete actions and state transitions

TurnForge does **not** aim to support:

* Real-time systems
* Continuous simulation
* Reactive, interrupt-driven turn models
* Initiative or speed-based turn sequencing

---

## Supported Game Categories

*(ordered by architectural fit and required engine extension)*

---

## 1. Tactical Competitive Wargame

**Best fit – minimal additional development**

### Description

Competitive, turn-based tactical games with:

* Player turns
* Explicit phases
* High rule density
* Statistical probability resolution
* Mostly complete information

### Why it fits TurnForge

* Rigid turn structure maps directly to FSM nodes
* Commands and decisions model rules cleanly
* No need for dynamic turn order or real-time reactions

### Examples

* Warhammer 40,000
* Digital tabletop wargames
* Scenario-based competitive tactical games

---

## 2. Tactical Cooperative Boardgame

**Excellent fit – low additional development**

### Description

Cooperative tactical games focused on:

* Fixed round structure
* Predictable activation flow
* Procedural pressure
* Collective optimization

### Why it fits TurnForge

* Phase-driven rounds map cleanly to FSM
* Spawn systems and schedulers model procedural escalation well
* No requirement for interrupt-heavy mechanics

### Examples

* Zombicide
* Cooperative dungeon crawlers
* Digital adaptations of tactical board games

---

## 3. Tactical Puzzle / Deterministic Strategy

**Excellent fit – engine-strength category**

### Description

Tactical games with:

* Full information
* Deterministic outcomes
* Optimization-focused decision-making
* Clear win/lose conditions per turn

### Why it fits TurnForge

* Immutable state enables perfect simulation
* FSM enforces strict turn structure
* Ideal for AI search, replay, and validation

### Examples

* Into the Breach
* Turn-based tactical puzzle games

---

## 4. Scenario-Based Strategy / Wargame

**Very good fit – low additional development**

### Description

Turn-based strategy games with:

* Alternating player turns
* Fixed phase order
* Simple but meaningful spatial tactics

### Why it fits TurnForge

* Phase-driven gameplay aligns naturally with FSM
* No initiative recalculation
* Clean command validation per phase

### Examples

* Advance Wars
* Light digital wargames
* Scenario-based tactical strategy games

---

## 5. Tactical RPG (Classic SRPG)

**Good fit – moderate rule complexity**

### Description

Story-driven tactical RPGs featuring:

* Fixed turn order
* Discrete actions
* Persistent consequences (e.g. permadeath)
* Minimal reactive systems

### Why it fits TurnForge

* Turn structure remains stable
* Actions map cleanly to commands
* FSM can model chapters, battles, and phases

### Examples

* Fire Emblem
* Classic turn-based tactical RPGs

---

## 6. Dungeon Crawler (Round-Based Tactical)

**Good fit – with constraints**

### Description

Tactical dungeon crawlers with:

* Round-based structure
* Persistent status effects
* Discrete enemy activations

### Constraints

* Turn order must be fixed or predefined
* No dynamic initiative systems

### Examples

* Gloomhaven (tactical layer)
* Tactical RPG dungeon scenarios

---

## Categories Explicitly Out of Scope

The following categories are **intentionally unsupported** by TurnForge:

* Real-time strategy (RTS / RTT)
* Action tactics
* Roguelikes with real-time or hybrid turns
* Initiative-based tactical games
* Fully reactive XCOM-like simulations
* JRPGs with ATB or speed-driven turns

These paradigms conflict with TurnForge’s core invariants and are considered **non-goals**.

---

## Summary

TurnForge is best described as a:

> **Phase-Driven Tactical Engine**
> for turn-based games with fixed flow, explicit rules, and deterministic simulation.

By narrowing its scope, TurnForge:

* Gains architectural clarity
* Avoids overengineering
* Excels in its intended domains
* Provides a stable foundation for complex rule systems

