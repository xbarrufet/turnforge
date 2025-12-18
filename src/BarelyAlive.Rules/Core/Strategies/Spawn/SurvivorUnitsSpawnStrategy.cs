using BarelyAlive.Rules.Core.Behaviours.ActorBehaviours;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Strategies.Spawn;

public class SurvivorUnitsSpawnStrategy:IUnitSpawnStrategy
{
    
    private Position DetermineSpawnLocation(UnitSpawnContext ctx)
    {
        // Look for PartySpawnActor points
        var spawnPoint = ctx.GameState.GetProps()
            .FirstOrDefault(p =>
                p.Behaviours != null && p.Behaviours.Any(b => b is PartySpawn));

        if (spawnPoint == null)
            throw new InvalidOperationException("No PartySpawnActor found.");
        return spawnPoint.Position;

    }

    public IReadOnlyList<UnitSpawnDecision> Decide(UnitSpawnContext ctx)
    {
        var decision =new  List<UnitSpawnDecision>();
        foreach (var unitDescriptor in ctx.UnitsToSpawn)
        {
            // For simplicity, spawn all units at the same position
            var spawnPosition = DetermineSpawnLocation(ctx);
            decision.Add(new UnitSpawnDecision(unitDescriptor.TypeId, spawnPosition,
                unitDescriptor.ExtraBehaviours ?? Array.Empty<IActorBehaviour>()));
        }
        return decision;
    }
}