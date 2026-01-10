using System.Collections.Immutable;
using System.Diagnostics;
using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Discrete;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Entities.Players;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

[DebuggerDisplay("Turn {TurnOrder.CurrentTurn}, Player: {TurnOrder.CurrentPlayer}")]
public sealed class GameState
{
    public ImmutableDictionary<EntityId, GameEntity> Entities { get; set; }
    public ImmutableDictionary<PlayerId, Player> Players { get; set; }

    // Indexes for efficient queries
    public ImmutableDictionary<string, List<EntityId>> TeamEntities { get; set; }
    public ImmutableDictionary<PlayerId, List<EntityId>> PlayerEntities { get; set; }
    public ImmutableDictionary<IBoardPositionId, List<EntityId>> PositionEntities { get; set; }
    // Note: SpatialIndex is kept for backward compatibility and complex spatial queries

    public NodeId CurrentStateId { get; set; }
    public IGameBoard? Board { get; }
    public TurnOrderState TurnOrder { get; }
    public bool IsGameStarted
        => Board != null && Players.Count > 0;

    private GameStateOverlay _overlay;

    private GameState? _resetState = null;


    public GameState()
    {
        Entities = ImmutableDictionary<EntityId, GameEntity>.Empty;
        Players = ImmutableDictionary<PlayerId, Player>.Empty;
        TeamEntities = ImmutableDictionary<string, List<EntityId>>.Empty;
        PlayerEntities = ImmutableDictionary<PlayerId, List<EntityId>>.Empty;
        PositionEntities = ImmutableDictionary<IBoardPositionId, List<EntityId>>.Empty;
        CurrentStateId = NodeId.Empty;
        Board = null;
        TurnOrder = TurnOrderState.Empty;
        _overlay = new GameStateOverlay(this);
    }

    /// <summary>
    /// Creates a builder for constructing initial game state.
    /// Use this for game initialization (StartGame action).
    /// For runtime entity creation, use ISpawnService with GameStateOverlay.
    /// </summary>
    public static InitialGameStateBuilder CreateBuilder()
    {
        return new InitialGameStateBuilder();
    }



    public GameState(
        ImmutableDictionary<EntityId, GameEntity> entities,
        ImmutableDictionary<PlayerId, Player> players,
        NodeId currentStateId,
        IGameBoard? board = null,
        TurnOrderState? turnOrder = null)
    {
        Entities = entities;
        Players = players;
        CurrentStateId = currentStateId;
        Board = board;
        TurnOrder = turnOrder ?? TurnOrderState.Empty;
        BuildIndexes();
        _overlay = new GameStateOverlay(this);
    }

    private void BuildIndexes()
    {
        var playerEntities = ImmutableDictionary.CreateBuilder<PlayerId, List<EntityId>>();
        var positionEntities = ImmutableDictionary.CreateBuilder<IBoardPositionId, List<EntityId>>();

        foreach (var entity in Entities.Values)
        {
            // Only Actors have positions now (direct property)
            if (entity is IActor actor && actor.CurrentPosition != null)
            {
                var position = actor.CurrentPosition;
                if (!positionEntities.ContainsKey(position))
                {
                    positionEntities[position] = new List<EntityId>();
                }

                positionEntities[position].Add(entity.Id);
            }

            PlayerEntities = playerEntities.ToImmutable();
            PositionEntities = positionEntities.ToImmutable();
        }
    }



    public static GameState Empty()
    {
        return new GameState(
            ImmutableDictionary<EntityId, GameEntity>.Empty,
            ImmutableDictionary<PlayerId, Player>.Empty,
            NodeId.Empty);
    }


    /// *******************************************************************
    /// GAME STATE MANIPULATION
    /// *******************************************************************

    public GameState CommitOverlayChanges()
    {
        if (_resetState == null)
            return _overlay.Commit();
        return _resetState;
    }

    public void RecordOverlayOperation(IGameStateOperation operation)
    {
        _overlay.Record(operation);
    }




    /// *******************************************************************
    /// QUERIES SECTION
    /// *******************************************************************


