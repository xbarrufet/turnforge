using System.Collections.Generic;
using System.Collections.Immutable;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Players;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay;

public sealed class GameStateBuilder : IGameStateMutator
{
    private readonly Dictionary<EntityId, GameEntity> _entities;
    private IGameBoard? _board;
    private readonly Dictionary<PlayerId, Player> _players;
    private NodeId? _currentStateId;
    private TurnOrderState _turnOrder;
    private bool _newPlayers = false;

    public GameStateBuilder(GameState baseState)
    {
        _entities = new Dictionary<EntityId, GameEntity>(baseState.Entities);
        _players = new Dictionary<PlayerId, Player>(baseState.Players);
        _currentStateId = baseState.CurrentStateId;
        _turnOrder = baseState.TurnOrder;
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
    

    public void SetBoard(IGameBoard board)
    {
        _board = board;
    }

    public void AddEntity(GameEntity entity)
    {
        _entities[entity.Id] = entity;
    }

    public void RemoveEntity(EntityId id)
    {
        _entities.Remove(id);
    }


    public void AddOrUpdatePlayer(Player player)
    {
        if (!_players.ContainsKey(player.PlayerId))
            _newPlayers = true;
        _players[player.PlayerId] = player;
    }

    public GameState Build()
    {
        // si hi ha nous _players, recreem el turn si no mantenim l'actual
        if (_newPlayers)
        {
            _turnOrder = TurnOrderState.Create(_players.Keys);
        }
        return new GameState(
            _entities.ToImmutableDictionary(),
            _players.ToImmutableDictionary(),
            _currentStateId ?? NodeId.Empty,
            _board,
            _turnOrder);
    }
}
