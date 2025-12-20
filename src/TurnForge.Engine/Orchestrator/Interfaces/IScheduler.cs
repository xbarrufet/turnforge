using TurnForge.Engine.Entities.Decisions.Interfaces;

namespace TurnForge.Engine.Orchestrator.Interfaces;

public interface IScheduler
{
    IScheduler Add(IEnumerable<IDecision> decisions);
    IScheduler Remove(IDecision decision);
    IEnumerable<IDecision> GetDecisions(Func<IDecision, bool> predicate);
    IEnumerable<IDecision> GetAll();
}
