using NUnit.Framework;
using TurnForge.Engine.Entities.Actors;
using BarelyAlive.Rules.Core.Domain.Entities;
using BarelyAlive.Rules.Core.Domain.Descriptors;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using Moq;
using TurnForge.Engine.Entities;

namespace BarelyAlive.Rules.Tests.Integration;

[TestFixture]
public class PropIntegrationTests
{
    private Mock<IGameCatalog> _mockCatalog;
    private GenericActorFactory _factory;

    [SetUp]
    public void Setup()
    {
        _mockCatalog = new Mock<IGameCatalog>();
        _factory = new GenericActorFactory(_mockCatalog.Object, new TurnForge.Engine.Services.TraitInitializationService());
    }

    [Test]
    public void BuildProp_ZombieSpawn_ShouldMapOrderImplicitly()
    {
        // Arrange
        var defId = "Spawn.Zombie";
        var definition = new ZombieSpawnDefinition(defId)
        { 
            // Order = 1, // Legacy
            Traits = { 
                new TurnForge.Engine.Traits.Standard.IdentityTrait("Zombie Spawn", "Spawn")
            }
        };
        
        // Use Trait instead of Property Property
        var descriptor = new ZombieSpawnDescriptor(defId);
        descriptor.RequestedTraits.Add(new BarelyAlive.Rules.Core.Domain.Traits.SpawnOrderTrait(99));

        _mockCatalog.Setup(c => c.GetDefinition<BaseGameEntityDefinition>(defId))
            .Returns(definition);

        // Act
        var prop = _factory.BuildProp(descriptor);

        // Assert
        Assert.That(prop, Is.InstanceOf<ZombieSpawn>());
        var zombieSpawn = (ZombieSpawn)prop;
        
        var comp = zombieSpawn.GetComponent<ZombieSpawnComponent>();
        Assert.That(comp, Is.Not.Null);
        Assert.That(comp.Order, Is.EqualTo(99), "Order should be implicitly mapped from Descriptor");
    }

    [Test]
    public void BuildProp_Door_ShouldMapColorImplicitly()
    {
        // Arrange
        var defId = "Door";
        var definition = new DoorDefinition(defId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Door", "Prop") } };
        
        // Use Trait instead of Property
        var descriptor = new DoorDescriptor(defId);
        descriptor.RequestedTraits.Add(new BarelyAlive.Rules.Core.Domain.Traits.ColorTrait(Color.Green));

        _mockCatalog.Setup(c => c.GetDefinition<BaseGameEntityDefinition>(defId))
            .Returns(definition);

        // Act
        var prop = _factory.BuildProp(descriptor);

        // Assert
        Assert.That(prop, Is.InstanceOf<Door>());
        var door = (Door)prop;
        
        var comp = door.GetComponent<ColorComponent>();
        Assert.That(comp, Is.Not.Null);
        Assert.That(comp.Color, Is.EqualTo(Color.Green), "Color should be implicitly mapped from Descriptor");
    }
}
