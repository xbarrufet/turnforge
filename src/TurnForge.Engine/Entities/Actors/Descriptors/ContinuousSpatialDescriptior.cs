using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Spawn.Descriptors;

public sealed record ContinuousSpatialDescriptior(
    BoundsDescriptor Bounds
) : SpatialDescriptor;