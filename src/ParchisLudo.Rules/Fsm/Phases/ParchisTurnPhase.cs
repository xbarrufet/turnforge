namespace Parchis.Rules.Fsm.Phases;

/// <summary>
/// DEPRECATED: This class represented the old FSM-based turn phase.
/// 
/// The turn logic has been migrated to the Action architecture.
/// See Parchis.Rules.Actions.ParchisTurnActionFactory for the new implementation.
/// 
/// Key nodes:
/// - ParchisRollDiceNode: Handles dice rolling
/// - ParchisSelectMoveNode: Handles pawn selection
/// - ParchisExecuteMoveNode: Applies the move
/// - ParchisCheckVictoryNode: Checks win conditions
/// </summary>
[Obsolete("Use ParchisTurnActionFactory.Create() instead")]
public class ParchisTurnPhase
{
    // This class is kept for reference only.
    // All logic has been moved to workflow nodes.
}
