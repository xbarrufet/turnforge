using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.LoadGame.Descriptors;

public sealed record DiscreteSpatialDescriptor(
    IReadOnlyList<Position> Nodes,
    IReadOnlyList<DiscreteConnectionDeacriptor> Connections
) : SpatialDescriptor;