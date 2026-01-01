using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Actions;

/// <summary>
/// System workflow that resets turn order for a new round.
/// Should be added as OnEntry to EndRoundNode (before returning to StartRound).
/// 
/// Resets CurrentPlayerIndex to 0 and increments RoundNumber.
/// </summary>
public static class StartNewRoundActionFactory
{
    public static IAction Create()
    {
        return ActionBuilder.Create("StartNewRound")
            .AddNode(new StartNewRoundNode())
            .Build();
    }
}

public class StartNewRoundNode : LinkableNode
{
    public override NodeId Id => new NodeId(Guid.NewGuid().ToString());
    
    public override ActionStepResult Execute(ActionContext context)
    {
        var state = context.State;
        var currentTurn = state.TurnOrder;
        
        // Skip if no turn order configured
        if (currentTurn.PlayerOrder.Count == 0)
        {
            return ActionStepResult.Success();
        }
        
        // Reset to first player and increment round
        var newRound = currentTurn.NextRound();
        
        // Record the operation
        context.Overlay.Record(new SetTurnOrderOperation(newRound));
        
        return ActionStepResult.Success();
    }
}
