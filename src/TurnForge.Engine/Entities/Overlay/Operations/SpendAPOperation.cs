using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

/// <summary>
/// Operation to spend action points from a player.
/// Used by actions (like Move) to consume AP via the overlay.
/// </summary>
public sealed class SpendAPOperation : IGameStateOperation
{
    public PlayerId PlayerId { get; }
    public int Amount { get; }
    public bool IsBonusTurn { get; }
    
    /// <summary>
    /// Target is not applicable for player operations.
    /// </summary>
    public EntityId Target => EntityId.Empty;
    
    public SpendAPOperation(PlayerId playerId, int amount = 1, bool isBonusTurn = false)
    {
        PlayerId = playerId;
        Amount = amount;
        IsBonusTurn = isBonusTurn;
    }
    
    public void Apply(IGameStateMutator mutator)
    {
        // Applied by overlay during commit
    }
}
