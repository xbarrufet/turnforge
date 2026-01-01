using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Actions;

/// <summary>
/// System workflow that advances the turn order to the next player.
/// Should be added as OnEntry to StartRoundNode.
/// 
/// Logic:
/// - If this is the first entry of a new round (index = 0), don't increment
/// - Otherwise, increment to next player
/// </summary>
public static class AdvanceTurnActionFactory
{
    public static IAction Create()
    {
        return ActionBuilder.Create("AdvanceTurn")
            .AddNode(new AdvanceTurnNode())
            .Build();
    }
}

public class AdvanceTurnNode : LinkableNode
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
        
        // Don't increment on first entry of round (index is already at 0)
        if (currentTurn.CurrentPlayerIndex == 0)
        {
            return ActionStepResult.Success();
        }
        
        // Advance to next player
        var nextTurn = currentTurn.NextPlayer();
        
        // Record the operation
        context.Overlay.Record(new SetTurnOrderOperation(nextTurn));
        
        return ActionStepResult.Success();
    }
}
