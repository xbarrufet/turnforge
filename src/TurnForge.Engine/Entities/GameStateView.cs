using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public sealed class GameStateView
{

    private readonly GameState _gameState;
    private readonly GameStateOverlay _overlay;

    public GameStateView(GameState gameState, GameStateOverlay overlay) {
        _gameState = gameState;
        _overlay = overlay;
    }

    public GameEntity GetEntity(EntityId id)
    {
        // 1. Check Overlay for pending changes (Creation, Modification, Deletion)
        if (_overlay.TryGetEntity(id, out var overlayEntity, out var isDestroyed))
        {
            if (isDestroyed) throw new KeyNotFoundException($"Entity {id} has been destroyed in pending transaction.");
            if (overlayEntity != null) return overlayEntity;
        }

        // 2. Fallback to base state
        if (_gameState.Entities.TryGetValue(id, out var entity))
        {
            return entity;
        }

        throw new KeyNotFoundException($"Entity {id} not found in state or overlay.");
    }
    
    /// <summary>
    /// Get position for an entity. Checks overlay first, then base state.
    /// </summary>
    public IBoardPosition? GetPosition(EntityId id)
    {
        // 1. Check overlay for pending move
        if (_overlay.TryGetPosition(id, out var overlayPosition))
        {
            return overlayPosition;
        }
        
        // 2. Fallback to base state (via board spatial index)
        return _gameState.Board?.SpatialIndex.GetEntityPosition(id);
    }

 
}