using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions; // For MissionDefinition.Board.Interfaces;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay;

public sealed class GameStateBuilder : IGameStateMutator
{
    private readonly Dictionary<EntityId, GameEntity> _entities;
    private readonly HashSet<EntityId> _modifiedEntities = new();
    private IGameBoard? _board;
    private readonly Dictionary<PlayerId, Player> _players;
    private NodeId? _currentStateId;
    private MissionDefinition? _mission;
    private TurnOrderState _turnOrder;

    public GameStateBuilder(GameState baseState)
    {
        _entities = new Dictionary<EntityId, GameEntity>(baseState.Entities);
        _players = new Dictionary<PlayerId, Player>(baseState.Players);
        _currentStateId = baseState.CurrentStateId;
        _mission = baseState.Mission;
        _turnOrder = baseState.TurnOrder;

        // Clone board if exists to allow mutation without affecting base state
        // We assume the board implementation supports cloning via IGameBoard.
        _board = baseState.Board?.Clone();
    }

    public void SetTurnOrder(TurnOrderState turnOrder)
    {
        _turnOrder = turnOrder;
    }

    /// <summary>
    /// Fluent setter for FSM node transition.
    /// </summary>
    public GameStateBuilder WithCurrentFsmNodeId(NodeId nodeId)
    {
        _currentStateId = nodeId;
        return this;
    }

    public void SetMission(MissionDefinition mission)
    {
        _mission = mission;
    }



    public void SetBoard(IGameBoard board)
    {
        _board = board;
    }

    private GameEntity GetMutableEntity(EntityId id)
    {
        if (!_entities.TryGetValue(id, out var entity))
            throw new InvalidOperationException($"Entity {id} not found");

        if (_modifiedEntities.Contains(id))
            return entity;

        var clone = entity.Clone();
        _entities[id] = clone;
        _modifiedEntities.Add(id);
        return clone;
    }

    public void AddEntity(GameEntity entity)
    {
        _entities[entity.Id] = entity;
        _modifiedEntities.Add(entity.Id);
    }

    public void RemoveEntity(EntityId id)
    {
        _entities.Remove(id);
        _modifiedEntities.Remove(id);
    }

    public void SetComponent<T>(EntityId id, T component) where T : notnull
    {
        var entity = GetMutableEntity(id);
        if (component is IGameEntityComponent c)
        {
            entity.ReplaceComponent(c);
        }
        else
        {
            throw new ArgumentException($"Component {typeof(T).Name} must implement IGameEntityComponent");
        }
    }

    public void RemoveComponent<T>(EntityId id) where T : notnull
    {
        var entity = GetMutableEntity(id);
        if (typeof(IGameEntityComponent).IsAssignableFrom(typeof(T)))
        {
             var method = typeof(GameEntity).GetMethod(nameof(GameEntity.RemoveComponent))?.MakeGenericMethod(typeof(T));
             method?.Invoke(entity, null);
        }
    }

    public void SetPosition(EntityId entityId, IBoardPosition position)
    {
        if (_board == null) throw new InvalidOperationException("Cannot set position: No board in game state");
        _board.SpatialIndex.Register(entityId, position);
    }

    public void ClearPosition(EntityId entityId)
    {
        _board?.SpatialIndex.Unregister(entityId);
    }

    public void MarkDestroyed(EntityId entityId)
    {
        RemoveEntity(entityId);
    }

    /// <summary>
    /// Modify a player's action points.
    /// </summary>
    public void SpendPlayerAP(PlayerId playerId, int amount, bool isBonusTurn)
    {
        if (isBonusTurn) return; // Bonus turns don't consume AP
        
        if (!_players.TryGetValue(playerId, out var player))
            throw new InvalidOperationException($"Player {playerId} not found");
        
        // Clone player if not already modified
        var modifiedPlayer = player.Clone();
        modifiedPlayer.ActionPoints = Math.Max(0, modifiedPlayer.ActionPoints - amount);
        _players[playerId] = modifiedPlayer;
    }

    public void AddPlayer(Player player)
    {
        if (_players.ContainsKey(player.PlayerId))
            throw new InvalidOperationException($"Player {player.PlayerId} already exists.");
            
        _players[player.PlayerId] = player;
    }

    public GameState Build()
        => new GameState(_entities.ToImmutableDictionary(), _players.ToImmutableDictionary(), _currentStateId, _board, _mission, _turnOrder);
}
