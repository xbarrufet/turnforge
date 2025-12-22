using System.Collections.Generic;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Commands.Game;

public sealed record SpawnStartingPropsCommand(
    IReadOnlyList<PropDescriptor> StartingProps
) : ICommand
{
    public Type CommandType => typeof(SpawnStartingPropsCommand);
}
