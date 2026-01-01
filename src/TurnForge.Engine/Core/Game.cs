using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Actors; // For Actor
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Definitions.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core;

public class Game : IGame
{
    public GameId Id { get;  }
    
    public IGameBoard GameBoard { get; }
    public Game(IGameBoard gameBoard)
    {
        Id = new GameId();
        GameBoard = gameBoard;
    }
    public Game(
        GameId id,
        IGameBoard gameBoard)
    {
        Id = id;
        GameBoard = gameBoard;
    }
    
    public Game(  IReadOnlyList<Actor> actor,
        IGameBoard gameBoard)
    {
        Id = new GameId();
        GameBoard = gameBoard;
    }
    
    
    
}