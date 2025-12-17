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
            .OfType<PartySpawnProp>()
            .FirstOrDefault();
        if (spawnPoint == null)
            throw new InvalidOperationException("No PartySpawnActor found.");
        return spawnPoint.Position;
    }

    public IReadOnlyList<UnitSpawnDecision> Decide(UnitSpawnContext ctx)
    {
        var decision =new  List<UnitSpawnDecision>();
        foreach (var unitDescriptor in ctx.PlayerUnits)
        {
            // For simplicity, spawn all units at the same position
            var spawnPosition = DetermineSpawnLocation(ctx);
            decision.Add(new UnitSpawnDecision(unitDescriptor, spawnPosition));
        }
        return decision;
    }
}