using Moq;
using NUnit.Framework;
using TurnForge.Engine.Commands.Actions;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Strategies.Actions;
using TurnForge.Engine.Strategies.Interactions;
using TurnForge.Engine.Strategies.Pipelines;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Tests.Commands.Actions;

public record TestActionCommand(string AgentId) : IActionCommand
{
    public bool HasCost => false;
    public Type CommandType => typeof(TestActionCommand);
}

[TestFixture]
public class ActionCommandHandlerTests
{
    private Mock<IActionStrategy<TestActionCommand>> _strategyMock;
    private Mock<IGameRepository> _repositoryMock;
    private Mock<IGameStateQuery> _queryMock;
    private InteractionRegistry _interactionRegistry;
    private ActionCommandHandler<TestActionCommand> _handler;

    [SetUp]
    public void Setup()
    {
        _strategyMock = new Mock<IActionStrategy<TestActionCommand>>();
        _repositoryMock = new Mock<IGameRepository>();
        _queryMock = new Mock<IGameStateQuery>();
        _interactionRegistry = new InteractionRegistry();

        _handler = new ActionCommandHandler<TestActionCommand>(
            _strategyMock.Object,
            _queryMock.Object,
            _repositoryMock.Object,
            _interactionRegistry
        );

        // Default mock setup
        var gameState = GameState.Empty().WithBoard(new GameBoard(new Mock<TurnForge.Engine.Spatial.Interfaces.ISpatialModel>().Object));
        _repositoryMock.Setup(r => r.LoadGameState()).Returns(gameState);
    }

    [Test]
    public void Handle_WhenStrategySuspended_ReturnsSuspendedResult()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var request = new InteractionRequest 
        { 
            SessionId = sessionId,
            Type = "Test",
            Prompt = "Test Prompt"
        };
        var strategyResult = StrategyResult.Suspended(request);

        _strategyMock.Setup(s => s.Execute(It.IsAny<TestActionCommand>(), It.IsAny<ActionContext>()))
            .Returns(strategyResult);

        var command = new TestActionCommand("agent-1");

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.IsSuspended, Is.True);
        Assert.That(result.Interaction, Is.Not.Null);
        Assert.That(result.Interaction.Type, Is.EqualTo("Test"));
        Assert.That(result.Tags, Contains.Item("SUSPENDED"));

        // Verify registry using the SessionId from the RESULT (which matches the context)
        var registeredContext = _interactionRegistry.Get(result.Interaction.SessionId);
        Assert.That(registeredContext, Is.Not.Null);
    }

    [Test]
    public void Handle_WhenStrategyCompleted_ReturnsOkResult()
    {
        // Arrange
        var strategyResult = StrategyResult.Completed(Array.Empty<TurnForge.Engine.Decisions.Actions.ActionDecision>());

        _strategyMock.Setup(s => s.Execute(It.IsAny<TestActionCommand>(), It.IsAny<ActionContext>()))
            .Returns(strategyResult);

        var command = new TestActionCommand("agent-1");

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.IsSuspended, Is.False);
        Assert.That(result.Interaction, Is.Null);
    }
    
    [Test]
    public void Handle_WhenStrategyFailed_ReturnsFailResult()
    {
        // Arrange
        var strategyResult = StrategyResult.Failed("Some error");

        _strategyMock.Setup(s => s.Execute(It.IsAny<TestActionCommand>(), It.IsAny<ActionContext>()))
            .Returns(strategyResult);

        var command = new TestActionCommand("agent-1");

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo("Some error"));
    }
}
