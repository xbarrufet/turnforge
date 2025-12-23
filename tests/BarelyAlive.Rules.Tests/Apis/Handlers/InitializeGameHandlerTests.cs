using BarelyAlive.Rules.Apis.Handlers;
using BarelyAlive.Rules.Core.Domain.Projectors;
using Moq;
using NUnit.Framework;
using TurnForge.Engine.Commands; // For CommandResult
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities; // For AgentTypeId equivalent
using TurnForge.Engine.Commands.Board;
using TurnForge.Engine.Appliers.Entity.Results;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.Core.Orchestrator;
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
                    ""agentName"": ""Survivor"", 
                    ""category"": ""Survivor"",
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
            .Setup(e => e.ExecuteCommand(It.IsAny<InitializeBoardCommand>()))
            .Returns(mockTransaction);
        
        _mockGameEngine
            .Setup(e => e.ExecuteCommand(It.IsAny<SpawnPropsCommand>()))
            .Returns(mockTransaction);

        // Act
        var result = _handler.Handle(missionJson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Response.Success, Is.True);
            Assert.That(result.Response.TransactionId, Is.EqualTo(mockTransaction.Id));
            Assert.That(result.Agents, Has.Count.EqualTo(1));
            Assert.That(result.Agents.First().DefinitionId, Is.EqualTo("Survivor"));
            // Payload should NOT contain agents as they are not spawned yet
            Assert.That(result.Response.Payload.Created, Is.Empty);
        });
    }
}
