
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Decisions;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Orchestrator;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;
using TFGameState = TurnForge.Engine.Entities.GameState;

namespace TurnForge.Engine.Tests.Infrastructure.Appliers
{
    [TestFixture]
    public class BoardApplierTests
    {
        private BoardApplier _sut;
        private Mock<IGameEntityFactory<GameBoard>> _factoryMock;

        [SetUp]
        public void Setup()
        {
            _factoryMock = new Mock<IGameEntityFactory<GameBoard>>();

            // Setup Factory to return a dummy board
            _factoryMock.Setup(f => f.Build(It.IsAny<IGameEntityDescriptor<GameBoard>>()))
                        .Returns(new GameBoard(new TurnForge.Engine.Spatial.ConnectedGraphSpatialModel(new TurnForge.Engine.Spatial.MutableTileGraph(new HashSet<TileId>()))));

            _sut = new BoardApplier(_factoryMock.Object);
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
            var mockBehaviour = new Mock<BaseBehaviour>();
            var behaviours = new List<IZoneBehaviour> { mockBehaviour.As<IZoneBehaviour>().Object };

            var zoneDescriptor = new ZoneDescriptor(
                Id: new ZoneId(zoneId),
                Bound: Mock.Of<IZoneBound>(),
                Behaviours: behaviours
            );

            var zones = new List<ZoneDescriptor> { zoneDescriptor };

            var boardDescriptor = new BoardDescriptor(spatial, zones);

            var decision = new BoardDecision(boardDescriptor)
            {
                Timing = DecisionTiming.Immediate
            };

            // Act
            var initialState = TFGameState.Empty();
            var finalState = _sut.Apply(decision, initialState);

            // Assert
            var resultBoard = finalState.GameState.Board;
            Assert.That(resultBoard, Is.Not.Null);

            // Verify zones added
            Assert.That(resultBoard.Zones.Count, Is.EqualTo(1));

            var createdZone = resultBoard.Zones.First();
            var behaviourComponent = createdZone.GetComponent<BehaviourComponent>();

            Assert.That(behaviourComponent, Is.Not.Null);
            Assert.That(behaviourComponent.Behaviours, Contains.Item(mockBehaviour.Object));
        }
    }
}
