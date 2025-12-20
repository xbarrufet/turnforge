using System;
using System.Collections.Generic;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;

namespace TurnForge.Engine.Entities.Board;

public sealed class BoardFactory : IBoardFactory
{
    public GameBoard Build(IGameEntityDescriptor<GameBoard> descriptor)
    {
        var boardDescriptor = (BoardDescriptor)descriptor;
        return new GameBoard(BuildSpatialModel(boardDescriptor.Spatial));
    }

    private ISpatialModel BuildSpatialModel(SpatialDescriptor spatial)
    {
        return spatial switch
        {
            DiscreteSpatialDescriptor d => BuildDiscreteSpatialModel(d),
            // ContinuousSpatialDescriptior => throw new NotImplementedException(),
            _ => throw new NotSupportedException()
        };
    }

    private ISpatialModel BuildDiscreteSpatialModel(DiscreteSpatialDescriptor d)
    {
        var graph = new MutableTileGraph(d.Nodes.ToHashSet());
        foreach (var connection in d.Connections)
        {
            graph.EnableEdge(connection.From, connection.To);
        }

        return new ConnectedGraphSpatialModel(graph);
    }
}