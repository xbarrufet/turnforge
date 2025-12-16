using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public class Game
{
    public GameId Id { get;  }
    private readonly Dictionary<ActorId, Actor> _actors = new();
    public IReadOnlyDictionary<ActorId, Actor> Actors => _actors;
  
    private readonly Dictionary<ActorId, Prop> _props = new();
    public IReadOnlyDictionary<ActorId, Prop> Prop => _props;
    public GameBoard GameBoard { get; }
    
    public Game(GameBoard gameBoard)
    {
        Id = new GameId();
        _actors = [];
        _props = [];
        GameBoard = gameBoard;
    }
    public Game(
        GameId id,
        GameBoard gameBoard)
    {
        Id = id;
        _actors = [];
        _props = [];
        GameBoard = gameBoard;
    }
    
    public Game(  IReadOnlyList<Actor> actor,
        GameBoard gameBoard)
    {
        Id = new GameId();
        _actors = [];
        _props = [];
        GameBoard = gameBoard;
    }
    
    public void AddActor(Actor actor)
    {
        _actors[actor.Id] = actor;
    }
    
    public void AddProp(Prop prop)
    {
        _props[prop.Id] = prop;
    }
    
    
    
}