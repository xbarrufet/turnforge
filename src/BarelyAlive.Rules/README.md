# BarelyAlive.Rules

This project serves as the **Translation Layer** between the generic `TurnForge.Engine` and the specific implementation of the "Barely Alive" game.

## Architectural Responsibility

Its main role is to act as an intermediary/adapter. It does **not** hold the state of the game (which resides in `GameState` in the Engine) nor the core logic of the turn-based system.

**Key responsibilities:**
1.  **Loader / Adapter**: Converts static data (JSON missions) into Engine Descriptors (`MissionLoader`).
2.  **Projection**: Translates Engine events (`IGameEffect`) into UI-consumable DTOs (`DomainProjector`).
3.  **Specific Definitions**: Provides the `Definition` classes specific to the game (e.g., `SurvivorDefinition`).
4.  **Specific Behaviours**: Implements game mechanics via `ActorBehaviour` and `ZoneBehaviour` extensions.
5.  **Strategies**: Customizes basic Engine processes (like Spawning) with game-specific logic.

## Folder Structure

### `/Adapter`
External data translation.
- `Dto/`: Data Transfer Objects for JSON serialization.
- `Loaders/`: Logic to parse files and convert them into Engine Descriptors.
- `Mappers/`: Utilities to map between DTOs and Engine objects.

### `/Apis`
Public interface for the UI.
- `Handlers/`: Orchestrators for high-level operations (Initialize, StartGame).
- `Messaging/`: Contracts/Payloads sent to the UI.

### `/Core/Domain`
Game specific extensions.
- `Behaviours/`: Custom implementations of `IActorBehaviour` and `IZoneBehaviour`.
- `Definitions/`: Extensions of `GameEntityDefinition` (e.g. `SurvivorDefinition`).
- `Projectors/`: Projectors that transform `IGameEffect` into UI updates.
- `Strategies/`: Implementations of `ISpawnStrategy` and other Engine strategies.
- `ValueObjects/`: Shared helper types (e.g. `Vector`).

## Best Practices

*   **Stateless**: Use the Engine's `GameState` as the single source of truth.
*   **Separation of Concerns**: Use `Projectors` for output and `Commands` for input.
*   **Alignment**: Keep namespaces aligned with the folder structure: `BarelyAlive.Rules.Core.Domain.*`.
