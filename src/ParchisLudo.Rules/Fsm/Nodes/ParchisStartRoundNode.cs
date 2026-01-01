using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace Parchis.Rules.Fsm.Nodes;

/// <summary>
/// Parchís-specific StartRound node.
/// Extends generic StartRoundNode.
/// 
/// OnEntry workflows can advance turn order, reset AP, etc.
/// </summary>
public class ParchisStartRoundNode : StartRoundNode
{
    public ParchisStartRoundNode() : base() { }
    
    /// <summary>
    /// Fluent builder for chaining.
    /// </summary>
    public new ParchisStartRoundNode WithTurnNode(BaseFsmNode turnNode)
    {
        base.WithTurnNode(turnNode);
        return this;
    }
}
