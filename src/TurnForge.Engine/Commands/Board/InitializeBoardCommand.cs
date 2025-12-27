using TurnForge.Engine.Core;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Definitions.Board.Descriptors;

namespace TurnForge.Engine.Commands.Board;

/// <summary>
/// Command to initialize the game board with spatial model and zones.
/// This is executed once at game start before spawning any entities.
/// </summary>
public sealed record InitializeBoardCommand(BoardDescriptor Descriptor) : ICommand
{
    public Type CommandType => typeof(InitializeBoardCommand);
}
