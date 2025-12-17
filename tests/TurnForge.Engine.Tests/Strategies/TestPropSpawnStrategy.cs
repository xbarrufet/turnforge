

using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

public class TestPropSpawnStrategy:IPropSpawnStrategy
{
    public IReadOnlyList<PropSpawnDecision> Decide(PropSpawnContext context)
    {
        List<PropSpawnDecision> decisions = new();
        foreach (var descriptor in context.Descriptors)
        {
            decisions.Add(new PropSpawnDecision(
                descriptor,
                descriptor.Position ?? new Position(0, 0)));
        }
        return decisions;
        
    }
}