using TurnForge.Engine.Definitions;

namespace TurnForge.Engine.Strategies.Spawn.Interfaces;

public interface ISpawnContext
{
    GameState GameState { get;  }
}