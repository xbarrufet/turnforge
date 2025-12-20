using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

public class TestPropSpawnStrategy : IPropSpawnStrategy
{
    public IReadOnlyList<PropSpawnDecision> Decide(PropSpawnContext context)
    {
        List<PropSpawnDecision> decisions = new();
        foreach (var descriptor in context.PropsToSpawn)
        {
            decisions.Add(new PropSpawnDecision(
                descriptor.TypeId,
                descriptor.Position ?? new Position(Vector.Zero),
                descriptor.ExtraBehaviours ?? new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()));
        }
        return decisions;
    }
}