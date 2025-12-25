# Architecture Deep Dive

TurnForge implements a **Command-Decision-Applier** pattern with FSM-controlled state transitions.

## Core Principles

| Principle | Implementation |
|-----------|----------------|
| **Single Mutation Point** | Only `Orchestrator` mutates `GameState` |
| **Immutable State** | All state changes create new instances |
| **Ordered Execution** | Commands → Decisions → Appliers → Effects |
| **Phase Control** | FSM validates commands per game phase |
| **Decoupled Logic** | Handlers generate decisions, Appliers execute them |

## Data Flow

```
Command → Validation → Handler → Decision → Scheduler → Applier → State + Effects
```

## Key Classes

- `GameEngineRuntime`: Entry point, orchestrates all components
- `FsmController`: State machine, validates commands, manages transitions
- `TurnForgeOrchestrator`: Routes decisions to appliers, mutates state
- `CommandBus`: Dispatches commands to handlers
- `GameState`: Immutable game state snapshot
