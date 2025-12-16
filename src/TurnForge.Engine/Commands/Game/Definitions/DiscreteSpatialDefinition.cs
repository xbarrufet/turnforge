using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Game.Definitions;

public sealed record DiscreteSpatialDefinition(
    IReadOnlyList<Position> Nodes,
    IReadOnlyList<DiscreteConnectionDefinition> Connections
) : SpatialDefinition;