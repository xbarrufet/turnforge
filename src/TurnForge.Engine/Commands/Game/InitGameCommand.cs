
using System.Collections.Generic;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Commands.Game;

public sealed record InitGameCommand(
    SpatialDescriptor Spatial,
    IReadOnlyList<ZoneDescriptor> Zones,
    IReadOnlyList<TurnForge.Engine.Entities.Descriptors.PropDescriptor> StartingProps
) : ICommand
{
    public Type CommandType => typeof(InitGameCommand);
}