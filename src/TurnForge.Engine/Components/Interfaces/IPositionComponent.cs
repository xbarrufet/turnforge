using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components.Interfaces
{
    public interface IPositionComponent:IGameEntityComponent
    {
        public IBoardPosition CurrentPosition { get; set;}
      
        public static IPositionComponent Empty(BoardPositionKind kind)
        {
            return kind switch
            {
                BoardPositionKind.Tile => new BasePositionComponent(),
                BoardPositionKind.Vector => new BasePositionComponent(),
                _ => throw new NotImplementedException()
            };  
        }
        public static IPositionComponent Empty()
        {
            return new BasePositionComponent();
        }
    }
}