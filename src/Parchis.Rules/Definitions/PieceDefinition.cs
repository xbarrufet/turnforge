using TurnForge.Engine.Definitions;
using TurnForge.Engine.Traits.Interfaces;
using Parchis.Rules.Board;

namespace Parchis.Rules.Definitions;

/// <summary>
/// Entity definition for a Parchís piece.
/// Each player has 4 pieces.
/// </summary>
public class PieceDefinition : BaseGameEntityDefinition
{
    public PlayerColor Color { get; }
    public int PieceNumber { get; }
    
    public PieceDefinition(PlayerColor color, int pieceNumber) 
        : base($"Parchis.Piece.{color}.{pieceNumber}", "Piece")
    {
        Color = color;
        PieceNumber = pieceNumber;
        
        // Add traits for piece state tracking
        // Note: Position is in component, not trait
    }
    
    /// <summary>
    /// Create definitions for all pieces of both players.
    /// </summary>
    public static IEnumerable<PieceDefinition> CreateAllPieceDefinitions()
    {
        foreach (PlayerColor color in Enum.GetValues<PlayerColor>())
        {
            for (int i = 0; i < 4; i++)
            {
                yield return new PieceDefinition(color, i);
            }
        }
    }
}
