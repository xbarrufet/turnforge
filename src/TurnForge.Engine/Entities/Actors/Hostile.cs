using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public sealed class Hostile(
    ActorId id,
    Position position,
    int health = 5,
    int actionPoints = 2)
    : Npc(id, position, health, actionPoints);