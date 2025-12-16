namespace TurnForge.Engine.Commands.Game.Definitions;

using TurnForge.Engine.ValueObjects;

public sealed record DiscreteConnectionDefinition(
    Position From,
    Position To
);