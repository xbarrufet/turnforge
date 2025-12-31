# TurnForge

**Phase-Driven Tactical Engine for Deterministic Turn-Based Games**

---

## 🚀 Value Proposition

TurnForge is a tactical game engine designed for developers who need **absolute control over turn flow, rules, and state evolution**.

It provides a deterministic, phase-driven execution model that prioritizes:

* Explicit game rules
* Fixed turn structure
* Reproducible simulation
* Strong separation between logic and presentation
* High testability and long-term maintainability

TurnForge deliberately focuses on a specific class of games instead of attempting to support incompatible paradigms.

---

## 🎯 What TurnForge Is (and What It Isn't)

### Supported Game Types (The "Sweet Spot")
TurnForge is built for **Phase-Driven Tactical Games** dealing with discrete actions and rules:
*   **Tactical Wargames** (e.g., Warhammer 40k, Kill Team)
*   **Boardgame Adaptations** (e.g., Zombicide, Gloomhaven)
*   **Deterministic Puzzles** (e.g., Into the Breach)

### Non-Goals
TurnForge is **NOT** suitable for:
*   Real-Time Strategy (RTS)
*   Physics-driven simulations
*   Twitch-reaction gameplay
*   Speed-based/ATB turn systems

---

## 🏗️ Conceptual Architecture

TurnForge is built around a **strict separation of concerns** between presentation, game rules, and execution.

![TurnForge Architecture](docs/hl_architecture.svg)

### 1. Game UI Layer (Presentation)
*(Built with Godot, Unity, etc.)*
*   **Role:** Client. Visualizes state and captures input.
*   **Constraint:** Never mutates state directly. Only sends **Commands**.

### 2. Game Rules Specification (Definition)
*(Pure C# abstractions)*
*   **Role:** Rulebook. Defines *what* the game is (Entities, Definitions, Missions).
*   **Constraint:** Declarative and stateless.

### 3. TurnForge Runtime (Execution)
*(The Engine)*
*   **Role:** Referee. Enforces flow, validates commands, and executes logic.
*   **Constraint:** Deterministic and headless.

---

## ⚙️ Core Design Principles

### 1. Phase-Driven Flow (FSM)
Game flow is controlled by a **Finite State Machine**. Only commands valid for the current phase are allowed. The FSM triggers **System Workflows** automatically on state transitions.

### 2. Workflow-Based Logic
Complex logic (like "Start Game", "Attack Sequence") is encapsulated in **Workflows**.
*   **Interactive Workflows:** Driven by user commands (e.g., Select Unit -> Select Target -> Confirm).
*   **System Workflows:** Run automatically by the engine (e.g., Spawn Phase, End Turn Cleanup).

### 3. Deterministic State (Overlay System)
State is immutable. Mutations happen via **Operations** applied to a transactional **Overlay**.
```
Command → Workflow → [Nodes] → Overlay Operations → New State + Effects
```

---

## 📚 Using TurnForge

### Documentation
*   **[Main Game Loop & Flow](memorybank/main_game_loop.md)**: How the engine runs.
*   **[Workflow System](memorybank/Workflows_Catalog.md)**: System vs. Interactive workflows.
*   **[Interactive Patterns](memorybank/Interactive_Workflows.md)**: Handling user input.
*   **[Board & Deployment](memorybank/Board_Management.md)**: Managing the grid and spawning.

### Getting Started
1.  Define your **Definitions** (Agents, Weapons).
2.  Configure the **FSM Graph** for your game loop.
3.  Implement **Workflows** for your specific mechanics.
4.  Hook up your UI to send **Commands** and listen for **State Changes**.

---

## Status & License
TurnForge is under active development. The architectural core (FSM, Workflows, Overlay) is stable.
[License Information]
