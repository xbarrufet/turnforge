using System.Runtime.InteropServices;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.Entities.Players;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

/// <summary>
/// Copy-on-Write overlay for tracking pending changes to game state.
/// Clones entities on first modification and applies changes in-place.
/// Maintains event log for UI animations/events.
/// </summary>
public sealed class GameStateOverlay
{
    // Reference to base state for cloning entities
    private readonly GameState _baseState;

    // Event log for UI (maintains full operation history)
    private readonly List<IGameStateOperation> _eventLog = new();

    // Modified entities (Copy-on-Write cache)
    private readonly Dictionary<EntityId, GameEntity> _modified = new();

    // Modified players (Copy-on-Write cache)
    private readonly Dictionary<PlayerId, Player> _modifiedPlayers = new();

    // Destroyed entity IDs
    private readonly HashSet<EntityId> _destroyed = new();

    // Position changes (for spatial queries)
    private readonly Dictionary<EntityId, IBoardPosition> _positionChanges = new();

    // Cloned board for spatial index updates
    private IGameBoard? _board;

    // Pending turn order change
    private TurnOrderState? _newTurnOrder;



    public GameStateOverlay(GameState baseState)
    {
        _baseState = baseState;
        // Clone board lazily on first position change
    }




    /// <summary>
    /// Record an operation. Clones entity if first modification, then applies in-place.
    /// </summary>
    public void Record(IGameStateOperation op)
    {
        // 1. Always add to event log for UI
        _eventLog.Add(op);

        // 2. Apply operation based on type
        switch (op)
        {
            case SpawnEntityOperation spawn:
                HandleSpawn(spawn);
                break;

            case MoveOperation move:
                HandleMove(move);
                break;

            case DestroyOperation destroy:
                HandleDestroy(destroy);
                break;

            case SetTurnOrderOperation setTurnOrder:
                HandleSetTurnOrder(setTurnOrder);
                break;

            case SpendAPOperation spendAP:
                HandleSpendAP(spendAP);
                break;

            case NextTurnResetApOperation resetPlayerAP:
                HandleNextTurnResetAp(resetPlayerAP);
                break;

            case AddPlayerOperation addPlayer:
                _modifiedPlayers[addPlayer.Player.PlayerId] = addPlayer.Player;
                break;

            case CreateBoardOperation createBoard:
                _board = createBoard.Board;
                break;

            default:
                // For unknown operations, get mutable entity and apply via mutator
                ApplyViaBuilder(op);
                break;
        }
    }

    private void HandleSpawn(SpawnEntityOperation spawn)
    {
        // New entities go directly to _modified
        _modified[spawn.EntityId] = spawn.Entity;

        // If position specified, record the position change
        if (spawn.Position != null)
        {
            _positionChanges[spawn.EntityId] = spawn.Position;
        }
    }

    private void HandleMove(MoveOperation move)
    {
        // Clone entity if needed (for consistency)
        EnsureEntityCloned(move.EntityId);

        // Record position change
        _positionChanges[move.EntityId] = move.NewPosition;
    }

    private void HandleDestroy(DestroyOperation destroy)
    {
        _destroyed.Add(destroy.EntityId);
        _modified.Remove(destroy.EntityId);
        // Position will be removed when GameState is rebuilt
    }

    private void HandleSetTurnOrder(SetTurnOrderOperation op)
    {
        _newTurnOrder = op.NewTurnOrder;
    }

    private void HandleSpendAP(SpendAPOperation op)
    {
        var targetPlayer = _modifiedPlayers.TryGetValue(op.PlayerId, out Player? player)
            ? player
            : _baseState.GetPlayerByPlayerId(op.PlayerId)?.Clone();

        if (targetPlayer == null)
            return;
        targetPlayer.ActionPool.ConsumeActions(op.EntityId, op.amouunt);
        _modifiedPlayers[op.PlayerId] = targetPlayer;
    }

    private void HandleNextTurnResetAp(NextTurnResetApOperation op)
    {
        foreach (var player in _baseState.Players.Values)
        {
            player.ActionPool.ResetAction();
            _modifiedPlayers[player.PlayerId] = player;
        }
        _newTurnOrder = _baseState.TurnOrder.NextRound();
    }


