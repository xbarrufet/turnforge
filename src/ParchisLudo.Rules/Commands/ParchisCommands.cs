using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.ValueObjects;

namespace Parchis.Rules.Commands;

public class RollDiceCommand : ICommand
{
    public static readonly CommandType Type = new("RollDice");
    public CommandType CommandType => Type;
}

public class MovePieceCommand : ICommand
{
    public int PieceNumber { get; init; }
    public int Steps { get; init; }
    
    public static readonly CommandType Type = new("MovePiece");
    public CommandType CommandType => Type;
}

public class PassTurnCommand : ICommand
{
    public static readonly CommandType Type = new("PassTurn");
    public CommandType CommandType => Type;
}

public class EndTurnCommand : ICommand
{
    public static readonly CommandType Type = new("EndTurn");
    public CommandType CommandType => Type;
}
