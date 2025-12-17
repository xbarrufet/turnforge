using TurnForge.Engine.Commands.LoadGame.Descriptors;

namespace TurnForge.Engine.Commands.Game.Descriptors;

using TurnForge.Engine.ValueObjects;

public sealed record ContinuousSpatialDescriptior(
    BoundsDescriptor Bounds
) : SpatialDescriptor;