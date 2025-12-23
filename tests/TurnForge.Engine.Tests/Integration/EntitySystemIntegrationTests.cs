using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Tests.Integration.Entities;

namespace TurnForge.Engine.Tests.Integration;

/// <summary>
/// Integration tests for Entity System following ENTIDADES.MD development phases
/// </summary>
[TestFixture]
public class EntitySystemIntegrationTests
{
    private InMemoryGameCatalog _catalog = null!;

    [SetUp]
    public void Setup()
    {
        _catalog = new InMemoryGameCatalog();
    }

    #region Phase 1: Custom Definition Registration & Retrieval

    /// <summary>
    /// Phase 1: Register custom definition (SurvivorDefinition) with Registry and Get
    /// </summary>
    [Test]
    public void Phase1_RegisterAndGetCustomDefinition_Success()
    {
        // Arrange - Create a custom SurvivorDefinition
        var survivorDef = new TestSurvivorDefinition
        {
            DefinitionId = "Survivors.Mike",
            Name = "Mike",
            Category = "Survivor",
            MaxHealth = 10

        };

        // Act - Register the definition
        _catalog.RegisterDefinition("Survivors.Mike", survivorDef);

        // Assert - Retrieve and verify
        var retrieved = _catalog.GetDefinition<TestSurvivorDefinition>("Survivors.Mike");
        
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.DefinitionId, Is.EqualTo("Survivors.Mike"));
        Assert.That(retrieved.Name, Is.EqualTo("Mike"));
        Assert.That(retrieved.Category, Is.EqualTo("Survivor"));
        Assert.That(retrieved.MaxHealth, Is.EqualTo(10));
    }

    [Test]
    public void Phase1_TryGetDefinition_ReturnsTrue_WhenExists()
    {
        // Arrange
        var survivorDef = new TestSurvivorDefinition
        {
            DefinitionId = "Survivors.Amy",
            Name = "Amy",
            Category = "Survivor",
            MaxHealth = 8
        };
        _catalog.RegisterDefinition("Survivors.Amy", survivorDef);

        // Act
        var success = _catalog.TryGetDefinition<TestSurvivorDefinition>("Survivors.Amy", out var result);

        // Assert
        Assert.That(success, Is.True);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Amy"));
    }

    [Test]
    public void Phase1_TryGetDefinition_ReturnsFalse_WhenNotExists()
    {
        // Act
        var success = _catalog.TryGetDefinition<TestSurvivorDefinition>("NonExistent", out var result);

        // Assert
        Assert.That(success, Is.False);
        Assert.That(result, Is.Null);
    }

    #endregion

    #region Phase 2: Entity Creation with Custom Descriptor and Universal Factory

    /// <summary>
    /// Phase 2: Create entity with custom Descriptor using Universal Factory
    /// TODO: Implement when GenericActorFactory supports custom definitions
    /// </summary>
    [Test]
    public void Phase2_CreateEntityWithCustomDescriptor_Success()
    {
        // Arrange - Register definition first
        var survivorDef = new TestSurvivorDefinition
        {
            DefinitionId = "Survivors.Mike",
            Name = "Mike",
            Category = "Survivor",
            MaxHealth = 10        };
        _catalog.RegisterDefinition("Survivors.Mike", survivorDef);

        // Create descriptor with position
        var descriptor = new TestSurvivorDescriptor("Survivors.Mike")
        {
            Position = new Position(TileId.New())
        };

        // Act - TODO: Use GenericActorFactory to create entity
        var factory = new GenericActorFactory(_catalog);
        var entity = factory.BuildAgent(descriptor);

        // Assert - TODO: Verify entity was created correctly
        Assert.That(entity, Is.Not.Null);
        Assert.That(entity.GetComponent<IHealthComponent>()?.MaxHealth, Is.EqualTo(10));
    }

    #endregion

    #region Phase 3: Definition with Custom Component and Registration

    /// <summary>
    /// Phase 3: Create definition with custom component and register
    /// TODO: Implement when custom component system is ready
    /// </summary>
    [Test]
    public void Phase3_DefinitionWithCustomComponent_Success()
    {
        // Arrange - Create definition with custom FactionComponent
        var survivorDef = new TestSurvivorDefinitionWithCustomComponent
        {
            DefinitionId = "Survivors.Josh",
            Name = "Josh",
            Category = "Survivor",
            Faction = "Police",
            MaxHealth = 12
        };
        // Act - Register
        _catalog.RegisterDefinition("Survivors.Josh", survivorDef);

        // Assert - Retrieve and verify custom component mapping
        var retrieved = _catalog.GetDefinition<TestSurvivorDefinitionWithCustomComponent>("Survivors.Josh");
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.Faction, Is.EqualTo("Police"));

        var descriptor = new TestSurvivorDescriptorWithCustomComponent("Survivors.Josh")
        {
            Position = new Position(TileId.New()),
            Faction = "Police"
        };

        // Act - TODO: Use GenericActorFactory to create entity
        var factory = new GenericActorFactory(_catalog);
        var entity = factory.BuildAgent(descriptor);

        // Assert
        Assert.That(entity, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(entity.GetComponent<IHealthComponent>()?.MaxHealth, Is.EqualTo(12));
            Assert.That(entity.GetComponent<IFactionComponent>()?.Faction, Is.EqualTo("Police"));
        });

    }

    #endregion

    #region Phase 4: Create Entity with Custom Component

    /// <summary>
    /// Phase 4: Create entity with custom component
    /// TODO: Implement when EngineAutoMapper supports custom components
    /// </summary>
    [Test]
    [Ignore("Phase 4: Pending Entity creation with custom components")]
    public void Phase4_CreateEntityWithCustomComponent_Success()
    {
        // TODO: Implement test for creating entity with custom FactionComponent
        // Should verify that EngineAutoMapper correctly maps custom component
    }

    #endregion

    #region Phase 5: Update Component with Universal Applier

    /// <summary>
    /// Phase 5: Update component using UniversalApplier
    /// TODO: Implement when UniversalApplier is available
    /// </summary>
    [Test]
    [Ignore("Phase 5: Pending UniversalApplier implementation")]
    public void Phase5_UpdateComponentWithUniversalApplier_Success()
    {
        // TODO: Implement test for updating entity components
        // Should verify atomic component updates work correctly
    }

    #endregion

    #region Test Helper Classes

    /// <summary>
    /// Test definition for Survivor with basic attributes
    /// </summary>
    private class TestSurvivorDefinition : BaseGameEntityDefinition
    {
        [MapToComponent(typeof(IHealthComponent), "MaxHealth")]
        public int MaxHealth { get; set; }

    }

    /// <summary>
    /// Test descriptor for creating survivors with position
    /// </summary>
    
    private class TestSurvivorDescriptor(string definitionId):AgentDescriptor(definitionId)
    {
        [MapToComponent(typeof(IPositionComponent), "CurrentPosition")]
        public Position Position { get; set; }
    }

    /// <summary>
    /// Test definition with custom component (Faction)
    /// </summary>
    [EntityType(typeof(Survivor))]
    private class TestSurvivorDefinitionWithCustomComponent : BaseGameEntityDefinition
    {
        [MapToComponent(typeof(IHealthComponent), nameof(IHealthComponent.MaxHealth))]
        public int MaxHealth { get; set; }

        // TODO: Define custom FactionComponent interface
        [MapToComponent(typeof(IFactionComponent), nameof(IFactionComponent.Faction))]
        public string Faction { get; set; } = string.Empty;
    }
       [EntityType(typeof(Survivor))]  
      private class TestSurvivorDescriptorWithCustomComponent(string definitionId):AgentDescriptor(definitionId)
    {
        [MapToComponent(typeof(IPositionComponent), "CurrentPosition")]
        public Position Position { get; set; }

        [MapToComponent(typeof(IFactionComponent), nameof(IFactionComponent.Faction))]
        public string Faction { get; set; } = string.Empty;
    }

    #endregion
}