    /// *******************************************************************
    /// Overlay filters
    /// /// *******************************************************************

    public IEnumerable<EntityId> FilterEntityIds(IEnumerable<EntityId> entityIds)
    {
        return _overlay.FilterEntityIds(entityIds);
    }

    public IEnumerable<TGameEntity> GetOverlayedEntities<TGameEntity>(IEnumerable<TGameEntity> entities) where TGameEntity : GameEntity
    {
        return _overlay.FilterEntities<TGameEntity>(entities);
    }

    public IEnumerable<GameEntity> GetOverlayedEntities(IEnumerable<GameEntity> entities)
    {
        return _overlay.FilterEntities(entities);
    }

    /// <summary>
    /// Check if entity is destroyed in overlay.
    /// </summary>
    internal bool IsEntityDestroyed(EntityId id)
    {
        return _overlay.IsDestroyed(id);
    }



    public TGameEntity GetOverlayedEntity<TGameEntity>(EntityId id) where TGameEntity : GameEntity
    {
        // 1. Check Overlay for pending changes (Creation, Modification, Deletion)
        if (_overlay.TryGetEntity(id, out var overlayEntity, out var isDestroyed))
        {
            if (isDestroyed) throw new KeyNotFoundException($"Entity {id} has been destroyed in pending transaction.");
            if (overlayEntity != null) return (TGameEntity)overlayEntity;
        }
        // 2. Fallback to base state
        if (Entities.TryGetValue(id, out var entity))
        {
            return (TGameEntity)entity;
        }

        throw new KeyNotFoundException($"Entity {id} not found in state or overlay.");
    }

    public GameEntity GetOverlayedEntity(EntityId id)
    {
        // 1. Check Overlay for pending changes (Creation, Modification, Deletion)
        if (_overlay.TryGetEntity(id, out var overlayEntity, out var isDestroyed))
        {
            if (isDestroyed) throw new KeyNotFoundException($"Entity {id} has been destroyed in pending transaction.");
            if (overlayEntity != null) return overlayEntity;
        }
        // 2. Fallback to base state
        if (Entities.TryGetValue(id, out var entity))
        {
            return entity;
        }

        throw new KeyNotFoundException($"Entity {id} not found in state or overlay.");
    }


    /// *******************************************************************
    /// Player queries.  TODO; Overlay support
    /// *******************************************************************
    /// <summary>
    /// Gets a player by their PlayerId
    /// </summary>
    public Player GetPlayerByPlayerId(PlayerId playerId)
    {
        if (Players.TryGetValue(playerId, out var player))
            return player;
        throw new KeyNotFoundException($"Player {playerId} not found in state or overlay.");
    }

    /// <summary>
    /// Gets the current player
    /// </summary>
    public Player GetCurrentPlayer()
    {
        return GetPlayerByPlayerId(TurnOrder.CurrentPlayer);
    }

    /// <summary>
    /// Checks if the current player has available actions
    /// </summary>
    public bool CurrentPlayerHasAvailableActions()
    {
        var player = GetCurrentPlayer();
        if (player == null || player.ActionPool == null)
            throw new InvalidOperationException("Current player not found in state or overlay or has no action pool.");
        return player.ActionPool.HasEnoughActions();
    }



    /// *******************************************************************
    /// Position queries
    /// *******************************************************************

