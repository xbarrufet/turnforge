using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities.Actors.Definitions;

public class TestUnitSpawnStrategy:IUnitSpawnStrategy
{
    public IReadOnlyList<UnitSpawnDecision> Decide(UnitSpawnContext context)
    {
        List<UnitSpawnDecision> decisions = new();
        foreach (var descriptor in context.UnitsToSpawn)
        {
            decisions.Add(new UnitSpawnDecision(descriptor.TypeId, Position.Zero, new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()));
        }
        return decisions;
    }
}