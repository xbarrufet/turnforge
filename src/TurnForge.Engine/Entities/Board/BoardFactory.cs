using TurnForge.Engine.Domain.Board.Spatial.Interfaces;
using TurnForge.Engine.Entities.Board.Definitions;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class BoardFactory : IBoardFactory
{

    private readonly IBoardTopologyFactory _topologyFactory;
    private readonly ISpatialIndexFactory _spatialIndexFactory;
    
    public BoardFactory(IBoardTopologyFactory topologyFactory, ISpatialIndexFactory spatialIndexFactory)
    {
        _topologyFactory = topologyFactory;
        _spatialIndexFactory = spatialIndexFactory;
    }
    
    public IGameBoard CreateGameBoard(IBoardDefinition definition)
    {
        ISpatialIndex spatialIndex = _spatialIndexFactory.CreateSpatialIndex();
        return definition switch
        {
            DiscretBoardDefinition discreteDef => new GameBoard(
                EntityId.New(),
                BoardKind.Discrete,
                _topologyFactory.CreateDiscreteTopology(discreteDef.Edges), 
                spatialIndex),
            _ => throw new NotImplementedException()
        };
    }

}