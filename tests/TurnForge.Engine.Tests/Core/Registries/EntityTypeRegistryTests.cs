using NUnit.Framework;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Core.Registries;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Core.Registries;

/// <summary>
/// Tests for EntityTypeRegistry functionality.
/// Verifies Definition→Entity and Entity→Descriptor type mappings.
/// </summary>
[TestFixture]
public class EntityTypeRegistryTests
{
    #region Test Entities and Definitions

    // Test entity with both attributes
    [DefinitionType(typeof(TestPlayerDefinition))]
    [DescriptorType(typeof(TestPlayerDescriptor))]
    public class TestPlayer : Agent
    {
        public TestPlayer(EntityId id, string definitionId, string name, string category)
            : base(id, definitionId, name, category) { }
    }

    // Test definition
    public class TestPlayerDefinition : BaseGameEntityDefinition
    {
        public int MaxHealth { get; set; } = 100;
        
        public TestPlayerDefinition(string definitionId, string name, string category)
            : base(definitionId) 
        {
            Traits.Add(new TurnForge.Engine.Traits.Standard.IdentityTrait(name, category));
        }
    }

    // Test descriptor
    public class TestPlayerDescriptor : AgentDescriptor
    {
        public string Faction { get; set; } = "Alliance";
        
        public TestPlayerDescriptor(string definitionId) : base(definitionId) { }
    }

    // Entity with only DefinitionType
    [DefinitionType(typeof(TestEnemyDefinition))]
    public class TestEnemy : Agent
    {
        public TestEnemy(EntityId id, string definitionId, string name, string category)
            : base(id, definitionId, name, category) { }
    }

    public class TestEnemyDefinition : BaseGameEntityDefinition
    {
        public TestEnemyDefinition(string definitionId, string name, string category)
            : base(definitionId) 
        {
            Traits.Add(new TurnForge.Engine.Traits.Standard.IdentityTrait(name, category));
        }
    }

    // Prop entity for testing
    [DefinitionType(typeof(TestDoorDefinition))]
    [DescriptorType(typeof(TestDoorDescriptor))]
    public class TestDoor : Prop
    {
        public TestDoor(EntityId id, string definitionId, string name, string category)
            : base(id, definitionId, name, category) { }
    }

    public class TestDoorDefinition : BaseGameEntityDefinition
    {
        public TestDoorDefinition(string definitionId, string name, string category)
            : base(definitionId)
        {
            Traits.Add(new TurnForge.Engine.Traits.Standard.IdentityTrait(name, category));
        }
    }

    public class TestDoorDescriptor : PropDescriptor
    {
        public bool IsLocked { get; set; } = false;
        
        public TestDoorDescriptor(string definitionId) : base(definitionId) { }
    }

    #endregion

    [SetUp]
    public void Setup()
    {
        // Clear registry before each test to ensure isolation
        EntityTypeRegistry.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        // Re-initialize after clearing for other tests
        EntityTypeRegistry.Initialize();
    }

    #region Initialization Tests

    [Test]
    public void Initialize_ShouldScanAssembliesForEntities()
    {
        // Act
        EntityTypeRegistry.Initialize();

        // Assert
        var playerEntityType = EntityTypeRegistry.GetEntityType(typeof(TestPlayerDefinition));
        Assert.That(playerEntityType, Is.EqualTo(typeof(TestPlayer)));
    }

    [Test]
    public void Initialize_CalledMultipleTimes_ShouldBeIdempotent()
    {
        // Act
        EntityTypeRegistry.Initialize();
        EntityTypeRegistry.Initialize();
        EntityTypeRegistry.Initialize();

        // Assert
        var entityType = EntityTypeRegistry.GetEntityType(typeof(TestPlayerDefinition));
        Assert.That(entityType, Is.EqualTo(typeof(TestPlayer)));
    }

    #endregion

    #region GetEntityType Tests

    [Test]
    public void GetEntityType_WithRegisteredDefinition_ShouldReturnEntityType()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var entityType = EntityTypeRegistry.GetEntityType(typeof(TestPlayerDefinition));

