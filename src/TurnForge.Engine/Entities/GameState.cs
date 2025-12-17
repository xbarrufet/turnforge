using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.States;

public sealed class GameState
{
    public Dictionary<ActorId, Unit> Units { get; } = new();
}