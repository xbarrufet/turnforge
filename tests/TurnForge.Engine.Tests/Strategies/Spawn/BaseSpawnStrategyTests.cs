using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace TurnForge.Engine.Tests.Strategies.Spawn;

/// <summary>
/// Tests for BaseSpawnStrategy hierarchical method resolution.
/// Verifies 3-level processing: ProcessBatch → ProcessSingle(typed) → ProcessSingle(generic)
/// </summary>
[TestFixture]
public class BaseSpawnStrategyTests
{
    #region Test Descriptors

    // Custom descriptor for type-specific testing
    public class TestWarriorDescriptor : AgentDescriptor
    {
        public int Strength { get; set; } = 10;
        public string Faction { get; set; } = "Alliance";
        
        public TestWarriorDescriptor(string definitionId) : base(definitionId) { }
    }

    // Another custom descriptor for batch testing
    public class TestArcherDescriptor : AgentDescriptor
    {
        public int Accuracy { get; set; } = 80;
        
        public TestArcherDescriptor(string definitionId) : base(definitionId) { }
    }

    #endregion

    #region Test Strategies

    // Strategy with Level 1 (ProcessBatch) implementation
    public class StrategyWithBatchProcessing : BaseSpawnStrategy
    {
        public int BatchCallCount { get; private set; } = 0;

        protected IReadOnlyList<TestArcherDescriptor> ProcessBatch(
            IReadOnlyList<TestArcherDescriptor> descriptors,
            GameState state)
        {
            BatchCallCount++;
            
            // Mark as processed by increasing accuracy
            for (int i = 0; i < descriptors.Count; i++)
            {
                descriptors[i].Accuracy = 80 + (i * 5);
            }
            
            return descriptors;
        }
    }

    // Strategy with Level 2 (ProcessSingle typed) implementation
    public class StrategyWithTypedProcessing : BaseSpawnStrategy
    {
        public int WarriorCallCount { get; private set; } = 0;

        protected TestWarriorDescriptor ProcessSingle(
            TestWarriorDescriptor descriptor,
            GameState state)
        {
            WarriorCallCount++;
            
            descriptor.Strength += 10; // Boost strength
            descriptor.Faction = "Horde";
            
            return descriptor;
        }
    }

    // Strategy with both Level 1 and Level 2 implementations
    public class StrategyWithMixedProcessing : BaseSpawnStrategy
    {
        public int BatchCallCount { get; private set; } = 0;
        public int WarriorCallCount { get; private set; } = 0;
        public int GenericCallCount { get; private set; } = 0;

        protected IReadOnlyList<TestArcherDescriptor> ProcessBatch(
            IReadOnlyList<TestArcherDescriptor> descriptors,
            GameState state)
        {
            BatchCallCount++;
            
            foreach (var desc in descriptors)
            {
                desc.Accuracy = 100; // Mark as batch-processed
            }
            
            return descriptors;
        }

        protected TestWarriorDescriptor ProcessSingle(
            TestWarriorDescriptor descriptor,
            GameState state)
        {
            WarriorCallCount++;
            
            descriptor.Strength = 50; // Mark as individually processed
            
            return descriptor;
        }

        // Override to track fallback calls
        protected override AgentDescriptor ProcessDescriptor(
            AgentDescriptor descriptor,
            GameState state)
        {
            GenericCallCount++;
            return base.ProcessDescriptor(descriptor, state);
        } 
    }

    // Strategy with no custom processing (only Level 3 fallback)
    public class StrategyWithOnlyFallback : BaseSpawnStrategy
    {
        public int FallbackCallCount { get; private set; } = 0;

        protected override AgentDescriptor ProcessDescriptor(
            AgentDescriptor descriptor,
            GameState state)
        {
            FallbackCallCount++;
            return base.ProcessDescriptor(descriptor, state);
        }
    }

    #endregion

    #region Level 1: ProcessBatch Tests

