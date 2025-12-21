using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Adapter.Repositories;

public class InMemoryGameRepository : IGameRepository
{
    private TurnForge.Engine.Core.Game? _currentGame;
    private GameState? _currentState;

    public void SaveGame(TurnForge.Engine.Core.Game game)
    {
        _currentGame = game;
    }

    public TurnForge.Engine.Core.Game LoadGame(GameId gameId)
    {
        if (_currentGame == null || _currentGame.Id != gameId)
            throw new Exception("Game not found");
        return _currentGame;
    }

    public TurnForge.Engine.Core.Game? GetCurrent()
    {
        return _currentGame;
    }

    public GameState LoadGameState()
    {
        if (_currentState == null)
            return TurnForge.Engine.Entities.GameState.Empty();
        return _currentState;
    }

    public void SaveGameState(GameState state)
    {
        _currentState = state;
    }
}
