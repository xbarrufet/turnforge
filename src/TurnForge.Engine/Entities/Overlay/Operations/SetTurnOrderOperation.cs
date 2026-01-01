using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

/// <summary>
/// Operation to update the TurnOrderState in GameState.
/// </summary>
public sealed class SetTurnOrderOperation : IGameStateOperation
{
    // System operation doesn't target a specific entity
    public EntityId Target { get; } = EntityId.Empty;
    
    public TurnOrderState NewTurnOrder { get; }
    
    public SetTurnOrderOperation(TurnOrderState newTurnOrder)
    {
        NewTurnOrder = newTurnOrder;
    }
    
    public void Apply(IGameStateMutator mutator)
    {
        mutator.SetTurnOrder(NewTurnOrder);
    }
}
