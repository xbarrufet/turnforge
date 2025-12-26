namespace BarelyAlive.Rules.Tests.Strategies.Actions;

using NUnit.Framework;
using TurnForge.Engine.Commands.Attack;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Strategies.Actions;
using BarelyAlive.Rules.Tests.Helpers;
using TurnForge.Engine.ValueObjects;
using BarelyAlive.Rules.Core.Domain.Strategies.Actions;
using TurnForge.Engine.Services.Dice;
using Moq;

[TestFixture]
public class BasicMeleeAttackStrategyTests
{
    [Test]
    public void Execute_AttackerNotFound_ReturnsFailed()
    {
        // Arrange
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var dice = new DiceThrowService();
        var strategy = new BasicMeleeAttackStrategy(query, dice);
        var command = new AttackCommand("nonexistent", "target-1");
        var context = new ActionContext(state, board);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ValidationErrors[0], Does.Contain("Attacker not found"));
    }
    
    [Test]
    public void Execute_TargetNotFound_ReturnsFailed()
    {
        // Arrange
        var attackerPos = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var survivorId, category: "Survivor", ap: 3, position: attackerPos)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var dice = new DiceThrowService();
        var strategy = new BasicMeleeAttackStrategy(query, dice);
        var command = new AttackCommand(survivorId, "nonexistent");
        var context = new ActionContext(state, board);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ValidationErrors[0], Does.Contain("Target not found"));
    }
    
    [Test]
    public void Execute_TargetOutOfRange_ReturnsFailed()
    {
        // Arrange - Using different positions
        var attackerPos = Position.FromTile(new TileId(Guid.NewGuid()));
        var targetPos = Position.FromTile(new TileId(Guid.NewGuid())); // Different position
        
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard(mock => {
                mock.Setup(m => m.IsValidPosition(It.IsAny<Position>())).Returns(true);
                mock.Setup(m => m.Distance(It.IsAny<Position>(), It.IsAny<Position>())).Returns(10); // Far away
            })
            .WithAgent("survivor", out var survivorId, category: "Survivor", ap: 3, position: attackerPos)
            .WithAgent("zombie", out var zombieId, category: "Zombie", position: targetPos)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var dice = new DiceThrowService();
        var strategy = new BasicMeleeAttackStrategy(query, dice);
        var command = new AttackCommand(survivorId, zombieId);
        var context = new ActionContext(state, board);
        
        // Act
        var result = strategy.Execute(command, context);
        
        // Assert - should fail due to range check (board.Distance returns > 1 for unconnected tiles)
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ValidationErrors[0], Does.Contain("out of melee range"));
    }
}
