using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Definitions;

public sealed class DiscretBoardDefinition : BaseGameEntityDefinition, IBoardDefinition
{
    private readonly List<(TileId positionFrom, TileId positionTo)> _edges = new();

    public DiscretBoardDefinition(string id) : base(id, "Board")
    {
    }

    public BoardKind Kind => BoardKind.Discrete;

    public IReadOnlyList<(TileId positionFrom, TileId positionTo)> Edges => _edges;

    // TODO: Implement props support for discrete boards if needed
    public IReadOnlyList<BoardPropDefinition>? Props => null;

    public void AddTileFromStringConnection(string positionFrom, string positionTo)
    {
        _edges.Add((new TileId(positionFrom), new TileId(positionTo)));
    }
}