using NUnit.Framework;
using TurnForge.Engine.Appliers.Spawn;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Core.Factories;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Decisions.Spawn;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Integration;

/// <summary>
/// Integration tests for the complete spawn system pipeline:
/// SpawnRequest → DescriptorBuilder → Strategy → SpawnDecision → Applier → Entity
/// </summary>
[TestFixture]
public class SpawnSystemIntegrationTests
{
    private TurnForgeOrchestrator _orchestrator = null!;
    private GenericActorFactory _factory = null!;
    private InMemoryGameCatalog _catalog = null!;

    [SetUp]
    public void Setup()
    {
        // Setup catalog with test definitions
        _catalog = new InMemoryGameCatalog();
        _catalog.RegisterDefinition("TestAgent", new TestAgentDefinition
        {
            DefinitionId = "TestAgent",
            Name = "Test Agent",
            Category = "TestCategory"
        });
        _catalog.RegisterDefinition("TestProp", new TestPropDefinition
        {
            DefinitionId = "TestProp",
            Name = "Test Prop",
            Category = "TestPropCategory"
        });

        // Setup factory
        _factory = new GenericActorFactory(_catalog);

        // Setup appliers
        var agentApplier = new AgentSpawnApplier(_factory);
        var propApplier = new PropSpawnApplier(_factory);

        // Setup orchestrator
        _orchestrator = new TurnForgeOrchestrator();
        _orchestrator.RegisterApplier(agentApplier);
        _orchestrator.RegisterApplier(propApplier);
        _orchestrator.SetState(GameState.Empty());
    }

    [Test]
    public void AgentSpawnApplier_IsRegistered_InOrchestrator()
    {
        // Arrange
        var descriptor = new AgentDescriptor("TestAgent");
        var decision = new SpawnDecision<AgentDescriptor>(descriptor);

        // Act - Should not throw "No applier registered"
        var effects = _orchestrator.Apply(decision);

        // Assert
        Assert.That(effects, Is.Not.Null);
        Assert.That(effects.Length, Is.EqualTo(1));
        Assert.That(_orchestrator.CurrentState.Agents.Count, Is.EqualTo(1));
    }

    [Test]
    public void PropSpawnApplier_IsRegistered_InOrchestrator()
    {
        // Arrange
        var descriptor = new PropDescriptor("TestProp");
        var decision = new SpawnDecision<PropDescriptor>(descriptor);

        // Act
        var effects = _orchestrator.Apply(decision);

        // Assert
        Assert.That(effects, Is.Not.Null);
        Assert.That(effects.Length, Is.EqualTo(1));
        Assert.That(_orchestrator.CurrentState.Props.Count, Is.EqualTo(1));
    }

    [Test]
    public void SpawnDecision_CreatesAgent_WithCorrectProperties()
    {
        // Arrange
        var descriptor = new AgentDescriptor("TestAgent");
        var decision = new SpawnDecision<AgentDescriptor>(descriptor);

        // Act
        _orchestrator.Apply(decision);
        var agent = _orchestrator.CurrentState.Agents.First().Value;

        // Assert
        Assert.That(agent.DefinitionId, Is.EqualTo("TestAgent"));
        Assert.That(agent.Name, Is.EqualTo("Test Agent"));
        Assert.That(agent.Category, Is.EqualTo("TestCategory"));
    }

    [Test]
    public void SpawnDecision_GeneratesEntitySpawnedEffect()
    {
        // Arrange
        var descriptor = new AgentDescriptor("TestAgent");
        var decision = new SpawnDecision<AgentDescriptor>(descriptor);

        // Act
        var effects = _orchestrator.Apply(decision);
        var effect = effects[0];

        // Assert
        Assert.That(effect, Is.Not.Null);
        
        // Verify effect type and data via reflection
        var definitionIdProp = effect.GetType().GetProperty("DefinitionId");
        var entityTypeProp = effect.GetType().GetProperty("EntityType");
        
        Assert.That(definitionIdProp?.GetValue(effect), Is.EqualTo("TestAgent"));
        Assert.That(entityTypeProp?.GetValue(effect), Is.EqualTo("Agent"));
    }

    [Test]
    public void MultipleSpawnDecisions_CreateMultipleEntities()
    {
        // Arrange
        var descriptor1 = new AgentDescriptor("TestAgent");
        var descriptor2 = new AgentDescriptor("TestAgent");
        var decision1 = new SpawnDecision<AgentDescriptor>(descriptor1);
        var decision2 = new SpawnDecision<AgentDescriptor>(descriptor2);

        // Act
        _orchestrator.Apply(decision1);
        _orchestrator.Apply(decision2);

        // Assert
        Assert.That(_orchestrator.CurrentState.Agents.Count, Is.EqualTo(2));
        
        // Verify different IDs
        var agentIds = _orchestrator.CurrentState.Agents.Keys.ToList();
        Assert.That(agentIds[0], Is.Not.EqualTo(agentIds[1]));
    }

    [Test]
    public void DescriptorBuilder_BuildsAgentDescriptor_FromRequest()
    {
        // Arrange
        var request = new SpawnRequest("TestAgent", Count: 1);
        var definition = _catalog.GetDefinition<BaseGameEntityDefinition>("TestAgent");

        // Act
        var descriptor = DescriptorBuilder.Build<AgentDescriptor>(request, definition);

        // Assert
        Assert.That(descriptor, Is.Not.Null);
        Assert.That(descriptor.DefinitionID, Is.EqualTo("TestAgent"));
    }

    [Test]
    public void CompleteSpawnFlow_Request_To_Entity()
    {
        // Arrange: SpawnRequest
        var request = new SpawnRequest("TestAgent", Count: 1);
        var definition = _catalog.GetDefinition<BaseGameEntityDefinition>("TestAgent");

        // Act 1: Build Descriptor
        var descriptor = DescriptorBuilder.Build<AgentDescriptor>(request, definition);

        // Act 2: Create Decision
        var decision = new SpawnDecision<AgentDescriptor>(descriptor);

        // Act 3: Apply Decision
        var effects = _orchestrator.Apply(decision);

        // Assert: Entity created with correct properties
        Assert.That(_orchestrator.CurrentState.Agents.Count, Is.EqualTo(1));
        var agent = _orchestrator.CurrentState.Agents.First().Value;
        
        Assert.That(agent.DefinitionId, Is.EqualTo("TestAgent"));
        Assert.That(effects.Length, Is.EqualTo(1));
    }

    [Test]
    public void BatchSpawn_CreatesMultipleEntities()
    {
        // Arrange
        var request = new SpawnRequest("TestAgent", Count: 3);
        var definition = _catalog.GetDefinition<BaseGameEntityDefinition>("TestAgent");

        // Act: Simulate batch spawn (Count = 3)
        for (int i = 0; i < request.Count; i++)
        {
            var descriptor = DescriptorBuilder.Build<AgentDescriptor>(request, definition);
            var decision = new SpawnDecision<AgentDescriptor>(descriptor);
            _orchestrator.Apply(decision);
        }

        // Assert
        Assert.That(_orchestrator.CurrentState.Agents.Count, Is.EqualTo(3));
    }

    #region Test Helper Classes

    /// <summary>
    /// Test definition for agents
    /// </summary>
    private class TestAgentDefinition : BaseGameEntityDefinition
    {
        public int MaxHealth { get; set; }
    }

    /// <summary>
    /// Test definition for props
    /// </summary>
    private class TestPropDefinition : BaseGameEntityDefinition
    {
        public bool IsBlocking { get; set; }
    }

    #endregion
}
