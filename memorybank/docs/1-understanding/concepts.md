# Overview & Core Principles

TurnForge is a turn-based tactical game engine built on immutable state and clear separation of concerns.

## What is TurnForge?

A C# engine for creating tactical turn-based games (skirmish wargames, tactical RPGs, etc.). It provides:

- **FSM-driven game flow** - Manage phases, turns, and state transitions declaratively
- **Component-based entities** - Flexible agent/prop system with reusable components
- **Command pattern** - Separate user intent from execution logic
- **Strategy extensibility** - Customize game rules without modifying the engine

## Core Components

- **FSM** - Finite State Machine managing turn flow and valid actions per phase
- **Entities** - Agents (active) and Props (static) with component-based data
- **Commands** - User/AI intentions (Move, Attack, Spawn, etc.)
- **Strategies** - Configurable business logic for processing commands
- **Decisions** - Intent-to-execution bridge (what should happen)
- **Appliers** - State mutators (make it happen)
- **Effects** - UI notifications (show what happened)
