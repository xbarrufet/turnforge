using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;

namespace TurnForge.Engine.Infrastructure.Appliers;

public sealed class BoardApplier() : IBoardApplier
{

    public GameBoard Apply(InitializeGameCommand command)
    {
        // 1️⃣ Spatial
        
        // 2️⃣ Board
        var board = new GameBoard(BuildSpatialModel(command.Spatial));
        // 3️⃣ Zones
        foreach (var zoneDesc in command.Zones)
        {
            var zone = new Zone(
                zoneDesc.Id,
                zoneDesc.Bound,
                zoneDesc.Behaviours);
            board.AddZone(zone);
        }
        return board;
    }
    
    private ISpatialModel BuildSpatialModel( SpatialDescriptor spatial)
    {
        return spatial switch
        {
            DiscreteSpatialDescriptor d => BuildDiscreteSpatialModel(d),
            ContinuousSpatialDescriptior =>
                throw new NotImplementedException(),
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
