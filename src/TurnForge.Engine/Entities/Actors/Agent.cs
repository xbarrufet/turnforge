using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class Agent(
    ActorId id,
    Position position,
    int health,
    int actionPoints)
    : Actor(id, position)
{
    public int Health { get; protected set; } = health;
    public int ActionPoints { get; protected set; } = actionPoints;

    public bool IsAlive => Health > 0;
}