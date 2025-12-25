# Part I: Understanding TurnForge

*How the engine works internally - architecture, patterns, and execution model*

[← Back to Main](../../README.md)

---

## Overview

This section explains TurnForge's internal architecture. Read these docs to understand:
- Design patterns and principles
- Data flow and execution model  
- Internal system pipelines
- Why things work the way they do

**Best for:** Learning the engine, contributing, or understanding design decisions.

---

## Core Architecture

### Foundation
- **[Concepts](concepts.md)** - Engine overview and core philosophy
- **[Architecture](architecture.md)** - Command-Decision-Applier pattern, immutable state
- **[Entity System](entity-system.md)** - ECS, definitions, and descriptors
- **[Command Pipeline](command-pipeline.md)** - Command lifecycle, validation, decision scheduling

### State Management
- **[FSM System](fsm-system.md)** - Node types, recursive navigation, auto-navigation
- **[Orchestrator](orchestrator.md)** - Single mutation point, applier registration, scheduler

### Sub-Systems
- **[Spawn System](spawn-system.md)** - SpawnRequest → Descriptor → Strategy → Decision → Factory
- **[Effects System](effects-system.md)** - IGameEffect, effect types, UI propagation
- **[Action System](action-system.md)** - Action pipeline *[Pending]*
- **[Board System](board-system.md)** - Spatial model and board *[Pending]*

---

## Learning Path

**Recommended order:**
1. Start with [Architecture](architecture.md) to understand core principles
2. Read [Command Flow](command-flow.md) to see how execution works
3. Explore specific pipelines ([Action](action-pipeline.md) / [Spawn](spawn-pipeline.md)) as needed
4. Dive into [FSM System](fsm-system.md) for phase management

**Then move to:**  
→ [Part II: Using TurnForge](../2-using/README.md) to learn the public API

---

[← Back to Main](../../README.md) | [Next: Using TurnForge →](../2-using/README.md)
