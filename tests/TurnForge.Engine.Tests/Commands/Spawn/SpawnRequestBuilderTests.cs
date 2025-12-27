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
        Assert.That(request.TraitsToOverride, Is.Null);
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
    public void At_SetsPositionTrait()
    {
        // Act
        var request = SpawnRequestBuilder
            .For("Survivors.Mike")
            .At(_testPosition)
            .Build();

        // Assert
        var traits = request.TraitsToOverride;
        Assert.That(traits, Is.Not.Null);
        var posTrait = traits!.OfType<TurnForge.Engine.Traits.Standard.PositionTrait>().FirstOrDefault();
        Assert.That(posTrait, Is.Not.Null);
        Assert.That(posTrait!.InitialPosition, Is.EqualTo(_testPosition));
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
    public void WithTrait_AddsToTraits()
    {
        // Arrange
        var trait = new TurnForge.Engine.Traits.Standard.TeamTrait("TeamA", "Controller1");

        // Act
        var request = SpawnRequestBuilder
            .For("Enemies.Boss")
            .WithTrait(trait)
            .Build();

        // Assert
        Assert.That(request.TraitsToOverride, Is.Not.Null);
        Assert.That(request.TraitsToOverride!.Count(), Is.EqualTo(1));
        Assert.That(request.TraitsToOverride.First(), Is.SameAs(trait));
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
        var trait = new TurnForge.Engine.Traits.Standard.TeamTrait("TeamB", "Controller2");

        // Act
        var request = SpawnRequestBuilder
            .For("Enemies.DragonBoss")
            .At(_testPosition)
            .WithCount(1)
            .WithTrait(trait)
            .WithComponent(component)
            .Build();

        // Assert
        Assert.That(request.DefinitionId, Is.EqualTo("Enemies.DragonBoss"));
        Assert.That(request.Count, Is.EqualTo(1));
        
        var traits = request.TraitsToOverride;
        Assert.That(traits!.Count(), Is.EqualTo(2)); // Position + Team
        Assert.That(traits.OfType<TurnForge.Engine.Traits.Standard.PositionTrait>().Any(), Is.True);
        Assert.That(traits.OfType<TurnForge.Engine.Traits.Standard.TeamTrait>().Any(), Is.True);
        
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
        
        var posTrait = request.TraitsToOverride!.OfType<TurnForge.Engine.Traits.Standard.PositionTrait>().FirstOrDefault();
        Assert.That(posTrait, Is.Not.Null);
        Assert.That(posTrait!.InitialPosition, Is.EqualTo(_testPosition));
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

    // Helper test component
    private class TestComponent : IGameEntityComponent
    {
        public Guid EntityId { get; set; }
    }
}
