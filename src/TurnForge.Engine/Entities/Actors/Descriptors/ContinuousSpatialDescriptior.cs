using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Game.Descriptors;

public sealed record ContinuousSpatialDescriptior(
    BoundsDescriptor Bounds
) : SpatialDescriptor;