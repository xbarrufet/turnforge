using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;

using TurnForge.Engine.Infrastructure.Factories.Interfaces;
namespace TurnForge.Engine.Infrastructure.Factories;

public class SimpleGameFactory : IGameFactory
{
    public Game Build(GameBoard gameBoard)
    {
        return new Game(gameBoard);
    }
}