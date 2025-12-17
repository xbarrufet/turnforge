using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Rules.BarelyAlive.Actors;

public class ZombieSpawnOrderTrait(int spawnOrder) : IActorTrait
{
    public int SpawnOrder { get; } = spawnOrder;
}