    [Test]
    public void ProcessBatch_WhenImplemented_ShouldProcessAllDescriptorsOfSameType()
    {
        // Arrange
        var strategy = new StrategyWithBatchProcessing();
        var archers = new List<AgentDescriptor>
        {
            new TestArcherDescriptor("Archer1"),
            new TestArcherDescriptor("Archer2"),
            new TestArcherDescriptor("Archer3")
        };

        // Act
        var result = strategy.Process(archers, null!);

        // Assert
        Assert.That(strategy.BatchCallCount, Is.EqualTo(1), "ProcessBatch should be called once for batch");
        Assert.That(result.Count, Is.EqualTo(3));
        
        // Verify each archer was processed
        for (int i = 0; i < result.Count; i++)
        {
            var archer = (TestArcherDescriptor)result[i];
            Assert.That(archer.Accuracy, Is.EqualTo(80 + (i * 5)));
        }
    }

    [Test]
    public void ProcessBatch_WithMultipleBatches_ShouldCallOncePerType()
    {
        // Arrange
        var strategy = new StrategyWithMixedProcessing();
        var descriptors = new List<AgentDescriptor>
        {
            new TestArcherDescriptor("Archer1"),
            new TestArcherDescriptor("Archer2"),
            new TestWarriorDescriptor("Warrior1"),
            new TestArcherDescriptor("Archer3"),
            new TestWarriorDescriptor("Warrior2")
        };

        // Act
        var result = strategy.Process(descriptors, null!);

        // Assert
        Assert.That(strategy.BatchCallCount, Is.EqualTo(1), "ProcessBatch should be called once for archers");
        Assert.That(strategy.WarriorCallCount, Is.EqualTo(2), "ProcessSingle should be called twice for warriors");
        Assert.That(result.Count, Is.EqualTo(5));
    }

    #endregion

    #region Level 2: ProcessSingle (Typed) Tests

    [Test]
    public void ProcessSingle_WhenImplemented_ShouldProcessEachDescriptorIndividually()
    {
        // Arrange
        var strategy = new StrategyWithTypedProcessing();
        var warriors = new List<AgentDescriptor>
        {
            new TestWarriorDescriptor("Warrior1") { Strength = 10 },
            new TestWarriorDescriptor("Warrior2") { Strength = 15 }
        };

        // Act
        var result = strategy.Process(warriors, null!);

        // Assert
        Assert.That(strategy.WarriorCallCount, Is.EqualTo(2), "ProcessSingle should be called for each warrior");
        Assert.That(result.Count, Is.EqualTo(2));
        
        foreach (var warrior in result.Cast<TestWarriorDescriptor>())
        {
            Assert.That(warrior.Faction, Is.EqualTo("Horde"));
            Assert.That(warrior.Strength, Is.GreaterThan(10), "Strength should be boosted");
        }
    }

    #endregion

    #region Level 3: ProcessSingle (Generic Fallback) Tests

    [Test]
    public void ProcessSingle_WhenNoCustomImplementation_ShouldUseFallback()
    {
        // Arrange
        var strategy = new StrategyWithOnlyFallback();
        var genericDescriptors = new List<AgentDescriptor>
        {
            new AgentDescriptor("Agent1"),
            new AgentDescriptor("Agent2")
        };

        // Act
        var result = strategy.Process(genericDescriptors, null!);

        // Assert
        Assert.That(strategy.FallbackCallCount, Is.EqualTo(2), "Fallback should be called for each descriptor");
        Assert.That(result.Count, Is.EqualTo(2));
        
        // Fallback accepts descriptor as-is
        Assert.That(result[0].DefinitionID, Is.EqualTo("Agent1"));
        Assert.That(result[1].DefinitionID, Is.EqualTo("Agent2"));
    }

