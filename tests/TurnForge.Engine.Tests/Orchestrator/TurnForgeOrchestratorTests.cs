using System.Linq;
using Moq;
using NUnit.Framework;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Orchestrator;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;
using TFGameState = TurnForge.Engine.Entities.GameState;

namespace TurnForge.Engine.Tests.Orchestrator;

[TestFixture]
public class TurnForgeOrchestratorTests
{
    private TurnForgeOrchestrator _orchestrator;

    [SetUp]
    public void Setup()
    {
        _orchestrator = new TurnForgeOrchestrator();
    }

    public record TestDecision(DecisionTiming Timing, string Value) : IDecision
    {
        public string OriginId => "TestOrigin";
    }

    [Test]
    public void ApplyImmediate_MutatesStateAndDoesNotPersist()
    {
        // Arrange
        var decision = new TestDecision(
            new DecisionTiming(DecisionTimingWhen.OnCommandExecutionEnd, null, DecisionTimingFrequency.Single),
            "Immediate"
        );
        var mockApplier = new Mock<IApplier<TestDecision>>();
        mockApplier
            .Setup(a => a.Apply(It.IsAny<TestDecision>(), It.IsAny<TFGameState>()))
            .Returns((TestDecision d, TFGameState s) =>
            {
                return new ApplierResponse(s.WithBoard(new GameBoard(Mock.Of<ISpatialModel>())), Array.Empty<IGameEffect>());
            });

        _orchestrator.RegisterApplier(mockApplier.Object);
        _orchestrator.Enqueue(new[] { decision });

        // Act
        _orchestrator.ExecuteScheduled(null, "OnCommandExecutionEnd");

        // Assert
        mockApplier.Verify(a => a.Apply(It.Is<TestDecision>(d => d.Value == "Immediate"), It.IsAny<TFGameState>()), Times.Once);
        Assert.That(_orchestrator.CurrentState.Board, Is.Not.Null);

        // Verify it was removed (Single frequency)
        _orchestrator.ExecuteScheduled(null, "OnCommandExecutionEnd");
        mockApplier.Verify(a => a.Apply(It.IsAny<TestDecision>(), It.IsAny<TFGameState>()), Times.Once);
    }

    [Test]
    public void ApplyDeferredSingle_MutatesOnPhaseAndRemoves()
    {
        // Arrange
        var phaseName = "TestPhase";
        var decision = new TestDecision(
            new DecisionTiming(DecisionTimingWhen.OnStateStart, phaseName, DecisionTimingFrequency.Single),
            "DeferredSingle"
        );
        var mockApplier = new Mock<IApplier<TestDecision>>();
        mockApplier
            .Setup(a => a.Apply(It.IsAny<TestDecision>(), It.IsAny<TFGameState>()))
            .Returns((TestDecision d, TFGameState s) =>
                new ApplierResponse(s.WithBoard(new GameBoard(Mock.Of<ISpatialModel>())), Array.Empty<IGameEffect>()));

        _orchestrator.RegisterApplier(mockApplier.Object);
        _orchestrator.Enqueue(new[] { decision });

        // Act - Trigger wrong phase
        _orchestrator.ExecuteScheduled("WrongPhase", "OnStateStart");
        mockApplier.Verify(a => a.Apply(It.IsAny<TestDecision>(), It.IsAny<TFGameState>()), Times.Never);

        // Act - Trigger correct phase
        _orchestrator.ExecuteScheduled(phaseName, "OnStateStart");
        mockApplier.Verify(a => a.Apply(It.Is<TestDecision>(d => d.Value == "DeferredSingle"), It.IsAny<TFGameState>()), Times.Once);
        Assert.That(_orchestrator.CurrentState.Board, Is.Not.Null);

        // Act - Trigger again (Should be removed)
        _orchestrator.ExecuteScheduled(phaseName, "OnStateStart");
        mockApplier.Verify(a => a.Apply(It.IsAny<TestDecision>(), It.IsAny<TFGameState>()), Times.Once);
    }

    [Test]
    public void ApplyDeferredPermanent_MutatesOnPhaseAndPersists()
    {
        // Arrange
        var phaseName = "TestPhase";
        var decision = new TestDecision(
            new DecisionTiming(DecisionTimingWhen.OnStateEnd, phaseName, DecisionTimingFrequency.Permanent),
            "DeferredPermanent"
        );
        var mockApplier = new Mock<IApplier<TestDecision>>();
        mockApplier
            .Setup(a => a.Apply(It.IsAny<TestDecision>(), It.IsAny<TFGameState>()))
            .Returns((TestDecision d, TFGameState s) =>
                new ApplierResponse(s.WithBoard(new GameBoard(Mock.Of<ISpatialModel>())), Array.Empty<IGameEffect>()));

        _orchestrator.RegisterApplier(mockApplier.Object);
        _orchestrator.Enqueue(new[] { decision });

        // Act - Trigger
        _orchestrator.ExecuteScheduled(phaseName, "OnStateEnd");
        mockApplier.Verify(a => a.Apply(It.Is<TestDecision>(d => d.Value == "DeferredPermanent"), It.IsAny<TFGameState>()), Times.Once);

        // Act - Trigger again (Should NOT be removed)
        _orchestrator.ExecuteScheduled(phaseName, "OnStateEnd");
        mockApplier.Verify(a => a.Apply(It.Is<TestDecision>(d => d.Value == "DeferredPermanent"), It.IsAny<TFGameState>()), Times.Exactly(2));
    }

    [Test]
    public void ValidarEstadoMutaCorrectamente()
    {
        // Arrange
        var decision = new TestDecision(
            new DecisionTiming(DecisionTimingWhen.OnCommandExecutionEnd, null, DecisionTimingFrequency.Single),
            "MutationCheck"
        );

        var customApplier = new TestStateMutatingApplier();

        _orchestrator.RegisterApplier(customApplier);
        _orchestrator.Enqueue(new[] { decision });

        Assert.That(_orchestrator.CurrentState.Board, Is.Null, "Initial state should have null board");

        // Act
        _orchestrator.ExecuteScheduled(null, "OnCommandExecutionEnd");

        // Assert
        Assert.That(_orchestrator.CurrentState.Board, Is.Not.Null, "State should be mutated with new board");
    }

    public class TestStateMutatingApplier : IApplier<TestDecision>
    {
        public ApplierResponse Apply(TestDecision decision, TFGameState state)
        {
            var newState = state.WithBoard(new GameBoard(Mock.Of<ISpatialModel>()));
            return new ApplierResponse(newState, Array.Empty<IGameEffect>());
        }
    }
}
