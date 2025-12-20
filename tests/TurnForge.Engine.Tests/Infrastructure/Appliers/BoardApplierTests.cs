using Moq;
using NUnit.Framework;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Infrastructure.Appliers;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Infrastructure.Appliers;

[TestFixture]
public class BoardApplierTests
{
    private BoardApplier _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new BoardApplier();
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

        // Act
        var resultBoard = _sut.Apply(spatial, zones);

        // Assert
        Assert.That(resultBoard, Is.Not.Null);
        Assert.That(resultBoard.Zones, Has.Count.EqualTo(1));

        var createdZone = resultBoard.Zones.First();
        var behaviourComponent = createdZone.GetComponent<BehaviourComponent>();

        Assert.That(behaviourComponent, Is.Not.Null);
        Assert.That(behaviourComponent.Behaviours, Contains.Item(mockBehaviour.Object));
    }
}
