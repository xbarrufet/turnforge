using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public sealed class Unit(
    ActorId id,
    Position position,
    int health = 10,
    int actionPoints = 3)
    : Agent(id, position, health, actionPoints);