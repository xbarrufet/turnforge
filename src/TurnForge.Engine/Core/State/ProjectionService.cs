using TurnForge.Engine.Definitions;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Core.State;

public class ProjectionService : IStateProjector
{
    public GameState Project(GameState baseState, IEnumerable<IDecision> decisions)
    {
        var currentState = baseState;
        foreach (var decision in decisions)
        {
            currentState = decision.Apply(currentState);
        }
        return currentState;
    }
}
