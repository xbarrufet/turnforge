namespace TurnForge.Engine.Commands.Game.Definitions;

using TurnForge.Engine.ValueObjects;

public sealed record DiscreteConnectionDeacriptor(
    Position From,
    Position To
);