using NUnit.Framework;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Tests.Commands.Spawn;

/// <summary>
/// Tests for SpawnRequestBuilder fluent API.
/// These tests verify the builder produces correct SpawnRequest objects.
/// </summary>
[TestFixture]
public class SpawnRequestBuilderTests
{
    private readonly Position _testPosition = new(new TileId(Guid.NewGuid()));

    [Test]
    public void For_WithValidDefinitionId_CreatesBuilder()
    {
        // Act
        var builder = SpawnRequestBuilder.For("Survivors.Mike");
        var request = builder.Build();

        // Assert
        Assert.That(request.DefinitionId, Is.EqualTo("Survivors.Mike"));
        Assert.That(request.Count, Is.EqualTo(1));
        Assert.That(request.PropertyOverrides, Is.Null);
        Assert.That(request.ExtraComponents, Is.Null);
    }

    [Test]
    public void For_WithNullDefinitionId_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => SpawnRequestBuilder.For(null!));
        Assert.Throws<ArgumentException>(() => SpawnRequestBuilder.For(""));
        Assert.Throws<ArgumentException>(() => SpawnRequestBuilder.For("   "));
    }

    [Test]
    public void At_SetsPosition()
    {
        // Act
        var request = SpawnRequestBuilder
            .For("Survivors.Mike")
            .At(_testPosition)
            .Build();

        // Assert
        Assert.That(request.Position, Is.EqualTo(_testPosition));
    }

    [Test]
    public void WithCount_SetsCount()
    {
        // Act
        var request = SpawnRequestBuilder
            .For("Enemies.Zombie")
            .WithCount(5)
            .Build();

        // Assert
        Assert.That(request.Count, Is.EqualTo(5));
    }

    [Test]
    public void WithCount_ZeroOrNegative_ThrowsArgumentException()
    {
        // Arrange
        var builder = SpawnRequestBuilder.For("Test");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.WithCount(0));
        Assert.Throws<ArgumentException>(() => builder.WithCount(-1));
    }

    [Test]
    public void WithProperty_AddsToOverrides()
    {
        // Act
        var request = SpawnRequestBuilder
            .For("Enemies.Boss")
            .WithProperty("Health", 500)
            .WithProperty("PhaseCount", 3)
            .Build();

        // Assert
        Assert.That(request.PropertyOverrides, Is.Not.Null);
        Assert.That(request.PropertyOverrides!.Count, Is.EqualTo(2));
        Assert.That(request.PropertyOverrides["Health"], Is.EqualTo(500));
        Assert.That(request.PropertyOverrides["PhaseCount"], Is.EqualTo(3));
    }

    [Test]
    public void WithProperty_Generic_AddsToOverrides()
    {
        // Act
        var request = SpawnRequestBuilder
            .For("Survivors.Mike")
            .WithProperty<int>("Health", 12)
            .WithProperty<string>("Faction", "Police")
            .Build();

        // Assert
        Assert.That(request.PropertyOverrides!["Health"], Is.EqualTo(12));
        Assert.That(request.PropertyOverrides["Faction"], Is.EqualTo("Police"));
    }

    [Test]
    public void WithProperty_NullOrEmptyKey_ThrowsArgumentException()
    {
        // Arrange
        var builder = SpawnRequestBuilder.For("Test");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.WithProperty(null!, "value"));
        Assert.Throws<ArgumentException>(() => builder.WithProperty("", "value"));
        Assert.Throws<ArgumentException>(() => builder.WithProperty<int>(null!, 123));
    }

    [Test]
    public void WithComponent_AddsComponent()
    {
        // Arrange
        var mockComponent = new TestComponent();

        // Act
        var request = SpawnRequestBuilder
            .For("Test")
            .WithComponent(mockComponent)
            .Build();

        // Assert
        Assert.That(request.ExtraComponents, Is.Not.Null);
        Assert.That(request.ExtraComponents!.Count(), Is.EqualTo(1));
        Assert.That(request.ExtraComponents.First(), Is.SameAs(mockComponent));
    }

    [Test]
    public void WithComponents_AddsMultipleComponents()
    {
        // Arrange
        var component1 = new TestComponent();
        var component2 = new TestComponent();

        // Act
        var request = SpawnRequestBuilder
            .For("Test")
            .WithComponents(component1, component2)
            .Build();

        // Assert
        Assert.That(request.ExtraComponents!.Count(), Is.EqualTo(2));
    }

    [Test]
    public void WithComponent_Null_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = SpawnRequestBuilder.For("Test");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.WithComponent<IGameEntityComponent>(null!));
    }

    [Test]
    public void FluentChaining_AllMethodsCombined_ProducesCorrectRequest()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var request = SpawnRequestBuilder
            .For("Enemies.DragonBoss")
            .At(_testPosition)
            .WithCount(1)
            .WithProperty("Health", 1000)
            .WithProperty("PhaseCount", 5)
            .WithProperty<string>("Faction", "Undead")
            .WithComponent(component)
            .Build();

        // Assert
        Assert.That(request.DefinitionId, Is.EqualTo("Enemies.DragonBoss"));
        Assert.That(request.Position, Is.EqualTo(_testPosition));
        Assert.That(request.Count, Is.EqualTo(1));
        Assert.That(request.PropertyOverrides!.Count, Is.EqualTo(3));
        Assert.That(request.PropertyOverrides["Health"], Is.EqualTo(1000));
        Assert.That(request.PropertyOverrides["PhaseCount"], Is.EqualTo(5));
        Assert.That(request.PropertyOverrides["Faction"], Is.EqualTo("Undead"));
        Assert.That(request.ExtraComponents!.Count(), Is.EqualTo(1));
    }

    [Test]
    public void ImplicitConversion_ProducesSpawnRequest()
    {
        // Act - implicit conversion via assignment
        SpawnRequest request = SpawnRequestBuilder
            .For("Survivors.Mike")
            .At(_testPosition);

        // Assert
        Assert.That(request, Is.Not.Null);
        Assert.That(request.DefinitionId, Is.EqualTo("Survivors.Mike"));
        Assert.That(request.Position, Is.EqualTo(_testPosition));
    }

    [Test]
    public void Build_WithoutSettingDefinitionId_ThrowsInvalidOperationException()
    {
        // Arrange - bypass For() to create invalid state (reflection hack for testing)
        var builder = (SpawnRequestBuilder)System.Runtime.Serialization.FormatterServices
            .GetUninitializedObject(typeof(SpawnRequestBuilder));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void ComplexScenario_BatchSpawnWithVariations()
    {
        // Arrange
        var positions = Enumerable.Range(0, 5)
            .Select(_ => new Position(new TileId(Guid.NewGuid())))
            .ToList();

        // Act - Create 5 zombies with increasing health
        var requests = positions.Select((pos, index) => SpawnRequestBuilder
            .For("Enemies.Zombie")
            .At(pos)
            .WithProperty("Health", 10 + (index * 5))
            .WithProperty("Speed", index % 2 == 0 ? "Fast" : "Slow")
            .Build())
            .ToList();

        // Assert
        Assert.That(requests.Count, Is.EqualTo(5));
        Assert.That(requests[0].PropertyOverrides!["Health"], Is.EqualTo(10));
        Assert.That(requests[2].PropertyOverrides!["Health"], Is.EqualTo(20));
        Assert.That(requests[4].PropertyOverrides!["Health"], Is.EqualTo(30));
        Assert.That(requests[0].PropertyOverrides["Speed"], Is.EqualTo("Fast"));
        Assert.That(requests[1].PropertyOverrides["Speed"], Is.EqualTo("Slow"));
    }

    // Helper test component
    private class TestComponent : IGameEntityComponent
    {
        public Guid EntityId { get; set; }
    }
}
