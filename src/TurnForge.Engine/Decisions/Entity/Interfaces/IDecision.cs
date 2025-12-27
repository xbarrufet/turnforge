using TurnForge.Engine.Definitions;
using TurnForge.Engine.Core.Orchestrator;

namespace TurnForge.Engine.Decisions.Entity.Interfaces;

public interface IDecision
{
    DecisionTiming Timing { get; }
    string OriginId { get; }
}