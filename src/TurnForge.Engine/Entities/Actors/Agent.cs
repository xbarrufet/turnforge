using TurnForge.Engine.Entities.Actors.Components;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public class Agent : Actor
{
    public AgentDefinition Definition { get; }
    public HealthComponent Health { get; }

    public Agent(
        ActorId id,
        Position position,
        AgentDefinition definition,
        IReadOnlyList<IActorBehaviour>? behaviours = null)
        : base(id, position, behaviours)
    {
        Definition = definition;
        Health = new HealthComponent(definition.MaxHealth);
    }

    public bool IsAlive => Health.IsAlive;
}