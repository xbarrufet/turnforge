using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Rules.BarelyAlive.Actors;

namespace TurnForge.Rules.BarelyAlive.Strategies.Spawn;

public class SurvivorSpawnStrategy:IUnitSpawnStrategy
{
    
    private Position DetermineSpawnLocation(UnitSpawnContext ctx)
    {
        // Look for PartySpawnActor points
        var spawnPoint = ctx.GameState.Props
            .FirstOrDefault(p => p.Value.Definition.TypeId == BarelyAliveTypes.PartySpawn).Value;
        if (spawnPoint == null)
            throw new InvalidOperationException("No PartySpawnActor found.");
        return spawnPoint.Position;
    }

    public IReadOnlyList<UnitSpawnDecision> Decide(UnitSpawnContext ctx)
    {
        var decision = new List<UnitSpawnDecision>();
        foreach (var unitDescriptor in ctx.UnitsToSpawn)
        {
            // For simplicity, spawn all units at the same position
            var spawnPosition = DetermineSpawnLocation(ctx);
            decision.Add(new UnitSpawnDecision(
                unitDescriptor.TypeId, 
                spawnPosition,
                unitDescriptor.ExtraBehaviours ?? Array.Empty<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()));
        }
        return decision;
    }
}