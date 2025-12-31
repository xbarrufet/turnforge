using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Core.Fsm.V2;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Commands;

namespace Parchis.Rules.Fsm.Phases;

/// <summary>
/// The single phase for a Parchís turn.
/// Handles the sequence: Roll Dice -> Move Piece.
/// If a 6 is rolled, the player may roll again (handled by rule logic granting extra AP or resetting state).
/// </summary>
public class ParchisTurnPhase : GamePhase
{
    public ParchisTurnPhase()
    {
        Name = "ParchisTurn";
    }

    public override IReadOnlyList<Type> AllowedCommands => new[]
    {
        typeof(RollDiceCommand),
        typeof(MovePieceCommand),
        typeof(PassTurnCommand)
    };

    public override PhaseResult Action(GameState state)
    {
        // Check if player has rolled
        var hasRolled = state.Metadata.ContainsKey("DiceResult");
        
        // If not rolled, we wait for RollDiceCommand (or Pass)
        if (!hasRolled)
        {
            return PhaseResult.Pass();
        }

        // If rolled, we wait for MovePieceCommand (or Pass)
        // If logic allows auto-pass (no moves), it should be handled here or by a bot?
        // For now, we wait for user input.
        return PhaseResult.Pass();
    }

    public override bool ShouldMoveToNext(GameState state)
    {
        // Phase ends when player has no more actions.
        // Base implementation checks ActionPoints > 0.
        // We assume Parchís rules manage ActionPoints (1 AP = 1 active turn sequence).
        // If 6 is rolled, rules might grant +1 AP, keeping the phase active.
        return base.ShouldMoveToNext(state);
    }
}
