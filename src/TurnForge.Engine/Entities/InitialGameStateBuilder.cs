using System.Collections.Immutable;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Players;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

/// <summary>
/// Builder for creating initial GameState.
/// Used only during game initialization (StartGame action).
/// For runtime entity creation, use ISpawnService with GameStateOverlay.
/// </summary>
public class InitialGameStateBuilder
{
    private readonly Dictionary<EntityId, GameEntity> _entities = new();
    private readonly Dictionary<PlayerId, Player> _players = new();
    private IGameBoard? _board;
    private TurnOrderState? _turnOrder;

    /// <summary>
    /// Adds players to the initial game state.
    /// </summary>
    public InitialGameStateBuilder WithPlayers(IEnumerable<Player> players)
    {
        foreach (var player in players)
        {
            _players[player.PlayerId] = player;
        }
        return this;
    }

    /// <summary>
    /// Adds a single player to the initial game state.
    /// </summary>
    public InitialGameStateBuilder WithPlayer(Player player)
    {
        _players[player.PlayerId] = player;
        return this;
    }

    /// <summary>
    /// Adds starting agents with their positions.
    /// </summary>
    public InitialGameStateBuilder WithStartingAgents(
        IEnumerable<(GameEntity agent, IBoardPosition position)> agentsWithPositions)
    {
        foreach (var (agent, position) in agentsWithPositions)
        {
            _entities[agent.Id] = agent;
        }
        return this;
    }

    /// <summary>
    /// Adds a single starting agent with its position.
    /// </summary>
    public InitialGameStateBuilder WithStartingAgent(GameEntity agent, IBoardPosition position)
    {
        _entities[agent.Id] = agent;
        return this;
    }

    /// <summary>
    /// Sets the game board (required).
    /// </summary>
    public InitialGameStateBuilder WithGameBoard(IGameBoard board)
    {
        _board = board;
        return this;
    }

    /// <summary>
    /// Adds starting zones with their positions.
    /// </summary>
    public InitialGameStateBuilder WithStartingZones(
        IEnumerable<(GameEntity zone, IBoardPosition position)> zonesWithPositions)
    {
        foreach (var (zone, position) in zonesWithPositions)
        {
            _entities[zone.Id] = zone;
        }
        return this;
    }

    /// <summary>
    /// Adds a single starting zone with its position.
    /// </summary>
    public InitialGameStateBuilder WithStartingZone(GameEntity zone, IBoardPosition position)
    {
        _entities[zone.Id] = zone;
        return this;
    }

    /// <summary>
    /// Adds connections with their positions.
    /// </summary>
    public InitialGameStateBuilder WithConnections(
        IEnumerable<(GameEntity connection, IBoardPosition position)> connectionsWithPositions)
    {
        foreach (var (connection, position) in connectionsWithPositions)
        {
            _entities[connection.Id] = connection;
        }
        return this;
    }

    /// <summary>
    /// Adds a single connection with its position.
    /// </summary>
    public InitialGameStateBuilder WithConnection(GameEntity connection, IBoardPosition position)
    {
        _entities[connection.Id] = connection;
        return this;
    }

    /// <summary>
    /// Adds starting props with their positions.
    /// </summary>
    public InitialGameStateBuilder WithStartingProps(
        IEnumerable<(GameEntity prop, IBoardPosition position)> propsWithPositions)
    {
        foreach (var (prop, position) in propsWithPositions)
        {
            _entities[prop.Id] = prop;
        }
        return this;
    }

    /// <summary>
    /// Adds a single starting prop with its position.
    /// </summary>
    public InitialGameStateBuilder WithStartingProp(GameEntity prop, IBoardPosition position)
    {
        _entities[prop.Id] = prop;
        return this;
    }

  

    /// <summary>
    /// Sets the turn order (optional, will be created from players if not provided).
    /// </summary>
    public InitialGameStateBuilder WithTurnOrder(TurnOrderState turnOrder)
    {
        _turnOrder = turnOrder;
        return this;
    }

    /// <summary>
    /// Builds the initial GameState.
    /// Validates that required components (Players, Board) are present.
    /// </summary>
    /// <exception cref="InvalidOperationException">If required components are missing.</exception>
    public GameState Build()
    {
        // Validate required components
        if (_players.Count == 0)
        {
            throw new InvalidOperationException("Cannot build GameState without players. Use WithPlayers() or WithPlayer().");
        }

        if (_board == null)
        {
            throw new InvalidOperationException("Cannot build GameState without a board. Use WithGameBoard().");
        }

        // Create turn order from players if not provided
        var turnOrder = _turnOrder ?? TurnOrderState.Create(_players.Keys);

        return new GameState(
            _entities.ToImmutableDictionary(),
            _players.ToImmutableDictionary(),
            NodeId.Empty,
            _board,
            _turnOrder);
    }
}
