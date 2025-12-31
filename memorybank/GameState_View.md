# GameState View Implementation

## Overview
The **GameStateView** is the pattern used by workflows and external systems to inspect the game state. It acts as a smart layer that unifies the committed (immutable) `GameState` with the pending modifications (mutable) in the `GameStateOverlay`.

## Why is this needed?
When a workflow executes multiple steps (e.g., *Check Range* -> *Calculate Damage* -> *Apply Damage*), the intermediate steps are recorded in the Overlay but not yet committed to the State.
Without `GameStateView`, a subsequent step in the same transaction wouldn't "see" changes made by a previous step (like an entity moving or dying).

## How it works

The View follows a strictly ordered resolution strategy:
1.  **Overlay (Pending)**: Checks if the entity exists or has been modified/destroyed in the current transaction.
2.  **Base State (Committed)**: If no pending changes exist, it falls back to the official immutable state.

### Code Example

```csharp
public GameEntity GetEntity(EntityId id)
{
    // 1. Check Overlay for pending changes
    if (_overlay.TryGetEntity(id, out var overlayEntity, out var isDestroyed))
    {
        if (isDestroyed) throw new Exception("Entity destroyed");
        if (overlayEntity != null) return overlayEntity;
    }

    // 2. Fallback to base state
    return _gameState.Entities[id];
}
```

## Overlay Logic (`TryGetEntity`)
The `GameStateOverlay` maintains an index (`EntityOverlayIndex`) of operations targeting each entity. `TryGetEntity` replays relevant operations (like Spawns) to return the temporary version of the entity.

## Benefits
*   **Consistency**: Workflows see a consistent world state even midway through a complex transaction.
*   **Isolation**: The main `GameState` remains pristine until `Commit()` is called.
*   **Performance**: Only looks up modified entities in the overlay; most queries fall through to the efficient immutable dictionary.
