using BarelyAlive.Rules.Core.Domain.Strategies.Pipelines;
using BarelyAlive.Rules.Tests.Helpers;
using Moq;
using NUnit.Framework;
using TurnForge.Engine.Commands.Attack;
using TurnForge.Engine.Services.Dice;
using TurnForge.Engine.Services.Dice.ValueObjects;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Strategies.Actions;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Tests.Strategies.Pipelines;

[TestFixture]
public class InteractiveCombatPipelineTests
{
    [Test]
    public void Execute_FirstRun_SuspendForDiceRoll()
    {
        // Arrange
        var sharedPos = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .WithAgent("attacker", out var attackerId, category: "Survivor", team: "Survivors", ap: 3, position: sharedPos)
            .WithAgent("target", out var targetId, category: "Zombie", team: "Zombies", position: sharedPos)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var diceMock = new Mock<IDiceThrowService>();
        var pipeline = new InteractiveCombatPipeline(query, diceMock.Object);
        
        var command = new AttackCommand(attackerId, targetId);
        var context = new ActionContext(state, board, attackerId);
        
        // Act
        var result = pipeline.Execute(command, context);
        
        // Assert
        Assert.That(result.IsSuspended, Is.True, "Should suspend for dice roll");
        Assert.That(result.Interaction, Is.Not.Null);
        Assert.That(result.Interaction!.Type, Is.EqualTo("DiceRoll"));
        Assert.That(result.Interaction.Prompt, Does.Contain("Roll to hit"));
        Assert.That(context.CurrentNodeId, Is.EqualTo("RequestHitRoll"));
    }
    
    [Test]
    public void Execute_AfterUserRollHit_AppliesDamage()
    {
        // Arrange
        var sharedPos = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .WithAgent("attacker", out var attackerId, category: "Survivor", team: "Survivors", ap: 3, position: sharedPos)
            .WithAgent("target", out var targetId, category: "Zombie", team: "Zombies", position: sharedPos)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var diceMock = new Mock<IDiceThrowService>();
        
        // Setup mock to return a passing roll
        diceMock.Setup(d => d.Roll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(new DiceRollResult 
            { 
                Total = 5, 
                Limit = new DiceThrowLimit { Threshold = 4 } 
            });
        
        var pipeline = new InteractiveCombatPipeline(query, diceMock.Object);
        var command = new AttackCommand(attackerId, targetId);
        var context = new ActionContext(state, board, attackerId);
        
        // First run - suspend
        var result1 = pipeline.Execute(command, context);
        Assert.That(result1.IsSuspended, Is.True);
        
        // Simulate user rolled a 5 (hit!)
        context.SetVariable("UserRoll", 5);
        context.CurrentNodeId = "EvaluateHit"; // Resume from after suspension
        
        // Act - Resume
        var result2 = pipeline.Execute(command, context);
        
        // Assert
        Assert.That(result2.IsValid, Is.True, "Should complete after hit");
        Assert.That(result2.Decisions, Is.Not.Empty, "Should have damage decisions");
    }
    
    [Test]
    public void Execute_AfterUserRollMiss_EndsWithoutDamage()
    {
        // Arrange
        var sharedPos = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, board) = new LocalTestGameBuilder()
            .WithBoard()
            .WithAgent("attacker", out var attackerId, category: "Survivor", team: "Survivors", ap: 3, position: sharedPos)
            .WithAgent("target", out var targetId, category: "Zombie", team: "Zombies", position: sharedPos)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        var diceMock = new Mock<IDiceThrowService>();
        
        var pipeline = new InteractiveCombatPipeline(query, diceMock.Object);
        var command = new AttackCommand(attackerId, targetId);
        var context = new ActionContext(state, board, attackerId);
        
        // First run - suspend
        pipeline.Execute(command, context);
        
        // Simulate user rolled a 2 (miss!)
        context.SetVariable("UserRoll", 2);
        context.CurrentNodeId = "EvaluateHit";
        
        // Act - Resume
        var result = pipeline.Execute(command, context);
        
        // Assert - Pipeline ends but without decisions (miss)
        Assert.That(result.IsValid, Is.False, "Miss should not be valid (no decisions)");
    }
}
