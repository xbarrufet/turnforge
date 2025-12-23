using System.Collections.Immutable;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Core.Orchestrator.Interfaces;

namespace TurnForge.Engine.Core.Orchestrator;

public sealed record TurnScheduler(ImmutableList<IDecision> Decisions) : IScheduler
{
    public static TurnScheduler Empty => new(ImmutableList<IDecision>.Empty);

    public IScheduler Add(IEnumerable<IDecision> decisions)
    {
        return new TurnScheduler(Decisions.AddRange(decisions));
    }

    public IScheduler Remove(IDecision decision)
    {
        return new TurnScheduler(Decisions.Remove(decision));
    }

    public IEnumerable<IDecision> GetDecisions(Func<IDecision, bool> predicate)
    {
        return Decisions.Where(predicate);
    }

    public IEnumerable<IDecision> GetAll()
    {
        return Decisions;
    }
}
