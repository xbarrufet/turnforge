using Moq;
using NUnit.Framework;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Infrastructure.Appliers;

[TestFixture]
public class BoardApplierTests
{
    private BoardApplier _sut;
    private Mock<TurnForge.Engine.Entities.Factories.Interfaces.IGameEntityFactory<GameBoard>> _factoryMock;

    [SetUp]
    public void Setup()
    {
        _sut = new BoardApplier();
        _factoryMock = new Mock<TurnForge.Engine.Entities.Factories.Interfaces.IGameEntityFactory<GameBoard>>();
        _factoryMock.Setup(f => f.Build(It.IsAny<IGameEntityDescriptor<GameBoard>>()))
                    .Returns(new GameBoard(new TurnForge.Engine.Spatial.ConnectedGraphSpatialModel(new TurnForge.Engine.Spatial.MutableTileGraph(new HashSet<TileId>()))));
    }

    [Test]
    public void Apply_ShouldCreateBoardWithCorrectZones_AndAttachBehaviours()
    {
        // Arrange
        var spatial = new DiscreteSpatialDescriptor(
            Nodes: new List<TileId> { TileId.New() },
            Connections: new List<DiscreteConnectionDeacriptor>()
        );

        var zoneId = "TestZone";
        var mockBehaviour = new Mock<TurnForge.Engine.Entities.Board.ZoneBehaviour>();
        var behaviours = new List<TurnForge.Engine.Entities.Board.Interfaces.IZoneBehaviour> { mockBehaviour.Object };

        var zoneDescriptor = new ZoneDescriptor(
            Id: new ZoneId(zoneId),
            Bound: Mock.Of<IZoneBound>(),
            Behaviours: behaviours
        );

        var zones = new List<ZoneDescriptor> { zoneDescriptor };

        var boardDescriptor = new BoardDescriptor(spatial, zones);

        // Act
        var resultBoard = _sut.Build(boardDescriptor, _factoryMock.Object);

        // Assert
        Assert.That(resultBoard, Is.Not.Null);
        // Assert.That(resultBoard.Zones, Has.Count.EqualTo(1)); // BoardApplier adds zones to the board returned by factory.
        // The mock factory returns a board. BoardApplier adds zones to it.
        // Since mock returns a fresh board, it should have zones added by applier if applier does that.
        // Let's verify applier logic: it iterates zones and calls board.AddZone(zone).
        Assert.That(resultBoard.Zones.Count, Is.EqualTo(1));

        var createdZone = resultBoard.Zones.First();
        var behaviourComponent = createdZone.GetComponent<BehaviourComponent>();

        Assert.That(behaviourComponent, Is.Not.Null);
        Assert.That(behaviourComponent.Behaviours, Contains.Item(mockBehaviour.Object));
    }
}
