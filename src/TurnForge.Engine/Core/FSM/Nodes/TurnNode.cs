using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Nodes;

/// <summary>
/// Generic TurnNode for turn-based games.
/// 
/// Executes a single player's turn using TurnOrderState.
/// Always transitions to EndRoundNode after completing.
/// 
/// Subclasses can override for game-specific behavior (AP, actions, etc).
/// </summary>
public class TurnNode : BaseFsmNode
{
    private BaseFsmNode? _endRoundNode;
    
    public TurnNode() : base("Turn") { }
    
    /// <summary>
    /// Configure the end round node to transition to.
    /// </summary>
    public TurnNode WithEndRound(BaseFsmNode endRound)
    {
        _endRoundNode = endRound;
        return this;
    }
    
    /// <summary>
    /// Override in subclass to define when turn is complete.
    /// Default: always complete immediately.
    /// </summary>
    public override bool IsCompleted(GameStateView state)
    {
        return true;
    }
    
    /// <summary>
    /// Always transition to EndRound.
    /// EndRound will decide if round is complete or continue.
    /// </summary>
    public override BaseFsmNode? GetNextNode(GameStateView state)
    {
        return _endRoundNode;
    }
    
    /// <summary>
    /// Get current player from state.
    /// </summary>
    public PlayerId GetCurrentPlayer(GameStateView state) => state.TurnOrder.CurrentPlayer;
    
    protected BaseFsmNode? EndRoundNode => _endRoundNode;
}
