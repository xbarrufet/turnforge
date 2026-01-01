using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Definitions; // For GameEntity
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components.Interfaces; 
using Parchis.Rules.Board;

namespace Parchis.Rules.Fsm.Nodes;

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
public class ParchisEndRoundNode : EndRoundNode
{
    public ParchisEndRoundNode() : base() { }
    
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
    protected override bool CheckGameOver(GameState state)
    {
        return CheckWinner(state) != null;
    }
    
    private PlayerId? CheckWinner(GameState state)
    {
        if (state.Board == null) return null;

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

            // Find all pawns for this color based on DefinitionId convention "pawn_{color}_{n}"
            var playerPawns = state.Entities.Values
                .Where(e => e.DefinitionId.StartsWith($"pawn_{colorName}"))
                .ToList();
            
            // In a standard game there are 4 pawns. If less, maybe game just started or custom rules?
            // Victory implies ALL pawns are at center.
            if (playerPawns.Count == 0) continue;
            
            var allAtCenter = playerPawns.All(p => IsAtCenter(state, p));
            
            if (allAtCenter)
            {
                // Return player ID. Assuming PlayerId matches color name for simplicity in this implementation
                // OR finding the player with that ID.
                return PlayerId.From(colorName);
            }
        }
        
        return null;
    }

    private bool IsAtCenter(GameState state, GameEntity pawn)
    {
        // Check position via SpatialIndex (authoritative)
        var pos = state.Board!.SpatialIndex.GetEntityPosition(pawn.Id);
        
        // Also check component as fallback if spatial index hasn't updated yet (though it should have)
        if (pos == null || pos.Equals(TilePosition.Empty))
        {
            pos = pawn.GetComponent<IPositionComponent>()?.CurrentPosition; // Simplified interface usage
        }

        if (pos is TilePosition tp && tp.TileId.Value == "center") 
            return true;
            
        return false;
    }
}
