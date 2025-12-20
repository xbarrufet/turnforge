using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;

namespace TurnForge.Engine.Infrastructure.Appliers;

public sealed class BoardApplier() : IBoardApplier
{

    public GameBoard Apply(SpatialDescriptor spatial, IEnumerable<ZoneDescriptor> zones)
    {
        // 1️⃣ Spatial

        // 2️⃣ Board
        var board = new GameBoard(BuildSpatialModel(spatial));
        // 3️⃣ Zones
        foreach (var zoneDesc in zones)
        {
            var zone = new Zone(
                new TurnForge.Engine.ValueObjects.EntityId(GenerateGuid(zoneDesc.Id.Value)),
                zoneDesc.Bound,
                new TurnForge.Engine.Entities.Components.BehaviourComponent(zoneDesc.Behaviours.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>()));
            board.AddZone(zone);
        }
        return board;
    }

    private ISpatialModel BuildSpatialModel(SpatialDescriptor spatial)
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
    private static Guid GenerateGuid(string input)
    {
        using var provider = System.Security.Cryptography.MD5.Create();
        var hash = provider.ComputeHash(System.Text.Encoding.Default.GetBytes(input));
        return new Guid(hash);
    }
}
