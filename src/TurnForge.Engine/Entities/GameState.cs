using System.Collections.Immutable;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Items; // For Item
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities.Definitions; // For MissionDefinition

namespace TurnForge.Engine.Entities;

public sealed class GameState
{
    public ImmutableDictionary<EntityId, GameEntity> Entities { get; }
    public ImmutableDictionary<PlayerId, Player> Players { get; }
    public NodeId? CurrentStateId { get; }
    public IGameBoard? Board { get; }
    public MissionDefinition? Mission { get; }
    public TurnOrderState TurnOrder { get; }

    public GameState(
        ImmutableDictionary<EntityId, GameEntity> entities,
        ImmutableDictionary<PlayerId, Player> players,
        NodeId? currentFsmNode,
        IGameBoard? board,
        MissionDefinition? mission = null,
        TurnOrderState? turnOrder = null)
    {
        Entities = entities;
        Players = players;
        CurrentStateId = currentFsmNode;
        Board = board;
        Mission = mission;
        TurnOrder = turnOrder ?? TurnOrderState.Empty;
    }

    public static GameState Empty()
    {
        return new GameState(
            ImmutableDictionary<EntityId, GameEntity>.Empty,
            ImmutableDictionary<PlayerId, Player>.Empty,
            null,
            null,
            null,
            TurnOrderState.Empty); 
    }

    public Player? GetPlayerByPlayerId(PlayerId playerId)
    {
        return Players.TryGetValue(playerId, out var player) ? player : null;
    }
}





