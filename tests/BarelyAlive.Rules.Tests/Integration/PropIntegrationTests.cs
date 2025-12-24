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
        _factory = new GenericActorFactory(_mockCatalog.Object);
    }

    [Test]
    public void BuildProp_ZombieSpawn_ShouldMapOrderImplicitly()
    {
        // Arrange
        var defId = "Spawn.Zombie";
        var definition = new ZombieSpawnDefinition(defId, "Zombie Spawn", "Spawn") { Order = 1 };
        var descriptor = new ZombieSpawnDescriptor(defId)
        {
            Order = 99 // Override value
        };

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
        var definition = new DoorDefinition(defId, "Door", "Prop");
        var descriptor = new DoorDescriptor(defId, Color.Green); // Constructor sets color

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