        // Assert
        Assert.That(entityType, Is.Not.Null);
        Assert.That(entityType, Is.EqualTo(typeof(TestPlayer)));
    }

    [Test]
    public void GetEntityType_WithUnregisteredDefinition_ShouldReturnNull()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var entityType = EntityTypeRegistry.GetEntityType(typeof(BaseGameEntityDefinition));

        // Assert
        Assert.That(entityType, Is.Null);
    }

    [Test]
    public void GetEntityType_WithMultipleEntities_ShouldReturnCorrectTypes()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var playerType = EntityTypeRegistry.GetEntityType(typeof(TestPlayerDefinition));
        var enemyType = EntityTypeRegistry.GetEntityType(typeof(TestEnemyDefinition));
        var doorType = EntityTypeRegistry.GetEntityType(typeof(TestDoorDefinition));

        // Assert
        Assert.That(playerType, Is.EqualTo(typeof(TestPlayer)));
        Assert.That(enemyType, Is.EqualTo(typeof(TestEnemy)));
        Assert.That(doorType, Is.EqualTo(typeof(TestDoor)));
    }

    #endregion

    #region GetDefinitionType Tests

    [Test]
    public void GetDefinitionType_WithRegisteredEntity_ShouldReturnDefinitionType()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var definitionType = EntityTypeRegistry.GetDefinitionType(typeof(TestPlayer));

        // Assert
        Assert.That(definitionType, Is.Not.Null);
        Assert.That(definitionType, Is.EqualTo(typeof(TestPlayerDefinition)));
    }

    [Test]
    public void GetDefinitionType_WithUnregisteredEntity_ShouldReturnNull()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var definitionType = EntityTypeRegistry.GetDefinitionType(typeof(Agent));

        // Assert
        Assert.That(definitionType, Is.Null);
    }

    #endregion

    #region GetDescriptorType Tests

    [Test]
    public void GetDescriptorType_WithDescriptorAttribute_ShouldReturnDescriptorType()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var descriptorType = EntityTypeRegistry.GetDescriptorType(typeof(TestPlayer));

        // Assert
        Assert.That(descriptorType, Is.Not.Null);
        Assert.That(descriptorType, Is.EqualTo(typeof(TestPlayerDescriptor)));
    }

    [Test]
    public void GetDescriptorType_WithoutDescriptorAttribute_ShouldReturnNull()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var descriptorType = EntityTypeRegistry.GetDescriptorType(typeof(TestEnemy));

        // Assert
        Assert.That(descriptorType, Is.Null, "TestEnemy has no DescriptorType attribute");
    }

    [Test]
    public void GetDescriptorType_ForPropEntity_ShouldReturnPropDescriptor()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var descriptorType = EntityTypeRegistry.GetDescriptorType(typeof(TestDoor));

        // Assert
        Assert.That(descriptorType, Is.EqualTo(typeof(TestDoorDescriptor)));
    }

    #endregion

    #region GetTypeChain Tests

    [Test]
    public void GetTypeChain_WithCompleteChain_ShouldReturnBothTypes()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var (entityType, descriptorType) = EntityTypeRegistry.GetTypeChain(typeof(TestPlayerDefinition));

        // Assert
        Assert.That(entityType, Is.EqualTo(typeof(TestPlayer)));
        Assert.That(descriptorType, Is.EqualTo(typeof(TestPlayerDescriptor)));
    }

    [Test]
    public void GetTypeChain_WithOnlyEntityType_ShouldReturnEntityAndNullDescriptor()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var (entityType, descriptorType) = EntityTypeRegistry.GetTypeChain(typeof(TestEnemyDefinition));

        // Assert
        Assert.That(entityType, Is.EqualTo(typeof(TestEnemy)));
        Assert.That(descriptorType, Is.Null, "TestEnemy has no DescriptorType attribute");
    }

    [Test]
    public void GetTypeChain_WithUnregisteredDefinition_ShouldReturnNulls()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var (entityType, descriptorType) = EntityTypeRegistry.GetTypeChain(typeof(BaseGameEntityDefinition));

        // Assert
        Assert.That(entityType, Is.Null);
        Assert.That(descriptorType, Is.Null);
    }

    #endregion

    #region HasCustomEntity Tests

    [Test]
    public void HasCustomEntity_WithRegisteredDefinition_ShouldReturnTrue()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var hasCustomEntity = EntityTypeRegistry.HasCustomEntity(typeof(TestPlayerDefinition));

        // Assert
        Assert.That(hasCustomEntity, Is.True);
    }

    [Test]
    public void HasCustomEntity_WithUnregisteredDefinition_ShouldReturnFalse()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var hasCustomEntity = EntityTypeRegistry.HasCustomEntity(typeof(BaseGameEntityDefinition));

        // Assert
        Assert.That(hasCustomEntity, Is.False);
    }

    #endregion

    #region Manual Registration Tests

    [Test]
    public void Register_ManualRegistration_ShouldAddMapping()
    {
        // Arrange
        EntityTypeRegistry.Clear();

        // Act
        EntityTypeRegistry.Register(typeof(TestPlayerDefinition), typeof(TestPlayer));

        // Assert
        var entityType = EntityTypeRegistry.GetEntityType(typeof(TestPlayerDefinition));
        Assert.That(entityType, Is.EqualTo(typeof(TestPlayer)));
    }

    [Test]
    public void Register_WithInvalidDefinitionType_ShouldThrowException()
    {
        // Arrange
        EntityTypeRegistry.Clear();

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            EntityTypeRegistry.Register(typeof(string), typeof(TestPlayer)));
        
        Assert.That(ex.Message, Does.Contain("BaseGameEntityDefinition"));
    }

    [Test]
    public void Register_WithInvalidEntityType_ShouldThrowException()
    {
        // Arrange
        EntityTypeRegistry.Clear();

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            EntityTypeRegistry.Register(typeof(TestPlayerDefinition), typeof(string)));
        
        Assert.That(ex.Message, Does.Contain("GameEntity"));
    }

    #endregion

    #region GetAllMappings Tests

    [Test]
    public void GetAllMappings_ShouldReturnAllRegisteredMappings()
    {
        // Arrange
        EntityTypeRegistry.Initialize();

        // Act
        var mappings = EntityTypeRegistry.GetAllMappings();

        // Assert
        Assert.That(mappings.Count, Is.GreaterThan(0));
        Assert.That(mappings.ContainsKey(typeof(TestPlayerDefinition)), Is.True);
        Assert.That(mappings[typeof(TestPlayerDefinition)], Is.EqualTo(typeof(TestPlayer)));
    }

    #endregion

    #region Thread Safety Tests

    [Test]
    public void Initialize_ConcurrentCalls_ShouldBeThreadSafe()
    {
        // Arrange
        var tasks = new System.Threading.Tasks.Task[10];

        // Act
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = System.Threading.Tasks.Task.Run(() => EntityTypeRegistry.Initialize());
        }

        System.Threading.Tasks.Task.WaitAll(tasks);

        // Assert
        var entityType = EntityTypeRegistry.GetEntityType(typeof(TestPlayerDefinition));
        Assert.That(entityType, Is.EqualTo(typeof(TestPlayer)));
    }

    #endregion
}
