using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Commands.ACK
{
    public sealed record ACKCommand : ICommand
    {
        public Type CommandType => typeof(ACKCommand);
    }
}