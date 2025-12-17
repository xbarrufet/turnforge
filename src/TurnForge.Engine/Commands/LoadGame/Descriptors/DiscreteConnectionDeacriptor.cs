using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.LoadGame.Descriptors;

public sealed record DiscreteConnectionDeacriptor(
    Position From,
    Position To
);