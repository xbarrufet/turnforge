using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities.Overlay;

namespace TurnForge.Engine.Entities.Overlay;

/// <summary>
/// Operation to set or replace the TurnOrder at runtime.
/// Essential for initialization workflows (GameStart).
/// </summary>
public sealed class SetTurnOrderOperation : IGameStateOperation
{
    public TurnOrderState NewOrder { get; }
    public EntityId Target => EntityId.Empty; 

    public SetTurnOrderOperation(TurnOrderState newOrder)
    {
        NewOrder = newOrder;
    }

    public void Apply(IGameStateMutator mutator)
    {
        mutator.SetTurnOrder(NewOrder);
    }
}
