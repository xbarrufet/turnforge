using TurnForge.Engine.Traits;
using System.Linq;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Definitions.Board.Descriptors;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Components;
using TurnForge.Engine.Definitions.Descriptors.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Board;

public sealed class BoardFactory : IBoardFactory
{
    public GameBoard Build(IGameEntityDescriptor<GameBoard> descriptor)
    {
        var boardDescriptor = (BoardDescriptor)descriptor;
        var board = new GameBoard(BuildSpatialModel(boardDescriptor.Spatial));

        foreach (var zoneDesc in boardDescriptor.Zones)
        {
            ZoneDefinition zoneDefinition = new ZoneDefinition(
                new ZoneId(zoneDesc.Id.Value), 
                zoneDesc.Id.Value, // Using Id as Name since no dedicated Name field in descriptor yet
                zoneDesc.Bound);
            var zone = new Zone(zoneDefinition);
            board.AddZone(zone);
        }

        return board;
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

    private static Guid GenerateGuid(string input)
    {
        var hash = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.Default.GetBytes(input));
        return new Guid(hash);
    }
}