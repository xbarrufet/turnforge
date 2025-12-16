namespace TurnForge.Engine.Commands.Game.Definitions;

using TurnForge.Engine.ValueObjects;

public sealed record ContinuousSpatialDefinition(
    BoundsDefinition Bounds
) : SpatialDefinition;