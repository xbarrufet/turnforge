using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Orchestrator;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Strategies.Spawn;


using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;

public sealed record PropSpawnDecision(
    PropTypeId TypeId,
    Position Position,
    IReadOnlyList<IActorBehaviour>? ExtraBehaviours = null
) : ISpawnDecision<Prop>
{
    public DecisionTiming Timing { get; init; } = DecisionTiming.Immediate;
    public string OriginId { get; init; } = "System";
    public IGameEntityDescriptor<Prop> Descriptor => new PropDescriptor(TypeId, Position, ExtraBehaviours);
}
