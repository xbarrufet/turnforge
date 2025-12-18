using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core;

public class Game : IGame
{
    public GameId Id { get;  }
    
    public GameBoard GameBoard { get; }
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
    
    
    
}