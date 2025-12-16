using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Game.Definitions;

public sealed record ActorDefinition(
    ActorKind Kind,
    ActorId ActorId,
    Position StartPosition);
