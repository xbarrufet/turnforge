
using System.Collections.Generic;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Descriptors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Board.Descriptors;

namespace TurnForge.Engine.Commands.Game;

public sealed record InitGameCommand(
    SpatialDescriptor Spatial,
    IReadOnlyList<ZoneDescriptor> Zones,
    IReadOnlyList<PropDescriptor> StartingProps,
    IReadOnlyList<AgentDescriptor> Agents
) : ICommand;
