namespace BarelyAlive.Rules.Tests.Strategies.Actions;

using NUnit.Framework;
using TurnForge.Engine.Commands.Move;
using TurnForge.Engine.Components;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Strategies.Actions;
using BarelyAlive.Rules.Tests.Helpers;
using TurnForge.Engine.ValueObjects;
using BarelyAlive.Rules.Core.Domain.Strategies.Actions;

[TestFixture]
public class BarelyAliveMovementStrategyTests
{
    [Test]
    public void Execute_Survivor_WithNoZombies_Costs1AP()
    {
        // Arrange
        var startPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var survivorId, category: "Survivor", team: "Survivors", ap: 3, position: startPosition)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var context = new ActionContext(state, state.Board!);
        var strategy = new BarelyAliveMovementStrategy(query);
        var targetPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var command = new MoveCommand(survivorId, true, targetPosition);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Metadata.ActionPointsCost, Is.EqualTo(1), "No zombies = 1 AP cost");
    }
    
    [Test]
    public void Execute_Survivor_With2Zombies_Costs3AP()
    {
        // Arrange - IMPORTANT: Use same Position instance for all agents
        var sharedPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var survivorId, category: "Survivor", team: "Survivors", ap: 5, position: sharedPosition)
            .WithAgent("zombie1", out var _, category: "Zombie", team: "Zombies", position: sharedPosition)
            .WithAgent("zombie2", out var _, category: "Zombie", team: "Zombies", position: sharedPosition)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var context = new ActionContext(state, state.Board!);
        var strategy = new BarelyAliveMovementStrategy(query);
        var targetPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var command = new MoveCommand(survivorId, true, targetPosition);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Metadata.ActionPointsCost, Is.EqualTo(3), "2 zombies = 1 + 2 = 3 AP cost");
    }
    
    [Test]
    public void Execute_Zombie_WithOtherAgents_Costs1AP()
    {
        // Arrange
        var startPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .WithAgent("zombie", out var zombieId, category: "Zombie", team: "Zombies", ap: 3, position: startPosition)
            .WithAgent("survivor", out var _, category: "Survivor", team: "Survivors", position: startPosition)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var context = new ActionContext(state, state.Board!);
        var strategy = new BarelyAliveMovementStrategy(query);
        var targetPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var command = new MoveCommand(zombieId, true, targetPosition);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Metadata.ActionPointsCost, Is.EqualTo(1), "Zombies always pay fixed 1 AP");
    }
    
    [Test]
    public void Execute_Survivor_InsufficientAP_WithZombies_ReturnsFailed()
    {
        // Arrange - IMPORTANT: Use same Position instance
        var sharedPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var survivorId, category: "Survivor", team: "Survivors", ap: 2, position: sharedPosition) // Only 2 AP
            .WithAgent("zombie1", out var _, category: "Zombie", team: "Zombies", position: sharedPosition)
            .WithAgent("zombie2", out var _, category: "Zombie", team: "Zombies", position: sharedPosition)
            .Build(); // Would need 3 AP (1 + 2 zombies)
        
        var query = new GameStateQueryService(state, board);
        var context = new ActionContext(state, state.Board!);
        var strategy = new BarelyAliveMovementStrategy(query);
        var targetPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var command = new MoveCommand(survivorId, true, targetPosition);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ValidationErrors[0], Does.Contain("Insufficient Action Points"));
    }
    
    [Test]
    public void Execute_Survivor_FreeMovement_IgnoresZombies()
    {
        // Arrange
        var startPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var survivorId, category: "Survivor", team: "Survivors", ap: 1, position: startPosition)
            .WithAgent("zombie", out var _, category: "Zombie", team: "Zombies", position: startPosition)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var context = new ActionContext(state, state.Board!);
        var strategy = new BarelyAliveMovementStrategy(query);
        var targetPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var command = new MoveCommand(survivorId, false, targetPosition); // HasCost = false
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Metadata.ActionPointsCost, Is.EqualTo(0), "Free movement = 0 cost");
    }
}
