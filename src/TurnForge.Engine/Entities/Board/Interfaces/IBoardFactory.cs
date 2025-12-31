namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface IBoardFactory
{
   IGameBoard CreateGameBoard(IBoardDefinition definition);
}