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
- **[Architecture & Patterns](architecture.md)** - Core principles, Command-Decision-Applier pattern, immutable state
- **[Command Flow](command-flow.md)** - Command lifecycle, validation, decision scheduling, ACK system

### State Management
- **[FSM System](fsm-system.md)** - Node types, recursive navigation, auto-navigation, transitions
- **[Orchestrator](orchestrator.md)** - Single mutation point, applier registration, scheduler *[TODO]*

---

## System Pipelines

### Actions & Commands
- **[Action Pipeline](action-pipeline.md)** - ActionCommandHandler → Strategy → Decision → Applier flow
- **[Spawn Pipeline](spawn-pipeline.md)** - SpawnRequest → Descriptor → Strategy → Decision → Factory flow

### Supporting Systems
- **[Board & Spatial](board-spatial.md)** - ISpatialModel, GameBoard, Zones, position validation
-** [Effects System](effects-system.md)** - IGameEffect, effect types, UI propagation
- **[Factory System](factory-system.md)** - GenericActorFactory, PropertyAutoMapper, entity creation

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
