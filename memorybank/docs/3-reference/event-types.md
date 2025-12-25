# Event Types

This reference lists the core **GameEvents** available in TurnForge.

## Core Interface
`IGameEvent`: Base interface for all output notifications.
`GameEvent`: Abstract record providing common metadata (`Origin`, `Timestamp`).
`EventOrigin`: Enum (`System`, `Player`, `AI`, `Rule`).

## Concrete Events

### Entity Lifecycle
- **`EntitySpawnedEvent`**: An entity was created and added to the state.
- **`PropSpawnedEvent`**: A prop was created.
- **`AgentSpawnedEvent`**: An agent was created.

### Updates
- **`ComponentsUpdatedEvent`**: One or more components of an entity have changed.
- **`BoardCreatedEvent`**: The board has been initialized or loaded.
- **`BoardInitializedEvent`**: (Legacy/Specific) Board setup complete.

### FSM
- **`FSMTransitionEvent`**: The game state machine transitioned to a new state/node.

## Usage
Events are immutable. Do not modify them. Use Projectors to listen to them and update domain-specific views or external systems.
