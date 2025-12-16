using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public sealed class Civilian(
    ActorId id,
    Position position,
    int health = 3,
    int actionPoints = 1)
    : Npc(id, position, health, actionPoints);