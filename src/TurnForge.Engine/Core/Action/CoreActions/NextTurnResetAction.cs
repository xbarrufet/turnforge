using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.CoreActions;

public class NextTurnResetAction
{
    public const string ActionId = "Core.NextTurnResetAction";
    public static IAction Create()
    {
        var nextTurnResetActionNode = new NextTurnResetActionNode();

        return ActionBuilder.Create(ActionId)
            .WithContext(() => new SystemActionContext())
            .AddNode(nextTurnResetActionNode)
            .Build();
    }
}

public class NextTurnResetActionNode : LinkableNode
{
    public override NodeId Id => new("Reset_Action_Points_Node");
    public override ActionStepResult Execute(ActionContext context, GameStateView state)
    {
        if(state.IsEndTurn) 
            state.RecordOperation(new NextTurnResetApOperation());
        return ActionStepResult.Success();
    }
}
