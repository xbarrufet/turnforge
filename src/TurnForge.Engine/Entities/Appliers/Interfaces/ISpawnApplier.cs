using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface ISpawnApplier
{
    GameState Apply(IEnumerable<ISpawnDecision> decisions, GameState state);
}