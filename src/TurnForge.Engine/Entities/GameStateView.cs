using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public sealed class GameStateView
{

    private readonly GameState _gameState;


    public GameStateView(GameState gameState)
    {
        _gameState = gameState;
    }

    /// <summary>
    /// Access to underlying state. For internal orchestrator use.
    /// </summary>
    internal GameState BaseState => _gameState;

    /// <summary>
    /// Record an operation to the overlay. Changes are pending until commit.
    /// </summary>
    public void RecordOperation(IGameStateOperation operation)
    {
        // Log the operation being recorded
        Console.WriteLine($"Operation: {operation.GetType().Name} -> {FormatOperationParameters(operation)}");

        _gameState.RecordOverlayOperation(operation);
    }

    private string FormatOperationParameters(IGameStateOperation operation)
    {
        // Format operation parameters for logging
        return operation switch
        {
            MoveOperation move => $"Entity={move.EntityId}, NewPos={move.NewPosition}",
            SpendAPOperation spendAP => $"Player={spendAP.PlayerId}, AP={spendAP.amouunt}",
            SetTurnOrderOperation turnOrder => $"Round={turnOrder.NewTurnOrder.RoundNumber}, Player={turnOrder.NewTurnOrder.CurrentPlayer}",
            SpawnEntityOperation spawn => $"Entity={spawn.Entity.Name}, Pos={spawn.Position}",
            _ => operation.ToString() ?? "Unknown"
        };
    }

    /// <summary>
    /// Start a fluent LINQ-style query on entities.
    /// Queries are overlay-aware and return the current state including pending changes.
    /// </summary>


    public void ResetState(GameState state)
    {
        _gameState.SetResetState(state);

    }

    public GameEntity GetEntity(EntityId id)
    {
        return _gameState.GetOverlayedEntity(id);
    }

    /// <summary>
    /// Get all entities at a specific board position.
    /// Combines SpatialIndex base state with Overlay pending moves.
    /// </summary>
    public IEnumerable<GameEntity> GetEntitiesAt(IBoardPosition position)
    {
        return _gameState.GetOverlayedEntitiesAt(position);
    }

    public IEnumerable<TGameEntity> GetEntitiesAtOfType<TGameEntity>(IBoardPosition position) where TGameEntity : GameEntity
    {
        foreach (var gameEntity in GetEntitiesAt(position))
        {
            if (gameEntity is TGameEntity typedEntity)
                yield return typedEntity;
        }
    }




    /// <summary>
    /// Get position for an entity. Checks overlay first, then base state.
    /// </summary>
    public IBoardPositionId GetPosition(EntityId id)
    {
        try
        {
            // Get overlayed entity (throws if not found or destroyed)
            var entity = _gameState.GetOverlayedEntity(id);
            // Only Actors have positions now
            if (entity is IActor actor)
            {
                return actor.CurrentPosition;
            }
            return null;
        }
        catch (KeyNotFoundException)
        {
            // Entity destroyed or not found
            return null;
        }
    }

    /// <summary>
    /// Get all entities owned by a specific player.
    /// Uses TeamComponent.OwnerId for ownership lookup.
    /// </summary>
    public IEnumerable<GameEntity> GetEntitiesByOwner(PlayerId owner)
    {
        var entities = _gameState.GetEntitiesByPlayer(owner);
        foreach (var entityId in entities)
        {
            yield return GetEntity(entityId);
        }
    }






    public TurnOrderState TurnOrder => _gameState.TurnOrder;

    public bool IsGameStarted => _gameState.IsGameStarted;

    public bool StillAvailableActions() => _gameState.CurrentPlayerHasAvailableActions();

    public bool IsEndTurn => _gameState.TurnOrder.IsRoundComplete;
}


