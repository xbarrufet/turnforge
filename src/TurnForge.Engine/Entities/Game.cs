using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities.Interfaces;
using TurnForge.Engine.States;

namespace TurnForge.Engine.Entities;

public class Game : IGame
{
    public GameId Id { get;  }
    
    public GameBoard GameBoard { get; }
    private GameState _gameState = new GameState();
    
    public Game(GameBoard gameBoard)
    {
        Id = new GameId();
        GameBoard = gameBoard;
    }
    public Game(
        GameId id,
        GameBoard gameBoard)
    {
        Id = id;
        GameBoard = gameBoard;
    }
    
    public Game(  IReadOnlyList<Actor> actor,
        GameBoard gameBoard)
    {
        Id = new GameId();
        GameBoard = gameBoard;
    }
    
    public void AddUnit(Unit unit)
    {
        _gameState.AddUnit(unit);
    }
    public void AddHostile(Hostile hostile) 
    {
        _gameState.AddHostile(hostile);
    }
    
    public void AddProp(Prop prop)
    {
        _gameState.AddProp(prop);
    }
    
    public IReadOnlyGameState GetReadOnlyState()
    {
        return new ReadOnlyGameState(_gameState);
    }
    
    public GameState GetGameState()
    {
        return _gameState;
    }
    
    
    
}