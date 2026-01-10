using TurnForge.Engine.Core.Action.CoreActions;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Core.Fsm.Nodes;

/// <summary>
/// Generic EndTurn node for turn-based games.
/// 
/// This node is executed after a player completes their turn.
/// It automatically advances to the next player (via OnEntry action)
/// and then decides whether to:
/// - Continue to TurnNode (next player's turn) if round not complete
/// - Transition to EndRoundNode if all players have played
/// 
/// Flow:
/// TurnNode → EndTurnNode (advance player) → [TurnNode | EndRoundNode]
/// </summary>
public class EndTurnNode : BaseFsmNode
{
    private BaseFsmNode? _turnNode;
    private BaseFsmNode? _endRoundNode;

    public EndTurnNode() : base("EndTurn")
    {
        // OnEntry: Advance to next player
        OnEntry(MoveToNextPlayerEndTurnAction.Create());
    }

    /// <summary>
    /// Configure the turn node to return to for next player.
    /// </summary>
    public EndTurnNode WithTurnNode(BaseFsmNode turn)
    {
        _turnNode = turn;
        return this;
    }

    /// <summary>
    /// Configure the end round node to transition to when round complete.
    /// </summary>
    public EndTurnNode WithEndRound(BaseFsmNode endRound)
    {
        _endRoundNode = endRound;
        return this;
    }

    public override bool IsCompleted(GameStateView state)
    {
        // Always complete immediately (after OnEntry workflows)
        return true;
    }

    public override BaseFsmNode? GetNextNode(GameStateView state)
    {
        // Check if all players have played this round
        if (state.TurnOrder.IsRoundComplete)
        {
            return _endRoundNode;  // All players done, go to EndRound
        }

        // Not all players done, continue to next player's turn
        return _turnNode;
    }

    protected BaseFsmNode? TurnNode => _turnNode;
    protected BaseFsmNode? EndRoundNode => _endRoundNode;
}
