using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Board;

namespace TurnForge.Engine.Infrastructure.Factories.Interfaces;

public interface IGameFactory
{
    Game Build(GameBoard gameBoard);
}