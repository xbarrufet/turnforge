using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.CoreActions;

public class NextTurnResetActions
{
    public const string ActionId = "Core.ResetActionPoints";
    public static IAction Create()
    {
        var resetActionPointsNode = new ResetActionPointsNode();

        return ActionBuilder.Create(ActionId)
            .WithContext(() => new SystemActionContext())
            .AddNode(resetActionPointsNode)
            .Build();
    }
}

public class ResetActionPointsNode : LinkableNode
{
    public override NodeId Id => new("Reset_Action_Points_Node");
    public override ActionStepResult Execute(ActionContext context, GameStateView state)
    {
        state.RecordOperation(new SetTurnOrderOperation(state.TurnOrder.NextPlayer()));
        return ActionStepResult.Success();
    }
}
