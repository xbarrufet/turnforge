using TurnForge.Engine.Core.Orchestrator;

namespace TurnForge.Engine.Entities.Decisions.Interfaces;

public interface IDecision
{
    DecisionTiming Timing { get; }
    string OriginId { get; }
}