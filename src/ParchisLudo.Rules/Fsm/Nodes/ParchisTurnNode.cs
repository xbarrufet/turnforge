using ParchisLudo.Rules.Actions;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace ParchisLudo.Rules.Fsm.Nodes;

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
    // TODO: Restore AP tracking when needed (move to GameState for proper state management)

    /// <summary>
    /// Fluent builder for chaining.
    /// </summary>
    public new ParchisTurnNode WithEndRound(BaseFsmNode endRound)
    {
        base.WithEndRound(endRound);
        return this;
    }


    /// <summary>
    /// Turn completes when current player's AP = 0.
    /// Reads from GameState (set by SpendAPOperation).
    /// </summary>
    public override bool IsCompleted(GameStateView state)
    {
        return !state.StillAvailableActions();
    }

    public override IReadOnlyList<ActionId> GetAllowedActions()
    {
        // TODO: Fix - ParchisMoveAction does not have ActionId method
        return Array.Empty<ActionId>();
    }
}
