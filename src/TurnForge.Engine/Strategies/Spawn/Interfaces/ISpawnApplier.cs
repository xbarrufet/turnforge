using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Strategies.Spawn.Interfaces;


public interface ISpawnApplier
{
    void Spawn(PropSpawnDecision decision);
    void Spawn(UnitSpawnDecision decision);
    void Spawn(HostileSpawnDecision hostile);
}