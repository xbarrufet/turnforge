using ParchisLudo.Rules.Board;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace ParchisLudo.Rules.Fsm.Nodes;

/// <summary>
/// Parchís-specific EndRound node.
/// Extends generic EndRoundNode with victory checking.
/// 
/// Controls round completion:
/// - Checks if IsRoundComplete
/// - If not complete → StartRound (next player)
/// - If complete and winner → EndGame
/// - If complete and no winner → StartRound (new round)
/// </summary>
public class ParchisEndRoundNode : NextPlayerEndRoundNode
{

    /// <summary>
    /// Fluent builder for chaining.
    /// </summary>
    public new ParchisEndRoundNode WithStartRound(BaseFsmNode startRound)
    {
        base.WithStartRound(startRound);
        return this;
    }

    public new ParchisEndRoundNode WithEndGame(BaseFsmNode endGame)
    {
        base.WithEndGame(endGame);
        return this;
    }

    /// <summary>
    /// Check if any player has won.
    /// </summary>
    protected override bool CheckGameOver(GameStateView state)
    {
        return CheckWinner(state) != null;
    }

    private PlayerId? CheckWinner(GameStateView state)
    {
        // Iterate through all 4 colors
        var colors = new[] {
            ParchisBoard.PlayerColor.Red,
            ParchisBoard.PlayerColor.Blue,
            ParchisBoard.PlayerColor.Green,
            ParchisBoard.PlayerColor.Yellow
        };

        foreach (var color in colors)
        {
            var colorName = color.ToString().ToLower();

            // Find all pawns for this color using GameStateView
            var playerPawns = new List<GameEntity>();

            // Get all entities for this player (assuming PlayerId matches color)
            var playerId = PlayerId.From(colorName);
            foreach (var entity in state.GetEntitiesByOwner(playerId))
            {
                if (entity.DefinitionId.StartsWith($"pawn_{colorName}"))
                {
                    playerPawns.Add(entity);
                }
            }

            // In a standard game there are 4 pawns. If less, maybe game just started or custom rules?
            // Victory implies ALL pawns are at center.
            if (playerPawns.Count == 0) continue;

            var allAtCenter = playerPawns.All(p => IsAtCenter(state, p));

            if (allAtCenter)
            {
                return playerId;
            }
        }

        return null;
    }

    private bool IsAtCenter(GameStateView state, GameEntity pawn)
    {
        // Check position via GameStateView
        var pos = state.GetPosition(pawn.Id);

        if (pos is TilePosition tp && tp.TileId.Value == "center")
            return true;

        return false;
    }
}
