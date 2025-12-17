using System.Collections.Generic;
using TurnForge.Engine.Commands.Game.Descriptors;

namespace TurnForge.Engine.Commands.LoadGame.Descriptors;

public readonly struct DiscreteSpatialDefinitionDto(IReadOnlyList<PositionDto> nodes, IReadOnlyList<DiscreteConnectionDefinitionDto> connections)
{
    public IReadOnlyList<PositionDto> Nodes { get; init; }= nodes;
    public IReadOnlyList<DiscreteConnectionDefinitionDto> Connections { get; init; }= connections;
}

public readonly struct DiscreteConnectionDefinitionDto(PositionDto from, PositionDto to)
{
        public PositionDto From { get; init; } = from;
        public PositionDto To { get; init; } = to;
}