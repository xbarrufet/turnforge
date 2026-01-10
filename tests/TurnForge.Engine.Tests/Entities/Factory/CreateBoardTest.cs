using TurnForge.Engine.Core.Exceptions;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Discrete;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.Builders;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Entities.Factory;


[TestFixture]
public class CreateBoardTest
{

    // OPTION B: Use the real in-memory registry (simple and reliable)
    private InMemoryGameCatalog _realRegistry = null!;
    private readonly ZoneId zoneABDefinitionId = new ZoneId("zoneABDefinitionId");
    private readonly ZoneId zoneCDDefinitionId = new ZoneId("zoneCDDefinitionId");
    private GenericEntityFactory _entityFactory;
    private BoardFactory _boardFactory;
    private BoardDescriptorBuilder _boardDescriptorBuilder;

    [SetUp]
    public void SetUp()
    {
        _realRegistry = new InMemoryGameCatalog();
        _entityFactory = new GenericEntityFactory(_realRegistry);
        _boardFactory = new BoardFactory(_entityFactory);
        _boardDescriptorBuilder = new BoardDescriptorBuilder();
    }

    [Test]
    public void Create_Board_With_Zones_And_Connections_ok()
    {
        var descriptor = _boardDescriptorBuilder.WithKind(TopologyKind.Discrete)
            .WithZone(zoneABDefinitionId, zoneBuilder =>
            {
                zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
            })
            .WithZone(zoneCDDefinitionId, zoneBuilder =>
            {
                zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
            })
            .WithConnection(connection => connection
                .From(zoneABDefinitionId)
                .To(zoneCDDefinitionId)
                .WithDiscreteConnectionPoint(posBuilder => posBuilder
                    .AddConnection(new TileId("tile_1_1"), new TileId("tile_1_0"))
                ))
            .Build();

        Assert.That(descriptor.Connections, Has.Count.EqualTo(1));
    }

    [Test]
    public void Create_Board_With_NonExistent_From_Zone_Throws()
    {
        var nonExistentZoneId = new ZoneId("nonExistentZone");

        var ex = Assert.Throws<InvalidDescriptorException>(() =>
        {
            _boardDescriptorBuilder.WithKind(TopologyKind.Discrete)
                .WithZone(zoneCDDefinitionId, zoneBuilder =>
                {
                    zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
                })
                .WithConnection(connection => connection
                    .From(nonExistentZoneId) // Zone doesn't exist
                    .To(zoneCDDefinitionId)
                    .WithDiscreteConnectionPoint(posBuilder => posBuilder
                        .AddConnection(new TileId("tile_0_0"), new TileId("tile_0_0"))
                    ))
                .Build();
        });

        Assert.That(ex.Message, Does.Contain("Connection 'From' zone"));
        Assert.That(ex.Message, Does.Contain(nonExistentZoneId.Value));
        Assert.That(ex.Message, Does.Contain("does not exist"));
    }

    [Test]
    public void Create_Board_With_NonExistent_To_Zone_Throws()
    {
        var nonExistentZoneId = new ZoneId("nonExistentZone");

        var ex = Assert.Throws<InvalidDescriptorException>(() =>
        {
            _boardDescriptorBuilder.WithKind(TopologyKind.Discrete)
                .WithZone(zoneABDefinitionId, zoneBuilder =>
                {
                    zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
                })
                .WithConnection(connection => connection
                    .From(zoneABDefinitionId)
                    .To(nonExistentZoneId) // Zone doesn't exist
                    .WithDiscreteConnectionPoint(posBuilder => posBuilder
                        .AddConnection(new TileId("tile_0_0"), new TileId("tile_0_0"))
                    ))
                .Build();
        });

        Assert.That(ex.Message, Does.Contain("Connection 'To' zone"));
        Assert.That(ex.Message, Does.Contain(nonExistentZoneId.Value));
        Assert.That(ex.Message, Does.Contain("does not exist"));
    }

    [Test]
    public void Create_Board_With_Connection_Point_Outside_From_Zone_Throws()
    {
        var invalidTileId = new TileId("tile_99_99"); // This tile doesn't exist in a 2x2 grid

        var ex = Assert.Throws<InvalidDescriptorException>(() =>
        {
            _boardDescriptorBuilder.WithKind(TopologyKind.Discrete)
                .WithZone(zoneABDefinitionId, zoneBuilder =>
                {
                    zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
                })
                .WithZone(zoneCDDefinitionId, zoneBuilder =>
                {
                    zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
                })
                .WithConnection(connection => connection
                    .From(zoneABDefinitionId)
                    .To(zoneCDDefinitionId)
                    .WithDiscreteConnectionPoint(posBuilder => posBuilder
                        .AddConnection(invalidTileId, new TileId("tile_0_0")) // From position is invalid
                    ))
                .Build();
        });

        Assert.That(ex.Message, Does.Contain("Connection 'From' position"));
        Assert.That(ex.Message, Does.Contain(invalidTileId.Value));
        Assert.That(ex.Message, Does.Contain("is not inside zone"));
        Assert.That(ex.Message, Does.Contain(zoneABDefinitionId.Value));
    }

    [Test]
    public void Create_Board_With_Connection_Point_Outside_To_Zone_Throws()
    {
        var invalidTileId = new TileId("tile_99_99"); // This tile doesn't exist in a 2x2 grid

        var ex = Assert.Throws<InvalidDescriptorException>(() =>
        {
            _boardDescriptorBuilder.WithKind(TopologyKind.Discrete)
                .WithZone(zoneABDefinitionId, zoneBuilder =>
                {
                    zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
                })
                .WithZone(zoneCDDefinitionId, zoneBuilder =>
                {
                    zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
                })
                .WithConnection(connection => connection
                    .From(zoneABDefinitionId)
                    .To(zoneCDDefinitionId)
                    .WithDiscreteConnectionPoint(posBuilder => posBuilder
                        .AddConnection(new TileId("tile_0_0"), invalidTileId) // To position is invalid
                    ))
                .Build();
        });

        Assert.That(ex.Message, Does.Contain("Connection 'To' position"));
        Assert.That(ex.Message, Does.Contain(invalidTileId.Value));
        Assert.That(ex.Message, Does.Contain("is not inside zone"));
        Assert.That(ex.Message, Does.Contain(zoneCDDefinitionId.Value));
    }

    [Test]
    public void Create_Board_With_Multiple_Connection_Points_All_Valid()
    {
        var descriptor = _boardDescriptorBuilder.WithKind(TopologyKind.Discrete)
            .WithZone(zoneABDefinitionId, zoneBuilder =>
            {
                zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
            })
            .WithZone(zoneCDDefinitionId, zoneBuilder =>
            {
                zoneBuilder.WithZoneTopology(TileGraphFactory.CreateGridTileGraph(2, 2, "tile_{1}_{2}"));
            })
            .WithConnection(connection => connection
                .From(zoneABDefinitionId)
                .To(zoneCDDefinitionId)
                .WithDiscreteConnectionPoint(posBuilder => posBuilder
                    .AddConnection(new TileId("tile_0_0"), new TileId("tile_0_0"))
                    .AddConnection(new TileId("tile_1_1"), new TileId("tile_1_1"))
                ))
            .Build();

        Assert.That(descriptor.Connections, Has.Count.EqualTo(1));
        Assert.That(descriptor.Connections[0].ConnectionPosition.NumberOfConnections, Is.EqualTo(2));
    }

}