    /// <summary>
    /// Get all entities at a specific position, considering overlay modifications.
    /// Uses PositionEntities index for base lookups, then applies overlay filter.
    /// </summary>
    public IEnumerable<GameEntity> GetEntitiesAt(IBoardPosition position)
    {
        // 1. Get entity IDs from PositionEntities index (base state)
        var baseEntityIds = PositionEntities.TryGetValue(position.Id, out var ids)
            ? (IEnumerable<EntityId>)ids
            : Array.Empty<EntityId>();

        // 2. Build complete set of entity IDs at this position
        var entityIdsAtPosition = new HashSet<EntityId>();

        foreach (var entityId in baseEntityIds)
        {
            // Check if this entity has moved away in the overlay
            if (_overlay.TryGetPosition(entityId, out var overlayPos))
            {
                // Entity has a pending move - only include if it moved TO this position
                if (overlayPos?.Equals(position) == true)
                {
                    entityIdsAtPosition.Add(entityId);
                }
                // Otherwise it moved away, don't include
            }
            else
            {
                // No pending move, include from base state
                entityIdsAtPosition.Add(entityId);
            }
        }

        // 3. Add entities that moved TO this position via overlay
        foreach (var entityId in _overlay.GetEntitiesMovedTo(position))
        {
            entityIdsAtPosition.Add(entityId);
        }

        // 4. Filter out destroyed entities and return overlayed versions
        var validIds = FilterEntityIds(entityIdsAtPosition);

        foreach (var id in validIds)
        {
            yield return GetOverlayedEntity(id);
        }
    }

    /// <summary>
    /// Get all entities at a specific position (alias for backward compatibility).
    /// </summary>
    public IEnumerable<GameEntity> GetOverlayedEntitiesAt(IBoardPosition position)
        => GetEntitiesAt(position);

    /// <summary>
    /// Get all entities of a specific type.
    /// Uses lazy evaluation via yield return for better memory efficiency.
    /// </summary>
    public IEnumerable<TGameEntity> GetEntitiesByType<TGameEntity>()
     where TGameEntity : GameEntity
    {
        foreach (var entity in Entities.Values)
        {
            if (entity is TGameEntity)
            {
                // Get the overlayed version of this specific entity
                var overlayedEntity = GetOverlayedEntity<TGameEntity>(entity.Id);
                yield return overlayedEntity;
            }
        }
    }

    public IEnumerable<TGameEntity> GetEntitiesByTeam<TGameEntity>(string team) where TGameEntity : GameEntity
    {
        if (!TeamEntities.TryGetValue(team, out var entityIds))
            yield break;

        // Filter out destroyed entities first (efficient)
        var validIds = FilterEntityIds(entityIds);

        foreach (var id in validIds)
        {
            var entity = GetOverlayedEntity(id);
            if (entity is TGameEntity typedEntity)
                yield return typedEntity;
        }
    }
    /// <summary>
    /// Get all entity IDs of a specific type belonging to a team.
    /// Uses lazy evaluation via yield return for better memory efficiency.
    /// </summary>
    public IEnumerable<EntityId> GetEntityIdsByTeam<TGameEntity>(string team) where TGameEntity : GameEntity
    {
        if (!TeamEntities.TryGetValue(team, out var entityIds)) yield break;

        // Filter out destroyed entities first
        var validIds = FilterEntityIds(entityIds);

        foreach (var id in validIds)
        {
            var entity = GetOverlayedEntity(id);
            if (entity is TGameEntity)
                yield return id;
        }
    }

    public IEnumerable<EntityId> GetEntitiesByPlayer(PlayerId playerId)
    {
        if (!PlayerEntities.TryGetValue(playerId, out var entityIds))
            return Array.Empty<EntityId>();

        // Filter out destroyed entities
        return FilterEntityIds(entityIds);
    }

    /// <summary>
    /// Get all entity IDs of a specific type belonging to a player.
    /// Uses lazy evaluation via yield return for better memory efficiency.
    /// </summary>
    public IEnumerable<EntityId> GetEntityIdsByPlayer<TGameEntity>(PlayerId playerId) where TGameEntity : GameEntity
    {
        if (!PlayerEntities.TryGetValue(playerId, out var entityIds)) yield break;

        // Filter out destroyed entities first
        var validIds = FilterEntityIds(entityIds);

        foreach (var id in validIds)
        {
            var entity = GetOverlayedEntity(id);
            if (entity is TGameEntity)
                yield return id;
        }
    }

    public void SetResetState(GameState resetState)
    {
        _resetState = resetState;
    }


    public IDiscreteZoneTopology GetDiscreteTopologyBoard()
    {
        if (Board is IDiscreteZoneTopology discreteBoard)
        {
            return discreteBoard;
        }
        throw new InvalidOperationException("Game board is not a discrete topology.");
    }




}