    private void ApplyViaBuilder(IGameStateOperation op)
    {
        // Fallback for operations we don't handle directly
        // This maintains backward compatibility
        EnsureEntityCloned(op.EntityId);
        // The operation will be applied during Commit()
    }

    private void EnsureEntityCloned(EntityId id)
    {
        if (_modified.ContainsKey(id) || _destroyed.Contains(id))
            return;

        if (_baseState.Entities.TryGetValue(id, out var entity))
        {
            _modified[id] = entity.Clone();
        }
    }

    private void EnsureBoardCloned()
    {
        if (_board == null && _baseState.Board != null)
        {
            _board = _baseState.Board.Clone();
        }
    }

    /// <summary>
    /// Commit all pending changes to a new immutable GameState.
    /// </summary>
    public GameState Commit()
    {
        var builder = new GameStateBuilder(_baseState);

        // Apply destroyed entities
        foreach (var id in _destroyed)
        {
            builder.RemoveEntity(id);
        }

        // Apply modified/new entities
        foreach (var entity in _modified.Values)
        {
            if (!_destroyed.Contains(entity.Id))
            {
                builder.AddEntity(entity);
            }
        }

        // Position changes are already reflected in entity components
        // No need to call SetEntityPosition

        // Use our cloned board if we modified it
        if (_board != null)
        {
            builder.SetBoard(_board);
        }

        if (_newTurnOrder != null)
        {
            builder.SetTurnOrder(_newTurnOrder);
        }

        // Apply modified players
        foreach (var modifiedPlayer in _modifiedPlayers.Values)
        {
            builder.AddOrUpdatePlayer(modifiedPlayer);
        }

        return builder.Build();
    }

    /// <summary>
    /// Get entity from overlay (modified) or base state.
    /// </summary>
    public bool TryGetEntity(EntityId id, out GameEntity? entity, out bool isDestroyed)
    {
        entity = null;
        isDestroyed = _destroyed.Contains(id);

        if (isDestroyed)
            return true;

        if (_modified.TryGetValue(id, out entity))
            return true;

        return false;
    }

    /// <summary>
    /// Get pending position for an entity.
    /// </summary>
    public bool TryGetPosition(EntityId id, out IBoardPosition? position)
    {
        position = null;

        if (_destroyed.Contains(id))
            return true; // Entity destroyed, position is null

        if (_positionChanges.TryGetValue(id, out position))
            return true;

        return false;
    }

    /// <summary>
    /// Check if entity has been destroyed in this overlay.
    /// </summary>
    public bool IsDestroyed(EntityId id) => _destroyed.Contains(id);

    /// <summary>
    /// Get all entities that have moved TO a specific position in this overlay.
    /// </summary>
    public IEnumerable<EntityId> GetEntitiesMovedTo(IBoardPosition position)
    {
        foreach (var kvp in _positionChanges)
        {
            if (kvp.Value.Equals(position))
            {
                yield return kvp.Key;
            }
        }
    }

    private bool IsNewEntity(EntityId id)
    {
        return _modified.ContainsKey(id) && !_baseState.Entities.ContainsKey(id);
    }

    /// <summary>
    /// Get the event log for UI animations/projections.
    /// </summary>
    public IReadOnlyList<IGameStateOperation> GetEvents() => _eventLog;

    /// <summary>
    /// Filter entity IDs to exclude destroyed entities.
    /// Very efficient - only HashSet lookups (O(1) per ID).
    /// </summary>
    internal IEnumerable<EntityId> FilterEntityIds(IEnumerable<EntityId> entityIds)
    {
        foreach (var id in entityIds)
        {
            // Skip if destroyed in overlay
            if (!_destroyed.Contains(id))
                yield return id;
        }
    }

    /// <summary>
    /// Filter entities to include only those that have been modified or are new.
    /// </summary>
    internal IEnumerable<TGameEntity> FilterEntities<TGameEntity>(IEnumerable<TGameEntity> entities) where TGameEntity : GameEntity
    {
        foreach (var entity in entities)
        {
            if (_modified.TryGetValue(entity.Id, out GameEntity? value))
                yield return (TGameEntity)value;
            else if (!_destroyed.Contains(entity.Id))
                yield return entity;
        }
    }

}
