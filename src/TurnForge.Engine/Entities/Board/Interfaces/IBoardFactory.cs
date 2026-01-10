namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface IBoardFactory
{
   IGameBoard CreateGameBoard(BoardDescriptor boardDescriptor);
   
}