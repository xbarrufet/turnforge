using NUnit.Framework;
using TurnForge.Engine.Entities.Board.Topology.Discrete;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Entities.Factory;

[TestFixture]
public class TileGraphFactoryTests
{
    [Test]
    public void CreateEmptyTileGraph_ReturnsEmptyGraph()
    {
        var tileGraph = TileGraphFactory.CreateEmptyTileGraph();
        
        Assert.That(tileGraph, Is.Not.Null);
    }

    [Test]
    public void CreateDefaultTileGraph_WithConnections_CreatesGraphCorrectly()
    {
        var connections = new[] { (new TileId("A"), new TileId("B")), (new TileId("B"), new TileId("C")) };
        
        var tileGraph = TileGraphFactory.CreateDefaultTileGraph(connections);
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.IsInsideZone(new TileId("A")), Is.True);
        Assert.That(tileGraph.IsInsideZone(new TileId("B")), Is.True);
        Assert.That(tileGraph.IsInsideZone(new TileId("C")), Is.True);
    }

    [Test]
    public void CreateSingleTileGraph_CreatesSelfLoop()
    {
        var tileId = new TileId("Tile1");
        
        var tileGraph = TileGraphFactory.CreateSingleTileGraph(tileId);
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.IsInsideZone(tileId), Is.True);
        Assert.That(tileGraph.CanTraverse(tileId, tileId), Is.True);
        Assert.That(tileGraph.Distance(tileId, tileId), Is.EqualTo(0));
    }

    [Test]
    public void CreateTrackTileGraph_LinearTrack_CreatesCorrectConnections()
    {
        var tileIds = new List<TileId> { new TileId("A"), new TileId("B"), new TileId("C") };
        
        var tileGraph = TileGraphFactory.CreateTrackTileGraph(tileIds);
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.IsInsideZone(new TileId("A")), Is.True);
        Assert.That(tileGraph.IsInsideZone(new TileId("B")), Is.True);
        Assert.That(tileGraph.IsInsideZone(new TileId("C")), Is.True);
        Assert.That(tileGraph.CanTraverse(new TileId("A"), new TileId("B")), Is.True);
        Assert.That(tileGraph.Distance(new TileId("A"), new TileId("C")), Is.EqualTo(2));
    }

    [Test]
    public void CreateTrackTileGraph_CircularTrack_AddsLoopConnection()
    {
        var tileIds = new List<TileId> { new TileId("A"), new TileId("B"), new TileId("C") };
        
        var tileGraph = TileGraphFactory.CreateTrackTileGraph(tileIds, circular: true);
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.CanTraverse(new TileId("C"), new TileId("A")), Is.True);
        Assert.That(tileGraph.Distance(new TileId("C"), new TileId("A")), Is.EqualTo(1));
    }

    [Test]
    public void CreateTrackTileGraph_BidirectionalTrack_AddsBothDirections()
    {
        var tileIds = new List<TileId> { new TileId("A"), new TileId("B") };
        
        var tileGraph = TileGraphFactory.CreateTrackTileGraph(tileIds, bidirectional: true);
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.CanTraverse(new TileId("A"), new TileId("B")), Is.True);
        Assert.That(tileGraph.CanTraverse(new TileId("B"), new TileId("A")), Is.True);
    }

    [Test]
    public void CreateTrackTileGraph_ByCount_CreatesGraphWithTiles()
    {
        var tileGraph = TileGraphFactory.CreateTrackTileGraph(numNodes: 5, pattern: "tile_{0}");
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.IsInsideZone(new TileId("tile_0")), Is.True);
        Assert.That(tileGraph.IsInsideZone(new TileId("tile_4")), Is.True);
    }

    [Test]
    public void CreateTrackTileGraph_CircularAndBidirectional_CreatesCorrectly()
    {
        var tileIds = new List<TileId> { new TileId("A"), new TileId("B"), new TileId("C") };
        
        var tileGraph = TileGraphFactory.CreateTrackTileGraph(tileIds, circular: true, bidirectional: true);
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.CanTraverse(new TileId("A"), new TileId("B")), Is.True);
        Assert.That(tileGraph.CanTraverse(new TileId("B"), new TileId("A")), Is.True);
        Assert.That(tileGraph.CanTraverse(new TileId("C"), new TileId("A")), Is.True);
    }

    [Test]
    public void CreateTrackTileGraph_EmptyList_ReturnsEmptyGraph()
    {
        var tileIds = new List<TileId>();
        
        var tileGraph = TileGraphFactory.CreateTrackTileGraph(tileIds);
        
        Assert.That(tileGraph, Is.Not.Null);
    }

    [Test]
    public void CreateTrackTileGraph_SingleTile_CreatesGraphWithSingleNode()
    {var tileIds = new List<TileId> { new TileId("SingleTile") };
        
        var tileGraph = TileGraphFactory.CreateSingleTileGraph(new TileId("SingleTile") );
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.IsInsideZone(new TileId("SingleTile")), Is.True);
    }

    [Test]
    public void CreateGridTileGraph_3x3Grid_CreatesCorrectConnections()
    {
        var tileGraph = TileGraphFactory.CreateGridTileGraph(rows: 3, columns: 3);
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.IsInsideZone(new TileId("R0C0")), Is.True);
        Assert.That(tileGraph.IsInsideZone(new TileId("R1C1")), Is.True);
        Assert.That(tileGraph.IsInsideZone(new TileId("R2C2")), Is.True);
    }

    [Test]
    public void CreateGridTileGraph_CanTraversHorizontally()
    {
        var tileGraph = TileGraphFactory.CreateGridTileGraph(rows: 2, columns: 3);
        
        Assert.That(tileGraph.CanTraverse(new TileId("R0C0"), new TileId("R0C1")), Is.True);
        Assert.That(tileGraph.CanTraverse(new TileId("R0C1"), new TileId("R0C2")), Is.True);
    }

    [Test]
    public void CreateGridTileGraph_CanTraverseVertically()
    {
        var tileGraph = TileGraphFactory.CreateGridTileGraph(rows: 3, columns: 2);
        
        Assert.That(tileGraph.CanTraverse(new TileId("R0C0"), new TileId("R1C0")), Is.True);
        Assert.That(tileGraph.CanTraverse(new TileId("R1C0"), new TileId("R2C0")), Is.True);
    }

    [Test]
    public void CreateGridTileGraph_CustomPattern()
    {
        var tileGraph = TileGraphFactory.CreateGridTileGraph(rows: 2, columns: 2, pattern: "tile_{1}_{2}");
        
        Assert.That(tileGraph, Is.Not.Null);
        Assert.That(tileGraph.IsInsideZone(new TileId("tile_0_0")), Is.True);
        Assert.That(tileGraph.IsInsideZone(new TileId("tile_1_1")), Is.True);
    }

    [Test]
    public void CreateGridTileGraph_DistanceCalculation()
    {
        var tileGraph = TileGraphFactory.CreateGridTileGraph(rows: 3, columns: 3);
        
        var distanceHorizontal = tileGraph.Distance(new TileId("R0C0"), new TileId("R0C2"));
        var distanceVertical = tileGraph.Distance(new TileId("R0C0"), new TileId("R2C0"));
        
        Assert.That(distanceHorizontal, Is.EqualTo(2));
        Assert.That(distanceVertical, Is.EqualTo(2));
    }
}

