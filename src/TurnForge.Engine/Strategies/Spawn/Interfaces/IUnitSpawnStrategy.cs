namespace TurnForge.Engine.Strategies.Spawn.Interfaces;

public interface IUnitSpawnStrategy
{
    IReadOnlyList<UnitSpawnDecision> Decide(UnitSpawnContext ctx);

}