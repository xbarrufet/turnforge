namespace TurnForge.Engine.Entities.Decisions.Interfaces;

using TurnForge.Engine.Orchestrator;

public interface IDecision
{
    DecisionTiming Timing { get; }
    string OriginId { get; }
}