using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class Npc(
    ActorId id,
    Position position,
    int health,
    int actionPoints)
    : Agent(id, position, health, actionPoints);