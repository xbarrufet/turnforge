using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Strategies.Spawn.Interfaces;

public interface ISpawnContext
{
    GameState GameState { get;  }
}