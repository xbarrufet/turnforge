using TurnForge.Engine.Entities.Board.Enums;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface IBoardPosition
{
    BoardPositionKind Kind { get; }
    
}
