using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Strategies.Spawn;

public sealed class AgentSpawnDecision
    (AgentTypeId typeId,
        Position position,
        IReadOnlyList<IActorBehaviour> behaviours
    ) : ISpawnDecision
{
    public Position Position { get; } = position;
    public IReadOnlyList<IActorBehaviour> ExtraBehaviours { get; } = behaviours;
    public AgentTypeId TypeId { get; } = typeId;
}
