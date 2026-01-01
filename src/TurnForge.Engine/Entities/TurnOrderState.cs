using System.Collections.Immutable;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

/// <summary>
/// Tracks turn order and current player for turn-based games.
/// Immutable state that can be stored in GameState.
/// </summary>
public record TurnOrderState(
    ImmutableList<PlayerId> PlayerOrder,
    int CurrentPlayerIndex,
    int RoundNumber
)
{
    /// <summary>
    /// Get the current player's ID.
    /// </summary>
    public PlayerId CurrentPlayer => 
        CurrentPlayerIndex < PlayerOrder.Count 
            ? PlayerOrder[CurrentPlayerIndex] 
            : PlayerId.From("none");
    
    /// <summary>
    /// True if all players have completed their turn this round.
    /// </summary>
    public bool IsRoundComplete => CurrentPlayerIndex >= PlayerOrder.Count;
    
    /// <summary>
    /// Advance to next player.
    /// </summary>
    public TurnOrderState NextPlayer() => 
        this with { CurrentPlayerIndex = CurrentPlayerIndex + 1 };
    
    /// <summary>
    /// Start a new round (reset to first player, increment round).
    /// </summary>
    public TurnOrderState NextRound() => 
        this with { CurrentPlayerIndex = 0, RoundNumber = RoundNumber + 1 };
    
    /// <summary>
    /// Create initial turn order from player list.
    /// </summary>
    public static TurnOrderState Create(IEnumerable<PlayerId> players) =>
        new(players.ToImmutableList(), 0, 1);
    
    /// <summary>
    /// Empty turn order (no players).
    /// </summary>
    public static TurnOrderState Empty =>
        new(ImmutableList<PlayerId>.Empty, 0, 0);
}
