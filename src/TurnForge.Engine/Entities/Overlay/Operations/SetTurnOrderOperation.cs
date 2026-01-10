using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

/// <summary>
/// Operation to update the TurnOrderState in GameState.
/// </summary>
public record struct SetTurnOrderOperation(TurnOrderState NewTurnOrder) : IGameStateOperation
{
    // System operation doesn't target a specific entity
    public EntityId EntityId => EntityId.Empty;
    
}
