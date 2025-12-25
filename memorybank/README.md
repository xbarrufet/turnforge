# TurnForge - Documentation Navigation

**Developer & AI Reference Guide**

[← Back to Project Root](../README.md)

---

## 🎯 Where Do I Start?

**Choose your path:**

- 🆕 **New to TurnForge?** → Start with [Understanding TurnForge](docs/1-understanding/README.md)
- 🛠️ **Building a game?** → Go to [Using TurnForge](docs/2-using/README.md)  
- ⚡ **Need a quick reference?** → Check [API Reference](docs/3-reference/README.md)
- 💡 **Want examples?** → Browse [Examples](examples/)

---

## 📚 Documentation Structure

This documentation is organized into **3 focused sections**:

### Part I: [Understanding TurnForge](docs/1-understanding/README.md)
*Learn how the engine works internally*

- [Architecture & Patterns](docs/1-understanding/architecture.md)
- [Command Flow](docs/1-understanding/command-flow.md)
- [FSM System](docs/1-understanding/fsm-system.md)
- [Action Pipeline](docs/1-understanding/action-pipeline.md)
- [Spawn Pipeline](docs/1-understanding/spawn-pipeline.md)
- [Board & Spatial](docs/1-understanding/board-spatial.md)
- [Effects System](docs/1-understanding/effects-system.md)
- [Factory System](docs/1-understanding/factory-system.md)

**Read this to:** Understand design patterns, execution model, internal systems.

---

### Part II: [Using TurnForge](docs/2-using/README.md)
*Practical API guide for building your game*

- [Getting Started](docs/2-using/getting-started.md)
- [Entity System API](docs/2-using/entities.md)
- [Command System API](docs/2-using/commands.md)
- [Strategy System API](docs/2-using/strategies.md)
- [Component API](docs/2-using/components.md)
- [Services API](docs/2-using/services.md)
- [FSM Configuration](docs/2-using/fsm-config.md)
- [Extension Points](docs/2-using/extension-points.md)

**Read this to:** Learn how to use APIs, create commands, implement strategies, extend the engine.

---

### Part III: [API Reference](docs/3-reference/README.md)
*Quick lookup for interfaces and signatures*

- [Core Interfaces](docs/3-reference/interfaces.md)
- [Command Types](docs/3-reference/commands-ref.md)
- [Strategy Interfaces](docs/3-reference/strategies-ref.md)
- [Component Interfaces](docs/3-reference/components-ref.md)
- [Effect Types](docs/3-reference/effects-ref.md)

**Read this to:** Find method signatures, interface definitions, type references.

---

## 💡 Examples

Learn by example:

- [Basic Move Command](examples/basic-move.md) - Complete movement implementation
- [Custom Strategy](examples/custom-strategy.md) - Zombicide-style zombie blocking
- [Custom Component](examples/custom-component.md) - Creating game-specific components

---

## 🗺️ Navigation Tips

**If you want to...**
- Understand the Command-Decision-Applier pattern → [Architecture](docs/1-understanding/architecture.md)
- Learn how FSM controls game flow → [FSM System](docs/1-understanding/fsm-system.md)
- Implement a custom move strategy → [Strategy System API](docs/2-using/strategies.md)
- Create a new component → [Component API](docs/2-using/components.md)
- Find IActionStrategy signature → [Strategy Interfaces](docs/3-reference/strategies-ref.md)
- See a complete working example → [Examples](examples/)

---

## 📝 Legacy Documentation

- [ENTIDADES.md](ENTIDADES.md) - Original monolithic documentation (deprecated, use sections above)

---

## 🤝 Contributing

Found an error? Have a suggestion? Documentation improvements are welcome!

---

**Happy Building! 🎮**
