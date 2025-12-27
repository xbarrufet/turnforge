using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Strategies.Spawn;

/// <summary>
/// Decision that carries an already-created Prop entity ready to be added to GameState.
/// The Strategy is responsible for creating the entity via Factory.
/// </summary>
public record PropSpawnDecision(Prop Entity) : ISpawnDecision<Prop>, IDecision
{
    public DecisionTiming Timing { get; init; } = DecisionTiming.Immediate;
    public string OriginId { get; init; } = "System";
}
