# Board Management System

## Overview
The Board system has been split into two distinct responsibilities: **Topology** (Static) and **Spatial Index** (Dynamic). This separation is crucial for the Copy-On-Write (COW) architecture of the GameState.

## Core Concepts

### 1. IBoardTopology (Static & Immutable)
*   **Responsibility**: Defines the graph structure, connections, distances, and validity of positions.
*   **State**: Immutable once created. Shared across all GameStates.
*   **Key**: Does *not* know about entities.

### 2. ISpatialIndex (Dynamic & Mutable)
*   **Responsibility**: Tracks where entities are located (`EntityId <-> BoardPosition`).
*   **State**: Mutable. Each `GameState` needs its own version of the index to ensure thread safety and history isolation.
*   **Optimization**: Uses dictionary-based cloning for extreme performance during state forking.

### 3. IGameBoard (Container)
Acts as the facade that unifies Topology and SpatialIndex.

## Cloning Strategy (The "Smart Copy")

When `GameState` is cloned (e.g., for AI thinking or Transacion planning), the Board is handled as follows:

1.  **Topology**: Copied by reference (pointer). Cost: O(1).
2.  **SpatialIndex**:
    *   **Deep Clone**: The internal dictionaries (`_entityPositions`, `_positionEntities`) are copied.
    *   **Why?**: Because `EntityId` and `IBoardPosition` are Structs (ValueTypes), copying the valid memory blocks is safe and fast.
    *   **Benefit**: We don't need to re-insert simple entities one by one.

```csharp
public GameBoard(GameBoard other)
{
    Id = other.Id;
    Kind = other.Kind;
    
    // Shared Reference (Cheap)
    Topology = other.Topology; 
    
    // Efficient Dictionary Clone (Fast Memcpy)
    SpatialIndex = other.SpatialIndex.Clone();
}
```
