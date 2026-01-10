using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Nodes;

/// <summary>
/// Generic StartRound node for turn-based games.
/// 
/// Prepares the next player's turn:
/// - OnEntry: advance to next player (via workflow)
/// - Always transitions to TurnNode
/// 
/// Subclasses can override for game-specific behavior.
/// </summary>
public class StartRoundNode : BaseFsmNode
{
    private BaseFsmNode? _turnNode;
    
    public StartRoundNode() : base("StartRound") { }
    
    /// <summary>
    /// Configure the turn node to transition to.
    /// </summary>
    public StartRoundNode WithTurnNode(BaseFsmNode turnNode)
    {
        _turnNode = turnNode;
        return this;
    }
    
    public override bool IsCompleted(GameStateView state)
    {
        // Always complete immediately (after OnEntry workflows)
        return true;
    }
    
    public override BaseFsmNode? GetNextNode(GameStateView state)
    {
        // Always go to turn node
        return _turnNode;
    }
    
    /// <summary>
    /// Get current player from state.
    /// </summary>
    public PlayerId GetCurrentPlayer(GameStateView state) => state.TurnOrder.CurrentPlayer;
    
    protected BaseFsmNode? TurnNode => _turnNode;
}
