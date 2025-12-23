using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.LoadGame.Descriptors;

public sealed record DiscreteSpatialDescriptor(
    IReadOnlyList<TileId> Nodes,
    IReadOnlyList<DiscreteConnectionDescriptor> Connections
) : SpatialDescriptor;