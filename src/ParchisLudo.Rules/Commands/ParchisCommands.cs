using TurnForge.Engine.Commands.Interfaces;

namespace Parchis.Rules.Commands;






/// <summary>
/// Command to roll the dice.
/// </summary>
public class RollDiceCommand : ICommand
{
    public Type CommandType => typeof(RollDiceCommand);
}

/// <summary>
/// Command to move a piece.
/// </summary>
public class MovePieceCommand : ICommand
{
    public int PieceNumber { get; init; }
    public int Steps { get; init; }
    public Type CommandType => typeof(MovePieceCommand);
}

/// <summary>
/// Command to pass the turn (when no valid moves available).
/// </summary>
public class PassTurnCommand : ICommand
{
    public Type CommandType => typeof(PassTurnCommand);
}

/// <summary>
/// Command to end the current turn and switch to next player.
/// </summary>
public class EndTurnCommand : ICommand
{
    public Type CommandType => typeof(EndTurnCommand);
}
