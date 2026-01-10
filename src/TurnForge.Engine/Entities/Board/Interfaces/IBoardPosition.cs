using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface IBoardPosition
{
    BoardPositionKind Kind { get; }
    IBoardPositionId Id { get; }
    public static IBoardPosition Limbo => new LimboPosition();
    bool IsLimbo() => Kind == BoardPositionKind.Limbo;

}


public class LimboPosition : IBoardPosition
{
    public BoardPositionKind Kind => BoardPositionKind.Limbo;
    public IBoardPositionId Id => new LimboPositionId();
}

