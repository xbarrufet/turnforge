using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.CoreActions;

/// <summary>
/// move to next player's or set endround
public class MoveToNextPlayerEndTurnAction
{

    public const string ActionId = "Core.EndTurn";
    public static IAction Create()
    {
        var nextPlayerNode = new SetNextPlayerInTurn();

        return ActionBuilder.Create(ActionId)
            .WithContext(() => new SystemActionContext())
            .AddNode(nextPlayerNode)
            .Build();
    }
}

public class SetNextPlayerInTurn : LinkableNode
{
    public override NodeId Id => new("Set_Next_Player_In_Turn");
    public override ActionStepResult Execute(ActionContext context, GameStateView state)
    {
        state.RecordOperation(new SetTurnOrderOperation(state.TurnOrder.NextPlayer()));
        return ActionStepResult.Success();
    }
}
