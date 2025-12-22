using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;

namespace TurnForge.Engine.Tests.Integration;

/// <summary>
/// Tests for [MapToBehaviours] attribute functionality
/// </summary>
[TestFixture]
public class MapToBehavioursTests
{
    private InMemoryGameCatalog _catalog = null!;

    [SetUp]
    public void Setup()
    {
        _catalog = new InMemoryGameCatalog();
    }

    [Test]
    public void MapToBehaviours_CopiesBehavioursFromDefinitionToEntity()
    {
        // Arrange
        var definition = new TestDefinitionWithBehaviours
        {
            DefinitionId = "Test.WithBehaviours",
            Name = "Test Entity",
            Category = "Test",
            MaxHealth = 10,
            Behaviours = new List<BaseBehaviour>
            {
                new TestBehaviour { BehaviourName = "TestBehaviour1" },
                new TestBehaviour { BehaviourName = "TestBehaviour2" }
            }
        };

        _catalog.RegisterDefinition(definition.DefinitionId, definition);
        var descriptor = new AgentDescriptor(definition.DefinitionId);
        var factory = new GenericActorFactory(_catalog);

        // Act
        var agent = factory.BuildAgent(descriptor);

        // Assert
        var behaviourComponent = agent.GetComponent<IBehaviourComponent>();
        Assert.That(behaviourComponent, Is.Not.Null, "Agent should have BehaviourComponent");
        
        var behaviour1 = behaviourComponent!.GetBehaviour<TestBehaviour>();
        Assert.That(behaviour1, Is.Not.Null, "Should have TestBehaviour");
        Assert.That(behaviour1!.BehaviourName, Is.EqualTo("TestBehaviour1"));
    }

    [Test]
    public void MapToBehaviours_EmptyList_DoesNotThrow()
    {
        // Arrange
        var definition = new TestDefinitionWithBehaviours
        {
            DefinitionId = "Test.NoBehaviours",
            Name = "Test Entity",
            Category = "Test",
            MaxHealth = 10,
            Behaviours = new List<BaseBehaviour>() // Empty
        };

        _catalog.RegisterDefinition(definition.DefinitionId, definition);
        var descriptor = new AgentDescriptor(definition.DefinitionId);
        var factory = new GenericActorFactory(_catalog);

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() =>
        {
            var agent = factory.BuildAgent(descriptor);
            var behaviourComp = agent.GetComponent<IBehaviourComponent>();
            Assert.That(behaviourComp, Is.Not.Null);
        });
    }

    #region Test Helper Classes

    private class TestDefinitionWithBehaviours : GameEntityDefinition
    {
        [MapToComponent(typeof(IHealthComponent), "MaxHealth")]
        public int MaxHealth { get; set; }

        [MapToBehaviours]
        public IReadOnlyList<BaseBehaviour> Behaviours { get; set; } = new List<BaseBehaviour>();
    }

    private class TestBehaviour : BaseBehaviour
    {
        public string BehaviourName { get; set; } = string.Empty;
    }

    #endregion
}
