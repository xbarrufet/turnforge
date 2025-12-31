namespace TurnForge.Engine.Entities.Board.Enums;

public enum BoardPositionKind
{
    Tile,
    Vector,
    Connection,
    Area

}

public static class BoardPositionKindExtensions
{
    public static BoardPositionKind ToPositionKind(this BoardKind boardKind)
    {
        return boardKind switch
        {
            BoardKind.Discrete => BoardPositionKind.Tile,
            BoardKind.Continuous => BoardPositionKind.Vector,
            _ => throw new NotImplementedException()
        };
    }
}

