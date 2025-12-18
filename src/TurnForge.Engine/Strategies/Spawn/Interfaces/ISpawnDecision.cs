using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Strategies.Spawn.Interfaces;

public interface ISpawnDecision
{
    Position Position { get; }
    IReadOnlyList<IActorBehaviour> ExtraBehaviours { get; }
}