    [Test]
    public void ProcessSingle_WithMixedTypes_ShouldUseCorrectProcessingLevel()
    {
        // Arrange
        var strategy = new StrategyWithMixedProcessing();
        var descriptors = new List<AgentDescriptor>
        {
            new TestArcherDescriptor("Archer1"),     // Level 1: Batch
            new TestWarriorDescriptor("Warrior1"),   // Level 2: Typed
            new AgentDescriptor("Generic1")          // Level 3: Fallback
        };

        // Act
        var result = strategy.Process(descriptors, null!);

        // Assert
        Assert.That(strategy.BatchCallCount, Is.EqualTo(1), "Batch for archers");
        Assert.That(strategy.WarriorCallCount, Is.EqualTo(1), "Typed for warriors");
        Assert.That(strategy.GenericCallCount, Is.EqualTo(1), "Fallback for generic");
        Assert.That(result.Count, Is.EqualTo(3));
        
        // Verify correct processing per type
        var archer = (TestArcherDescriptor)result[0];
        var warrior = (TestWarriorDescriptor)result[1];
        
        Assert.That(archer.Accuracy, Is.EqualTo(100), "Archer batch-processed");
        Assert.That(warrior.Strength, Is.EqualTo(50), "Warrior individually-processed");
    }

    #endregion

    #region Priority/Precedence Tests

    [Test]
    public void MethodResolution_ShouldPreferBatchOverSingle()
    {
        // Arrange
        var strategy = new StrategyWithBatchProcessing();
        var archers = new List<AgentDescriptor>
        {
            new TestArcherDescriptor("Archer1")
        };

        // Act
        var result = strategy.Process(archers, null!);

        // Assert
        Assert.That(strategy.BatchCallCount, Is.EqualTo(1), "Batch should be preferred even for single item");
    }

    #endregion

    #region ToDecisions Tests

    [Test]
    public void ToDecisions_ShouldWrapDescriptorsInDecisions()
    {
        // Arrange
        var strategy = new StrategyWithOnlyFallback();
        var descriptors = new List<AgentDescriptor>
        {
            new TestWarriorDescriptor("Warrior1"),
            new TestArcherDescriptor("Archer1")
        };

        // Act
        var decisions = strategy.ToDecisions(descriptors);

        // Assert
        Assert.That(decisions.Count, Is.EqualTo(2));
        Assert.That(decisions[0].Descriptor, Is.TypeOf<TestWarriorDescriptor>());
        Assert.That(decisions[1].Descriptor, Is.TypeOf<TestArcherDescriptor>());
    }

    #endregion

    #region Edge Cases

    [Test]
    public void Process_WithEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var strategy = new StrategyWithMixedProcessing();
        var emptyList = new List<AgentDescriptor>();

        // Act
        var result = strategy.Process(emptyList, null!);

        // Assert
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(strategy.BatchCallCount, Is.EqualTo(0));
        Assert.That(strategy.WarriorCallCount, Is.EqualTo(0));
    }

    [Test]
    public void Process_WithNullGameState_ShouldStillProcess()
    {
        // Arrange
        var strategy = new StrategyWithTypedProcessing();
        var warriors = new List<AgentDescriptor>
        {
            new TestWarriorDescriptor("Warrior1")
        };

        // Act
        var result = strategy.Process(warriors, null!);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(strategy.WarriorCallCount, Is.EqualTo(1));
    }

    #endregion

    #region Performance/Caching Tests

    [Test]
    public void MethodResolution_ShouldCacheResults()
    {
        // Arrange
        var strategy = new StrategyWithMixedProcessing();
        var descriptors1 = new List<AgentDescriptor>
        {
            new TestWarriorDescriptor("Warrior1"),
            new TestArcherDescriptor("Archer1")
        };

        // Act - First call (method resolution happens)
        var result1 = strategy.Process(descriptors1, null!);
        
        // Reset counters
        var firstBatchCount = strategy.BatchCallCount;
        var firstWarriorCount = strategy.WarriorCallCount;

        // Act - Second call (should use cached method info)
        var descriptors2 = new List<AgentDescriptor>
        {
            new TestWarriorDescriptor("Warrior2"),
            new TestArcherDescriptor("Archer2")
        };
        var result2 = strategy.Process(descriptors2, null!);

        // Assert
        Assert.That(strategy.BatchCallCount, Is.EqualTo(firstBatchCount + 1), "Batch should be called again");
        Assert.That(strategy.WarriorCallCount, Is.EqualTo(firstWarriorCount + 1), "Warrior should be called again");
        
        // Both calls should succeed
        Assert.That(result1.Count, Is.EqualTo(2));
        Assert.That(result2.Count, Is.EqualTo(2));
    }

    #endregion
}
