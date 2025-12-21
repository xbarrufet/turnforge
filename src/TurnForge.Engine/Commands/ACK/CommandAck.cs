using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Commands.ACK
{
    public sealed record CommandAck : ICommand
    {
        public Type CommandType => typeof(CommandAck);
    }
}