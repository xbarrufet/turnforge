using System.Collections.Generic;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.States;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Interfaces;

public interface IGame
{
    GameId Id { get; }
    GameBoard GameBoard { get; }


}
