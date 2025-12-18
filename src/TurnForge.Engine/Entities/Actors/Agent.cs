using TurnForge.Engine.Entities.Actors.Components;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class Agent : Actor
{
    public HealthComponent Health { get; }

    protected Agent(
        ActorId id,
        Position position,
        HealthComponent health,
        IReadOnlyList<IActorBehaviour>? behaviours = null)
        : base(id, position, behaviours)
    {
        Health = health;
    }

    public bool IsAlive => Health.IsAlive;
}