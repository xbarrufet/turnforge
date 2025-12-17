using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Game.Definitions;

public sealed record DiscreteSpatialDescriptor(
    IReadOnlyList<Position> Nodes,
    IReadOnlyList<DiscreteConnectionDeacriptor> Connections
) : SpatialDescriptor;