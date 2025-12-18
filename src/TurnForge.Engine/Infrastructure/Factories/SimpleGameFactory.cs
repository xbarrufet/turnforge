using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;

namespace TurnForge.Engine.Infrastructure.Appliers;

public class SimpleGameFactory: IGameFactory
{
    public Game Build(GameBoard gameBoard)
    {
        return new Game(gameBoard);
    }
}