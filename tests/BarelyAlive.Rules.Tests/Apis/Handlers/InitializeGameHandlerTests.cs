using BarelyAlive.Rules.Apis.Handlers;
using BarelyAlive.Rules.Core.Domain.Projectors;
using Moq;
using NUnit.Framework;
using TurnForge.Engine.Commands; // For CommandResult
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Definitions; // For AgentTypeId
using TurnForge.Engine.Entities.Appliers.Results;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.Orchestrator;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Tests.Apis.Handlers;

[TestFixture]
public class InitializeGameHandlerTests
{
    private Mock<IGameEngine> _mockGameEngine = null!;
    private InitializeGameHandler _handler = null!;
    private DomainProjector _projector = null!;

    [SetUp]
    public void Setup()
    {
        _mockGameEngine = new Mock<IGameEngine>();
        _projector = new DomainProjector(); // Use real projector with strategy
        _handler = new InitializeGameHandler(_mockGameEngine.Object, _projector);
    }

    [Test]
    public void Handle_ValidMissionJson_ReturnsSuccessResponse()
    {
        // Arrange
        var missionJson = @"
        {
            ""missionId"": ""test_mission"",
            ""spatial"": { 
                ""type"": ""DiscreteGraph"", 
                ""nodes"": [ { ""x"": 0, ""y"": 0, ""z"": 0 } ], 
                ""connections"": [] 
            },
            ""zones"": [],
            ""props"": [],
            ""agents"": [ 
                { 
                    ""typeId"": ""Survivor"", 
                    ""maxHealth"": 3, 
                    ""maxBaseMovement"": 3,
                    ""position"": { ""x"": 0, ""y"": 0, ""z"": 0 }
                } 
            ]
        }";

        var effects = Array.Empty<IGameEffect>(); // InitGame only creates Board/Props, assume none for this test or mock PropSpawned if needed

        var mockCommand = new Mock<TurnForge.Engine.Commands.Interfaces.ICommand>().Object;

        var mockTransaction = new CommandTransaction(mockCommand)
        {
            Id = Guid.NewGuid(),
            Result = CommandResult.ACKResult,
            Effects = effects
        };

        _mockGameEngine
            .Setup(e => e.ExecuteCommand(It.IsAny<InitGameCommand>()))
            .Returns(mockTransaction);

        // Act
        var result = _handler.Handle(missionJson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Response.Success, Is.True);
            Assert.That(result.Response.TransactionId, Is.EqualTo(mockTransaction.Id));
            Assert.That(result.Agents, Has.Count.EqualTo(1));
            Assert.That(result.Agents.First().TypeId.Value, Is.EqualTo("Survivor"));
            // Payload should NOT contain agents as they are not spawned yet
            Assert.That(result.Response.Payload.Created, Is.Empty);
        });
    }
}
