using TurnForge.Engine.Entities;
using TurnForge.Engine.States;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Repositories.Interfaces;

public interface IGameRepository
{

    void SaveGame(Game game);
    Game LoadGame(GameId gameId);
    Game? GetCurrent();
    GameState Load();
    void Save(GameState state);
    
    
}