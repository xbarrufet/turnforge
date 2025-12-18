using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Repositories.InMemory;

public class InMemoryGameRepository:IGameRepository
{
    private readonly Dictionary<GameId, Game> _games = new();
    private Game _currentGame = null!;
    private GameState _gameState;

    public void SaveGame(Game game)
    {
        _games[game.Id] = game;
        _currentGame = game;
    }

    public Game LoadGame(GameId gameId)
    {
        _currentGame = _games[gameId];
        return _currentGame;
    }

    public Game? GetCurrent()
    {
        return _currentGame;
    }

    public GameState Load()
    {
        return _gameState;
    }

    public void SaveGameState(GameState state)
    {
        _gameState = state;
    }
}