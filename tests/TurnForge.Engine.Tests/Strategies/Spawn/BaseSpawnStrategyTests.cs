using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace TurnForge.Engine.Tests.Strategies.Spawn;

/// <summary>
/// Tests for BaseSpawnStrategy using Pattern Matching approach.
/// </summary>
[TestFixture]
public class BaseSpawnStrategyTests
{
    #region Test Descriptors

    public class TestWarriorDescriptor : AgentDescriptor
    {
        public int Strength { get; set; } = 10;
        public string Faction { get; set; } = "Alliance";
        
        public TestWarriorDescriptor(string definitionId) : base(definitionId) { }
    }

    public class TestArcherDescriptor : AgentDescriptor
    {
        public int Accuracy { get; set; } = 80;
        
        public TestArcherDescriptor(string definitionId) : base(definitionId) { }
    }

    #endregion

    #region Test Strategies

    // Strategy demonstrating Type-Specific processing via Pattern Matching
    public class PatternMatchingStrategy : BaseSpawnStrategy
    {
        public int WarriorCount { get; private set; }
        public int ArcherCount { get; private set; }
        public int DefaultCount { get; private set; }

        protected override AgentDescriptor ProcessDescriptor(
            AgentDescriptor descriptor,
            GameState state)
        {
            return descriptor switch
            {
                TestWarriorDescriptor w => ProcessWarrior(w),
                TestArcherDescriptor a => ProcessArcher(a),
                _ => ProcessDefault(descriptor)
            };
        }

        private AgentDescriptor ProcessWarrior(TestWarriorDescriptor w)
        {
            WarriorCount++;
            w.Strength += 5;
            return w;
        }

        private AgentDescriptor ProcessArcher(TestArcherDescriptor a)
        {
            ArcherCount++;
            a.Accuracy += 10;
            return a;
        }

        private AgentDescriptor ProcessDefault(AgentDescriptor d)
        {
            DefaultCount++;
            return d;
        }
    }

    // Strategy demonstrating Batch Processing (overriding execution flow)
    public class BatchProcessingStrategy : BaseSpawnStrategy
    {
        public bool BatchProcessed { get; private set; }

        public override IReadOnlyList<AgentDescriptor> Process(
            IReadOnlyList<AgentDescriptor> descriptors,
            GameState state)
        {
            // Example: Do something with the whole batch first
            BatchProcessed = true;
            
            // Then delegate to standard processing
            return base.Process(descriptors, state);
        }
    }

    #endregion

    [Test]
    public void Process_ShouldDispatchToCorrectTypeHandlers()
    {
        // Arrange
        var strategy = new PatternMatchingStrategy();
        var descriptors = new List<AgentDescriptor>
        {
            new TestWarriorDescriptor("W1"),
            new TestArcherDescriptor("A1"),
            new AgentDescriptor("D1")
        };

        // Act
        var result = strategy.Process(descriptors, null!);

        // Assert
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(strategy.WarriorCount, Is.EqualTo(1));
        Assert.That(strategy.ArcherCount, Is.EqualTo(1));
        Assert.That(strategy.DefaultCount, Is.EqualTo(1));

        // Verify modifications
        var warrior = (TestWarriorDescriptor)result[0];
        Assert.That(warrior.Strength, Is.EqualTo(15)); // 10 + 5

        var archer = (TestArcherDescriptor)result[1];
        Assert.That(archer.Accuracy, Is.EqualTo(90)); // 80 + 10
    }

    [Test]
    public void Process_WithBatchOverride_ShouldExecuteCustomBatchLogic()
    {
        // Arrange
        var strategy = new BatchProcessingStrategy();
        var descriptors = new List<AgentDescriptor> { new AgentDescriptor("D1") };

        // Act
        strategy.Process(descriptors, null!);

        // Assert
        Assert.That(strategy.BatchProcessed, Is.True);
    }
    
    [Test]
    public void ToDecisions_ShouldWrapDescriptors()
    {
        // Arrange
        var strategy = new PatternMatchingStrategy();
        var descriptors = new List<AgentDescriptor> { new AgentDescriptor("D1") };

        // Act
        var decisions = strategy.ToDecisions(descriptors);

        // Assert
        Assert.That(decisions.Count, Is.EqualTo(1));
        Assert.That(decisions[0].Descriptor.DefinitionId, Is.EqualTo("D1"));
    }
}
