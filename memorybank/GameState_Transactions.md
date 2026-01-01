# GameState Transactions & Mutation

## Overview
TurnForge uses an **Immutable State** architecture modified with a **Transactional Overlay**. This ensures that the global state is never mutated in place dangerously, but efficiently evolves through properly strictly defined transitions.

## The Architecture

### 1. Immutable GameState
The `GameState` class is effectively immutable for consumers.
*   **Collections**: Uses `ImmutableDictionary`.
*   **Access**: Public properties are Read-Only.
*   **Clone**: Removed the dangerous "Deep Clone" method to prevent performance pitfalls.

### 2. GameStateOverlay (The Transaction)
Represents a "Pending State" or a "Diff".
*   Accumulates `IGameStateOperation`s.
*   Does not apply changes immediately.
*   Acting as a recording tape of intentions.

### 3. GameStateBuilder (The Mutator)
The only class allowed to modify internal state.
*   **Pattern**: Copy-On-Write (COW).
*   **Entities**: Initially points to the same entities as the previous state.
*   **Mutation**: When `SetComponent` is called, the specific entity is Cloned, Modified, and the reference updated in the local dictionary.
*   **SpatialIndex**: Clones the Board's index efficiently to allow position changes without affecting the previous state.

## Transaction Flow

The action for modifying game state is strictly functional:

```
(CurrentState) + (Overlay/Operations) => (NewState)
```

### Implementation

```csharp
// 1. Start with a base state
GameState finalState = currentState;

// 2. Create an Overlay to record changes
var overlay = new GameStateOverlay();

// 3. Record Intentions
overlay.Record(new SpawnEntityOperation(orc));
overlay.Record(new MoveEntityOperation(hero, newPos));

// 4. Commit (Atomic Generation)
finalState = overlay.Commit(currentState);
```

### Advantages
1.  **Thread Safety**: You can calculate a turn on a background thread without locking.
2.  **Undo/Redo**: Trivial to implement by storing the chain of States.
3.  **AI Simulation**: AI can fork the state cheaply to simulate "What if" scenarios.
4.  **Performance**: Thanks to Copy-On-Write and Structural Sharing, we avoid the heavy cost of deep cloning the entire world.
