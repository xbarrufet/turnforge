using NUnit.Framework;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.ValueObjects;
using TurnForge.Rules.BarelyAlive.Actors;

namespace TurnForge.Rules.BarelyAlive.Test.Actors;

[TestFixture]
public class BarelyAliveActorFactoryTests
{
    private BarelyAliveActorFactory _factory;

    [SetUp]
    public void Setup()
    {
        _factory = new BarelyAliveActorFactory();
    }

    [Test]
    public void BuildProp_with_PartySpawnPoint_customType_creates_Prop_with_correct_type()
    {
        // Arrange
        var propDescriptor = new PropDescriptor(
            Id: ActorId.New(),
            CustomType: "PartySpawnPoint",
            Traits: new Dictionary<string, IReadOnlyList<ActorTraitDefinition>>(),
            Position: new Position(0, 0)
        );

        // Act
        var prop = _factory.BuildProp(propDescriptor, new Position(10, 10));

        // Assert
        Assert.That(prop, Is.Not.Null);
        Assert.That(prop.CustomType, Is.EqualTo("PartySpawnPoint"));
        Assert.That(prop.Position, Is.EqualTo(new Position(10, 10)));
    }

    [Test]
    public void BuildProp_with_ZombieSpawnPoint_customType_creates_Prop_with_correct_type()
    {
        // Arrange
        var traits = new Dictionary<string, IReadOnlyList<ActorTraitDefinition>>
        {
            {
                "SpawnOrder",
                new List<ActorTraitDefinition>
                {
                    new(Name: "SpawnOrder", Attributes: new Dictionary<string, string> { { "order", "3" } })
                }
            }
        };

        var propDescriptor = new PropDescriptor(
            Id: ActorId.New(),
            CustomType: "ZombieSpawnPoint",
            Traits: traits,
            Position: new Position(5, 5)
        );

        // Act
        var prop = _factory.BuildProp(propDescriptor, new Position(15, 15));

        // Assert
        Assert.That(prop, Is.Not.Null);
        Assert.That(prop.CustomType, Is.EqualTo("ZombieSpawnPoint"));
        Assert.That(prop.Position, Is.EqualTo(new Position(15, 15)));
        Assert.That(prop.Traits, Is.Not.Null);
    }

    [Test]
    public void BuildProp_creates_actor_with_descriptor_position_when_no_override_position()
    {
        // Arrange
        var descriptorPosition = new Position(7, 8);
        var propDescriptor = new PropDescriptor(
            Id: ActorId.New(),
            CustomType: "TestProp",
            Traits: new Dictionary<string, IReadOnlyList<ActorTraitDefinition>>(),
            Position: descriptorPosition
        );

        // Act
        var prop = _factory.BuildProp(propDescriptor, descriptorPosition);

        // Assert
        Assert.That(prop.Position, Is.EqualTo(descriptorPosition));
    }
}
