using TurnForge.Engine.Core.Action.CoreActions;

namespace TurnForge.Engine.Core.Fsm.Nodes;

public class ChekEndTurnAndResetApStartRoundNode:StartRoundNode
{
    public ChekEndTurnAndResetApStartRoundNode() : base()
    {
        // Automatically advance to next player when entering this node
        OnEntry(NextTurnResetAction.Create());
    }

    /// <summary>
    /// Fluent builder for chaining (returns specific type).
    /// </summary>
    public new ChekEndTurnAndResetApStartRoundNode WithTurnNode(BaseFsmNode turnNode)
    {
        base.WithTurnNode(turnNode);
        return this;
    }
    
}