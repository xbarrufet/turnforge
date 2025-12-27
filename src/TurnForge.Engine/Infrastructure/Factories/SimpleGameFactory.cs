using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Board;

using TurnForge.Engine.Infrastructure.Factories.Interfaces;
namespace TurnForge.Engine.Infrastructure.Factories;

public class SimpleGameFactory : IGameFactory
{
    public Game Build(GameBoard gameBoard)
    {
        return new Game(gameBoard);
    }
}