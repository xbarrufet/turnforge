using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;

namespace TurnForge.Engine.Infrastructure.Factories.Interfaces;

public interface IGameFactory
{
    Game Build(GameBoard gameBoard);
}