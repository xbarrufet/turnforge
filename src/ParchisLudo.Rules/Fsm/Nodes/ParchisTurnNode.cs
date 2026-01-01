using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace Parchis.Rules.Fsm.Nodes;

/// <summary>
/// Parchís-specific TurnNode.
/// Extends generic TurnNode with AP management and 6-bonus logic.
/// 
/// Each player gets 1 AP per turn.
/// AP is consumed by: dice roll + move.
/// If player rolls 6, AP is NOT consumed (bonus action).
/// 
/// Turn completes when AP = 0.
/// </summary>
public class ParchisTurnNode : TurnNode
{
    // Track remaining AP (starts at 1)
    // TODO: Move AP tracking to GameState for proper state management
    private int _actionPoints = 1;
    private bool _lastRollWasSix = false;
    
    public ParchisTurnNode() : base() { }
    
    /// <summary>
    /// Fluent builder for chaining.
    /// </summary>
    public new ParchisTurnNode WithEndRound(BaseFsmNode endRound)
    {
        base.WithEndRound(endRound);
        return this;
    }
    
    /// <summary>
    /// Called when a move is executed.
    /// </summary>
    public void ConsumeAction(bool rolledSix)
    {
        _lastRollWasSix = rolledSix;
        
        if (!rolledSix)
        {
            _actionPoints--;
        }
        // If rolled 6, AP stays the same (bonus action)
    }
    
    /// <summary>
    /// Reset AP for new turn.
    /// </summary>
    public void ResetForNewTurn()
    {
        _actionPoints = 1;
        _lastRollWasSix = false;
    }
    
    /// <summary>
    /// Turn completes when current player's AP = 0.
    /// Reads from GameState (set by SpendAPOperation).
    /// </summary>
    public override bool IsCompleted(GameState state)
    {
        var currentPlayerId = state.TurnOrder.CurrentPlayer;
        var player = state.GetPlayerByPlayerId(currentPlayerId);
        if (player == null) return true; // No player = turn complete
        
        return player.ActionPoints <= 0;
    }
    
    /// <summary>
    /// Transition to EndRound, reset AP for next time.
    /// </summary>
    public override BaseFsmNode? GetNextNode(GameState state)
    {
        // Note: AP reset now handled via operations when new turn starts
        return base.GetNextNode(state);
    }
    
    public int RemainingAP => _actionPoints;
    public bool LastRollWasSix => _lastRollWasSix;
}
