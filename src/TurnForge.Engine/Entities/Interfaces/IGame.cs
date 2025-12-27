using System.Collections.Generic;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Interfaces;

public interface IGame
{
    GameId Id { get; }
    GameBoard GameBoard { get; }


}
