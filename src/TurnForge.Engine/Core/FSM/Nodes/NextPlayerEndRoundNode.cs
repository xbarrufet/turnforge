using TurnForge.Engine.Core.Action.CoreActions;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Core.Fsm.Nodes;

/// <summary>
/// EndRound node that automatically advances to the next player.
/// 
/// This node extends EndRoundNode and adds an OnEntry action to advance
/// the turn order to the next player before transitioning.
/// 
/// Use this when you want automatic turn advancement without manually
/// configuring OnEntry workflows.
/// </summary>
public class NextPlayerEndRoundNode : EndRoundNode
{
    public NextPlayerEndRoundNode() : base()
    {
        // Automatically advance to next player when entering this node
        OnEntry(MoveToNextPlayerEndTurnAction.Create());
    }

    /// <summary>
    /// Fluent builder for chaining (returns specific type).
    /// </summary>
    public new NextPlayerEndRoundNode WithStartRound(BaseFsmNode startRound)
    {
        base.WithStartRound(startRound);
        return this;
    }

    public new NextPlayerEndRoundNode WithEndGame(BaseFsmNode endGame)
    {
        base.WithEndGame(endGame);
        return this;
    }
}