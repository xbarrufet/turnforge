# TurnForge Product Roadmap

> **Vision:** A generic turn-based game engine that allows rapid implementation of tactical skirmish games.

---

## 🎯 Version 1.0 (MVP)

**Goal:** Implement a complete playable game loop with core tactical mechanics.

### Core Systems

| System | Description | Status |
|--------|-------------|--------|
| **Board** | Tile-based game board with spatial queries | ✅ Done |
| **Entities** | Agents, Props, GameState | ✅ Done |
| **Turns & Phases** | Phase-based turn system with automatic end conditions | ✅ Done |
| **Factions** | Entity categories (Survivors, Zombies, etc.) | ✅ Done |
| **Movement** | AP-based movement with board validation | ✅ Done |
| **Combat** | Attack actions, damage calculation | ⚪ Backlog |
| **Weapons/Armor** | Equipment affecting combat stats | ⚪ Backlog |

### Dependencies (Features needed)

- [ ] FEATURE-005: BoardQueryService (for pathfinding, LoS)
- [ ] FEATURE-012: DiceThrowService ✅ DONE
- [ ] IDEA-005: Combat System (needs design)
- [ ] Equipment components (weapon stats, armor)

---

## 🔧 Version 1.1

**Goal:** Rich world interaction and persistent effects.

### Features

| Feature | Description | Related |
|---------|-------------|---------|
| **Prop Actions** | Open, activate, interact with Props | New Commands |
| **Inventory** | Agents carry/manage Items | FEATURE-013 |
| **Loot System** | Items in containers, pickup/drop | FEATURE-013 |
| **Item Types** | Consumables, keys, equipment | FEATURE-013 |
| **Effects/Buffs** | Temporal effects as Traits | IDEA-001 |

### Dependencies

- [ ] FEATURE-013: Items & Inventory System
- [ ] IDEA-001: Effects As Traits (needs design)
- [ ] Prop interaction commands

---

## 🚀 Version 1.2

**Goal:** Mission structure and advanced mechanics.

### Features

| Feature | Description | Related |
|---------|-------------|---------|
| **Mission Goals** | Victory/defeat conditions | New System |
| **Custom Termination** | Game-defined end states | Extends FSM |
| **Drop Carrying** | Agents carry mission objects | Extends Inventory |
| **Interrupted Actions** | Actions cancelled mid-execution | Extends Commands |

### Dependencies

- [ ] Mission/Objective system design
- [ ] Extended FSM for custom termination
- [ ] Inventory extensions for drops

---

## 📊 Current Progress

```
Version 1.0: ████████░░░░░░░░ ~50%
Version 1.1: ░░░░░░░░░░░░░░░░ 0%
Version 1.2: ░░░░░░░░░░░░░░░░ 0%
```

### What's Done (Engine)
- ✅ Entity system (Agents, Props)
- ✅ Component architecture
- ✅ Command/Decision/Applier pipeline
- ✅ Turn phases and AP system
- ✅ Movement strategies
- ✅ Board and spatial queries
- ✅ DiceThrowService

### What's Missing for 1.0
- ⚪ Combat system (attack commands, damage)
- ⚪ Weapon/Armor equipment
- ⚪ Line of Sight
- ⚪ Cover system (optional)

---

## ❓ Open Questions

1. **Combat:** Should combat be engine-level or game-specific (BarelyAlive)?
2. **Equipment:** Weapons as Items (1.1) or as Components (1.0)?
3. **Priorities:** Focus entirely on 1.0 before starting 1.1?
