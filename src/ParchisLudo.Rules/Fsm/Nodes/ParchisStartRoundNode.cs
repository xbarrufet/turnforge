using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Entities;

namespace ParchisLudo.Rules.Fsm.Nodes;

/// <summary>
/// Parchís-specific StartRound node.
/// Extends generic StartRoundNode.
/// 
/// OnEntry workflows can advance turn order, reset AP, etc.
/// </summary>
public class ParchisStartRoundNode : ChekEndTurnAndResetApStartRoundNode
{

    public override BaseFsmNode? GetNextNode(GameStateView state)
    {
        // Log turn information before transitioning
        var turnOrder = state.TurnOrder;
        var currentPlayer = turnOrder.CurrentPlayer;
        var roundNumber = turnOrder.RoundNumber;

        Console.WriteLine($"StartRound: Round={roundNumber}, Player={currentPlayer}");

        return base.GetNextNode(state);
    }

    /// <summary>
    /// Fluent builder for chaining.
    /// </summary>
    public new ParchisStartRoundNode WithTurnNode(BaseFsmNode turnNode)
    {
        base.WithTurnNode(turnNode);
        return this;
    }
}
