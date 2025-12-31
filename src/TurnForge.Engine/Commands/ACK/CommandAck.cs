using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.ValueObjects;

namespace TurnForge.Engine.Commands.ACK
{
    public sealed record ACKCommand : ICommand
    {
        public static CommandType CommandType => CommandType.ACK;

        CommandType ICommand.CommandType => CommandType;
    }
}