using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Definitions.Interfaces;
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