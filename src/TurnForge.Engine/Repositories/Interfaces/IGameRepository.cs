using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Repositories.Interfaces;

public interface IGameRepository
{

    void SaveGame(Game game);
    Game LoadGame(GameId gameId);
    Game? GetCurrent();
    GameState Load();
    void SaveGameState(GameState state);
    
    
}