using Parchis.Rules.Board;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.ValueObjects;

namespace Parchis.Rules;

/// <summary>
/// Bootstrap for initializing a Parchís game.
/// Creates the initial game state with all pieces at home.
/// </summary>
public static class ParchisBootstrap
{
    public const int PiecesPerPlayer = 4;
    
    /// <summary>
    /// Create initial game state for a 2-player Parchís game.
    /// </summary>
    public static GameState CreateInitialState()
    {
        var state = GameState.Empty();
        
        // Create pieces for Yellow player
        for (int i = 0; i < PiecesPerPlayer; i++)
        {
            var piece = CreatePiece(PlayerColor.Yellow, i);
            state = state.WithAgent(piece);
        }
        
        // Create pieces for Blue player
        for (int i = 0; i < PiecesPerPlayer; i++)
        {
            var piece = CreatePiece(PlayerColor.Blue, i);
            state = state.WithAgent(piece);
        }
        
        // Set initial metadata
        state = state
            .WithMetadata("CurrentPlayer", PlayerColor.Yellow)
            .WithMetadata("TurnNumber", 1)
            .WithMetadata("ConsecutiveSixes", 0);
        
        return state;
    }
    
    /// <summary>
    /// Create a piece agent for a player.
    /// </summary>
    private static TurnForge.Engine.Definitions.Actors.Agent CreatePiece(PlayerColor color, int pieceNumber)
    {
        var id = new EntityId($"{color}_{pieceNumber}");
        
        // Using basic Agent structure - will add components later
        return new TurnForge.Engine.Definitions.Actors.Agent(
            id,
            $"{color} Piece {pieceNumber}",
            Array.Empty<TurnForge.Engine.Components.Interfaces.IGameEntityComponent>(),
            Array.Empty<TurnForge.Engine.Traits.Interfaces.IBaseTrait>()
        );
    }
}
