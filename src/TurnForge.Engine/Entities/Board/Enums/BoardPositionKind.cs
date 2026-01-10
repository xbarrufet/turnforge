namespace TurnForge.Engine.Entities.Board.Enums;

public enum BoardPositionKind
{
    Tile,
    Vector,
    Connection,
    Area,
    Limbo

}

public static class BoardPositionKindExtensions
{
    public static BoardPositionKind ToPositionKind(this TopologyKind topologyKind)
    {
        return topologyKind switch
        {
            TopologyKind.Discrete => BoardPositionKind.Tile,
            TopologyKind.Continuous => BoardPositionKind.Vector,
            _ => throw new NotImplementedException()
        };
    }
}

