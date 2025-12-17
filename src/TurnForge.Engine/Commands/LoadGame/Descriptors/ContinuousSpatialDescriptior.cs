namespace TurnForge.Engine.Commands.Game.Definitions;

using TurnForge.Engine.ValueObjects;

public sealed record ContinuousSpatialDescriptior(
    BoundsDescriptor Bounds
) : SpatialDescriptor;