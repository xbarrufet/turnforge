using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
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
    /// Get all entities at a specific board position.
    /// Combines SpatialIndex base state with Overlay pending moves.
    /// </summary>
    public IEnumerable<GameEntity> GetEntitiesAt(IBoardPosition position)
    {
        var result = new HashSet<EntityId>();
        
        // 1. Get entities from SpatialIndex base state
        var baseEntities = _gameState.Board?.SpatialIndex.QueryAt(position) ?? Array.Empty<EntityId>();
        foreach (var entityId in baseEntities)
        {
            // Check if this entity has moved away in the overlay
            if (_overlay.TryGetPosition(entityId, out var overlayPos))
            {
                // Entity has a pending move - only include if it moved TO this position
                if (overlayPos?.Equals(position) == true)
                {
                    result.Add(entityId);
                }
                // Otherwise it moved away, don't include
            }
            else
            {
                // No pending move, include from base state
                result.Add(entityId);
            }
        }
        
        // 2. Add entities that moved TO this position via overlay
        foreach (var entityId in _overlay.GetEntitiesMovedTo(position))
        {
            result.Add(entityId);
        }
        
        // 3. Resolve EntityIds to GameEntities
        foreach (var entityId in result)
        {
            if (!_overlay.IsDestroyed(entityId))
            {
                yield return GetEntity(entityId);
            }
        }
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
    
    /// <summary>
    /// Get all entities owned by a specific player.
    /// Uses TeamComponent.OwnerId for ownership lookup.
    /// </summary>
    public IEnumerable<GameEntity> GetEntitiesForOwner(PlayerId owner)
    {
        foreach (var entity in _gameState.Entities.Values)
        {
            var teamComponent = entity.GetComponent<ITeamComponent>() as TeamComponent;
            if (teamComponent?.OwnerId == owner)
            {
                yield return entity;
            }
        }
    }
    
    /// <summary>
    /// Get entities of a specific definition type owned by a player.
    /// TDefinition should be a definition class like ParchisPawnDefinition.
    /// </summary>
    public IEnumerable<GameEntity> GetEntities<TDefinition>(PlayerId owner)
    {
        var definitionTypeName = typeof(TDefinition).Name.ToLowerInvariant();
        
        foreach (var entity in GetEntitiesForOwner(owner))
        {
            // Check if entity's definition matches the type
            if (entity.DefinitionId.ToLowerInvariant().Contains(definitionTypeName.Replace("definition", "")))
            {
                yield return entity;
            }
        }
    }
    
    /// <summary>
    /// Get all ConnectionEntities that start from a specific tile.
    /// </summary>
    public IEnumerable<GameEntity> GetConnectionsFrom(TileId from)
    {
        foreach (var entity in _gameState.Entities.Values)
        {
            var position = GetEntityPosition(entity);
            if (position is ConnectionPosition cp && cp.From == from)
            {
                yield return entity;
            }
        }
    }
    
    /// <summary>
    /// Get the ConnectionEntity between two specific tiles.
    /// </summary>
    public GameEntity? GetConnection(TileId from, TileId to)
    {
        return GetConnectionsFrom(from)
            .FirstOrDefault(e => 
            {
                var pos = GetEntityPosition(e);
                return pos is ConnectionPosition cp && cp.To == to;
            });
    }
    
    /// <summary>
    /// Get all ConnectionEntities that a team can use from a tile.
    /// Filters by TeamComponent.Team matching the provided team.
    /// </summary>
    public IEnumerable<GameEntity> GetConnectionsForTeam(TileId from, string team)
    {
        foreach (var conn in GetConnectionsFrom(from))
        {
            var teamComponent = conn.GetComponent<ITeamComponent>() as TeamComponent;
            // If no team restriction, or team matches
            if (teamComponent?.Team == null || teamComponent.Team == team)
            {
                yield return conn;
            }
        }
    }
    
    private IBoardPosition? GetEntityPosition(GameEntity entity)
    {
        // Check overlay first, then use PositionComponent
        if (_overlay.TryGetPosition(entity.Id, out var overlayPos))
        {
            return overlayPos;
        }
        
        var posComponent = entity.GetComponent<IPositionComponent>();
        return posComponent?.CurrentPosition;
    }
}