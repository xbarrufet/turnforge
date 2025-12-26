using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Behaviours;
using TurnForge.Engine.Behaviours.Interfaces;

namespace TurnForge.Engine.Tests.Integration;

/// <summary>
/// Tests for [MapToTraits] attribute functionality
/// </summary>
[TestFixture]
public class MapToTraitsTests
{
    private InMemoryGameCatalog _catalog = null!;

    [SetUp]
    public void Setup()
    {
        _catalog = new InMemoryGameCatalog();
    }

    [Test]
    public void MapToTraits_CopiesTraitsFromDefinitionToEntity()
    {
        // Arrange
        var definition = new TestDefinitionWithTraits
        {
            DefinitionId = "Test.WithTraits",
            Name = "Test Entity",
            Category = "Test",
            MaxHealth = 10,
            Traits = new List<BaseTrait>
            {
                new TestTrait { TraitName = "TestTrait1" },
                new TestTrait { TraitName = "TestTrait2" }
            }
        };

        _catalog.RegisterDefinition(definition);
        var descriptor = new AgentDescriptor(definition.DefinitionId);
        var factory = new GenericActorFactory(_catalog);

        // Act
        var agent = factory.BuildAgent(descriptor);

        // Assert
        var traitComponent = agent.GetComponent<ITraitContainerComponent>();
        Assert.That(traitComponent, Is.Not.Null, "Agent should have TraitContainerComponent");
        
        var trait1 = traitComponent!.GetTrait<TestTrait>();
        Assert.That(trait1, Is.Not.Null, "Should have TestTrait");
        Assert.That(trait1!.TraitName, Is.EqualTo("TestTrait1"));
    }

    [Test]
    public void MapToTraits_EmptyList_DoesNotThrow()
    {
        // Arrange
        var definition = new TestDefinitionWithTraits
        {
            DefinitionId = "Test.NoTraits",
            Name = "Test Entity",
            Category = "Test",
            MaxHealth = 10,
            Traits = new List<BaseTrait>() // Empty
        };

        _catalog.RegisterDefinition(definition);
        var descriptor = new AgentDescriptor(definition.DefinitionId);
        var factory = new GenericActorFactory(_catalog);

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() =>
        {
            var agent = factory.BuildAgent(descriptor);
            var traitComp = agent.GetComponent<ITraitContainerComponent>();
            Assert.That(traitComp, Is.Not.Null);
        });
    }

    #region Test Helper Classes

    private class TestDefinitionWithTraits : BaseGameEntityDefinition
    {
        [MapToComponent(typeof(IHealthComponent), "MaxHealth")]
        public int MaxHealth { get; set; }

        [MapToTraits]
        public IReadOnlyList<BaseTrait> Traits { get; set; } = new List<BaseTrait>();
    }

    private class TestTrait : BaseTrait
    {
        public string TraitName { get; set; } = string.Empty;
    }

    #endregion
}
