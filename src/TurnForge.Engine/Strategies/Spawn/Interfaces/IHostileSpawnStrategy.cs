namespace TurnForge.Engine.Strategies.Spawn.Interfaces;

public interface IHostileSpawnStrategy
{
    IReadOnlyList<HostileSpawnDecision> Decide();
}