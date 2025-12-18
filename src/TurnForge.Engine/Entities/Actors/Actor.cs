using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class Actor
{
    public ActorId Id { get; }
    public Position Position { get; protected set; }
    public IReadOnlyList<IActorBehaviour>? Behaviours { get; }

    protected Actor(ActorId id, Position position, IReadOnlyList<IActorBehaviour>? behaviours = null)
    {
        Id = id;
        Position = position;
        Behaviours = behaviours;
    }
}