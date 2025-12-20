using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.LoadGame.Descriptors;

public sealed record DiscreteConnectionDeacriptor(
    TileId From,
    TileId To
);