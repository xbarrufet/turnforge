using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.LoadGame.Descriptors;

public sealed record DiscreteConnectionDescriptor(
    ConnectionId Id,
    TileId From,
    TileId To
);