using System.Runtime.InteropServices;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Entities.Overlay.Operations;
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
    
    // Destroyed entity IDs
    private readonly HashSet<EntityId> _destroyed = new();
    
    // Cloned board for spatial index updates
    private IGameBoard? _board;
    
    // Pending turn order change
    private TurnOrderState? _newTurnOrder;
    
    // Pending player AP changes
    private readonly List<SpendAPOperation> _pendingAPChanges = new();
    private readonly List<AddPlayerOperation> _pendingPlayerAdditions = new();
    
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

            case SetBoardOperation setBoard:
                HandleSetBoard(setBoard);
                break;

            case SetTurnOrderOperation setTurnOrder:
                HandleSetTurnOrder(setTurnOrder);
                break;
            
            case SpendAPOperation spendAP:
                HandleSpendAP(spendAP);
                break;
                
            case AddPlayerOperation addPlayer:
                _pendingPlayerAdditions.Add(addPlayer);
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
        _modified[spawn.Target] = spawn.NewEntity;
        
        // If position specified, update spatial index
        if (spawn.Position != null)
        {
            EnsureBoardCloned();
            _board?.SpatialIndex.Register(spawn.Target, spawn.Position);
        }
    }
    
    private void HandleMove(MoveOperation move)
    {
        // Clone entity if needed (for consistency, even though move only affects spatial index)
        EnsureEntityCloned(move.Target);
        
        // Update spatial index
        EnsureBoardCloned();
        _board?.SpatialIndex.Update(move.Target, move.NewPosition);
    }
    
    private void HandleDestroy(DestroyOperation destroy)
    {
        _destroyed.Add(destroy.Target);
        _modified.Remove(destroy.Target);
        
        // Remove from spatial index
        EnsureBoardCloned();
        _board?.SpatialIndex.Unregister(destroy.Target);
    }

    private void HandleSetBoard(SetBoardOperation op)
    {
        // Directly replace the overlaid board
        _board = op.NewBoard;
        // Note: New board starts fresh, so we don't need to replay previous spatial updates on it
        // unless we want to migrate entities. For StartGame, assume fresh board implies checking entities later.
        // Actually, if entities exist, they might be in invalid positions?
        // For StartGame, entities are usually spawned AFTER board is set.
    }

    private void HandleSetTurnOrder(SetTurnOrderOperation op)
    {
        _newTurnOrder = op.NewTurnOrder;
    }
    
    private void HandleSpendAP(SpendAPOperation op)
    {
        _pendingAPChanges.Add(op);
    }
    
    private void ApplyViaBuilder(IGameStateOperation op)
    {
        // Fallback for operations we don't handle directly
        // This maintains backward compatibility
        EnsureEntityCloned(op.Target);
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
    /// Commit all changes to create a new GameState.
    /// Uses pre-cloned entities from _modified for efficiency.
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
        foreach (var kvp in _modified)
        {
            if (!_destroyed.Contains(kvp.Key))
            {
                builder.AddEntity(kvp.Value);
            }
        }
        
        // Use our cloned board if we modified spatial index
        if (_board != null)
        {
            builder.SetBoard(_board);
        }

        if (_newTurnOrder != null)
        {
            builder.SetTurnOrder(_newTurnOrder);
        }
        
        // Apply pending AP changes
        foreach (var apChange in _pendingAPChanges)
        {
            builder.SpendPlayerAP(apChange.PlayerId, apChange.Amount, apChange.IsBonusTurn);
        }
        
        // Apply pending Player additions
        foreach (var addPlayer in _pendingPlayerAdditions)
        {
            builder.AddPlayer(addPlayer.Player);
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
            
        if (_board != null)
        {
            position = _board.SpatialIndex.GetEntityPosition(id);
            return position != null;
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if entity has been destroyed in this overlay.
    /// </summary>
    public bool IsDestroyed(EntityId id) => _destroyed.Contains(id);
    
    /// <summary>
    /// Get all entity IDs that have pending moves TO a specific position.
    /// Used for spatial queries.
    /// </summary>
    public IEnumerable<EntityId> GetEntitiesMovedTo(IBoardPosition position)
    {
        if (_board == null)
            yield break;
            
        foreach (var id in _board.SpatialIndex.QueryAt(position))
        {
            // Only return if this is a newly moved entity (in overlay)
            if (_modified.ContainsKey(id) || IsNewEntity(id))
            {
                yield return id;
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
}

/// <summary>
/// Operation to destroy/remove an entity from the game.
/// </summary>
public sealed class DestroyOperation : IGameStateOperation
{
    public EntityId Target { get; }
    
    public DestroyOperation(EntityId entityId)
    {
        Target = entityId;
    }
    
    public void Apply(IGameStateMutator mutator)
    {
        mutator.MarkDestroyed(Target);
    }
}
