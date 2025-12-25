namespace TurnForge.Engine.Tests.Strategies.Actions;

using NUnit.Framework;
using TurnForge.Engine.Commands.Move;
using TurnForge.Engine.Components;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Strategies.Actions;
using TurnForge.Engine.Tests.Helpers;
using TurnForge.Engine.ValueObjects;

[TestFixture]
public class BasicMoveStrategyTests
{
    [Test]
    public void Execute_ValidMovement_ReturnsSuccess()
    {
        // Arrange
        var (state, board) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var agentId, ap: 3)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var context = new ActionContext(state, state.Board!);
        var strategy = new BasicMoveStrategy(query);
        var targetPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var command = new MoveCommand(agentId, true, targetPosition);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Decisions, Has.Count.EqualTo(1));
    }
    
    [Test]
    public void Execute_ValidMovement_CostIs1AP()
    {
        // Arrange
        var (state, board) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var agentId, ap: 3)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var context = new ActionContext(state, state.Board!);
        var strategy = new BasicMoveStrategy(query);
        var targetPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var command = new MoveCommand(agentId, true, targetPosition);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.Metadata.ActionPointsCost, Is.EqualTo(1));
    }
    
    [Test]
    public void Execute_MoveToSamePosition_ReturnsFailed()
    {
        // Arrange
        var (state, board) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var agentId, ap: 3)
            .Build();
        
        // Get agent's actual current position from state
        var query = new GameStateQueryService(state, board);
        var agent = query.GetAgent(agentId);
        var currentPosition = agent!.PositionComponent.CurrentPosition;
        
        var context = new ActionContext(state, state.Board!);
        var strategy = new BasicMoveStrategy(query);
        var command = new MoveCommand(agentId, true, currentPosition); // Same position
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ValidationErrors[0], Does.Contain("Already at target position"));
    }
    
    [Test]
    public void Execute_InsufficientAP_ReturnsFailed()
    {
        // Arrange
        var (state, board) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var agentId, ap: 0) // 0 AP
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var context = new ActionContext(state, state.Board!);
        var strategy = new BasicMoveStrategy(query);
        var targetPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var command = new MoveCommand(agentId, true, targetPosition);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ValidationErrors[0], Does.Contain("Insufficient Action Points"));
    }
    
    [Test]
    public void Execute_FreeMovement_DoesNotCostAP()
    {
        // Arrange
        var (state, board) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var agentId, ap: 3)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var context = new ActionContext(state, state.Board!);
        var strategy = new BasicMoveStrategy(query);
        var targetPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var command = new MoveCommand(agentId, false, targetPosition); // HasCost = false
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Metadata.ActionPointsCost, Is.EqualTo(0));
        
        var apComponent = result.Decisions[0].GetComponent<BaseActionPointsComponent>();
        Assert.That(apComponent!.CurrentActionPoints, Is.EqualTo(3)); // Still 3 AP
    }
}
