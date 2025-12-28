using Parchis.Rules.Board;
using Parchis.Rules.Definitions;
using TurnForge.Engine.Definitions;

namespace Parchis.Rules;

/// <summary>
/// Bootstrap for initializing a Parchís game.
/// Uses TurnForge patterns: Board via factory.
/// </summary>
public static class ParchisBootstrap
{
    public const int PiecesPerPlayer = 4;
    
    /// <summary>
    /// Create initial game state with board.
    /// Pieces are managed separately via game API.
    /// </summary>
    public static GameState CreateInitialStateWithBoard()
    {
        var board = ParchisBoardFactory.Create();
        
        return GameState.Empty()
            .WithBoard(board)
            .WithMetadata("CurrentPlayer", PlayerColor.Yellow)
            .WithMetadata("TurnNumber", 1)
            .WithMetadata("ConsecutiveSixes", 0);
    }
    
    /// <summary>
    /// Get all piece definitions.
    /// </summary>
    public static IEnumerable<PieceDefinition> GetPieceDefinitions()
    {
        return PieceDefinition.CreateAllPieceDefinitions();
    }
}
