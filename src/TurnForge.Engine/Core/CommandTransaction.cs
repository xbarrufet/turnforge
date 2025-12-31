using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Core;

public class CommandTransaction
{
    public ICommand Command { get; }
    public CommandResult? Result { get; set; }
    public IGameEvent[]? Events { get; set; }
    public bool IsGameOver { get; set; }

    public CommandTransaction(ICommand command)
    {
        Command = command;
    }
}
