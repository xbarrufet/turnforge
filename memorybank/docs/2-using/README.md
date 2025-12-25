# Part II: Using TurnForge

*Practical API guide for building your game*

[← Back to Main](../../README.md)

---

## Overview

This section provides hands-on guides for using TurnForge's public API. Read these docs to:
- Get started quickly
- Implement game features
- Use commands, strategies, and components
- Extend and customize the engine

**Best for:** Building your game, implementing features, API reference.

---

## Getting Started

- **[Getting Started](getting-started.md)** - Installation, minimal setup, first command *[TODO]*

---

### Entity Management
- **[Getting Started](getting-started.md)** - Setting up the engine and game loop.
- **[Entities & Spawning](entities.md)** - Defining and creating entities.
- **[Components](components.md)** - Working with entity data.
- **[Extensions](extensions.md)** - Common patterns and builders.

### Game Logic
- **[Sending Commands](commands.md)** - Implementing the Command-Handler-Applier pattern.
- **[Phases & FSM](phases.md)** - Configuring the game loop states.
- **[Strategies](strategies.md)** - Implementing business rules.
- **[Services](services.md)** - Querying state and utilities.
- **[Dynamic Attributes](attributes.md)** - Data-driven stat system.

---

## Configuration & Extension

- **[FSM Configuration](fsm-config.md)** - Creating custom nodes, transitions, phase management *[TODO]*
- **[Extension Points](extension-points.md)** - Custom handlers, appliers, spatial models, effects *[TODO]*

---

## Quick Links

**Common Tasks:**
- Implement custom movement strategy → [Strategy System API](strategies.md)
- Query agents by category → [Services API](services.md)
- Create custom component → [Component API](components.md)
- Register custom command → [Command System API](commands.md)

**Need help?** Check [Examples](../../examples/) for complete working samples.

---

[← Back to Main](../../README.md) | [← Previous: Understanding](../1-understanding/README.md) | [Next: Reference →](../3-reference/README.md)
