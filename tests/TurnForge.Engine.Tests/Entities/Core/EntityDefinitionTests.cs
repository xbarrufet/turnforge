using NUnit.Framework;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Entities.Core;

[TestFixture]
public class EntityDefinitionTests
{
    [Test]
    public void AddTrait_WithoutRequired_AddsTraitSuccessfully()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        var trait = new VitalityTrait(100);

        // Act
        definition.AddTrait(trait, isRequired: false);

        // Assert
        Assert.That(definition.HasTrait<VitalityTrait>(), Is.True);
        Assert.That(definition.GetTrait<VitalityTrait>(), Is.Not.Null);
        Assert.That(definition.GetTrait<VitalityTrait>()!.BaseMaxHp, Is.EqualTo(100));
    }

    [Test]
    public void AddTrait_WithRequired_MarksTraitAsRequired()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        var trait = new VitalityTrait(100);

        // Act
        definition.AddTrait(trait, isRequired: true);

        // Assert
        var requiredTraits = definition.GetRequiredTraits<VitalityTrait>().ToList();
        Assert.That(requiredTraits, Has.Count.EqualTo(1));
        Assert.That(requiredTraits[0].BaseMaxHp, Is.EqualTo(100));
    }

    [Test]
    public void GetTraits_ReturnsAllTraits_RequiredAndNonRequired()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        definition.AddTrait(new VitalityTrait(100), isRequired: true);
        definition.AddTrait(new VitalityTrait(200), isRequired: false);

        // Act
        var allTraits = definition.GetTraits<VitalityTrait>().ToList();

        // Assert - VitalityTrait has StackAllowed=false, so second replaces first
        Assert.That(allTraits, Has.Count.EqualTo(1));
        Assert.That(allTraits[0].BaseMaxHp, Is.EqualTo(200));
    }

    [Test]
    public void GetRequiredTraits_ReturnsOnlyRequiredTraits()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        definition.AddTrait(new VitalityTrait(100), isRequired: true);
        definition.AddTrait(new VitalityTrait(200), isRequired: false);

        // Act
        var requiredTraits = definition.GetRequiredTraits<VitalityTrait>().ToList();

        // Assert - Should return all VitalityTraits since the TYPE is marked as required
        Assert.That(requiredTraits, Has.Count.GreaterThan(0));
    }

    [Test]
    public void RemoveTrait_WithRequiredTrait_ThrowsException()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        definition.AddTrait(new VitalityTrait(100), isRequired: true);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => definition.RemoveTrait<VitalityTrait>());
    }

    [Test]
    public void RemoveTrait_WithNonRequiredTrait_RemovesSuccessfully()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        definition.AddTrait(new VitalityTrait(100), isRequired: false);

        // Act
        definition.RemoveTrait<VitalityTrait>();

        // Assert
        Assert.That(definition.HasTrait<VitalityTrait>(), Is.False);
    }

    [Test]
    public void AddTrait_WithStackAllowedFalse_ReplacesExistingTrait()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        var trait1 = new VitalityTrait(100); // StackAllowed = false by default
        var trait2 = new VitalityTrait(200);

        // Act
        definition.AddTrait(trait1);
        definition.AddTrait(trait2);

        // Assert
        var traits = definition.GetTraits<VitalityTrait>().ToList();
        Assert.That(traits, Has.Count.EqualTo(1), "Should only have one trait since StackAllowed is false");
        Assert.That(traits[0].BaseMaxHp, Is.EqualTo(200), "Should have the last added trait");
    }

    // Test removed: MovableTrait no longer exists
    // Stacking functionality is already tested with VitalityTrait above

    [Test]
    public void TraitCount_ReturnsCorrectCount()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        definition.AddTrait(new VitalityTrait(100));

        // Act
        var count = definition.TraitCount<VitalityTrait>();

        // Assert
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public void AddTrait_WithNonITraitType_ThrowsArgumentException()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        var nonTrait = "not a trait";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => definition.AddTrait(nonTrait));
    }

    [Test]
    public void LegacyFluentAPI_AddTrait_StillWorks()
    {
        // Arrange & Act
        var definition = new BaseGameEntityDefinition("test-entity");
        definition.AddTrait(new VitalityTrait(100));

        // Assert
        Assert.That(definition.HasTrait<VitalityTrait>(), Is.True);
        // MovableTrait removed - no longer exists
    }

    [Test]
    public void LegacyFluentAPI_ReplaceTrait_ReplacesExistingTrait()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        definition.AddTrait(new VitalityTrait(100));

        // Act
        definition.ReplaceTrait(new VitalityTrait(200));

        // Assert
        var trait = definition.GetTrait<VitalityTrait>();
        Assert.That(trait, Is.Not.Null);
        Assert.That(trait!.BaseMaxHp, Is.EqualTo(200));
        Assert.That(definition.TraitCount<VitalityTrait>(), Is.EqualTo(1));
    }

    [Test]
    public void Traits_Property_ReturnsAllTraits()
    {
        // Arrange
        var definition = new BaseGameEntityDefinition("test-entity");
        definition.AddTrait(new VitalityTrait(100));

        // Act
        var allTraits = definition.Traits.ToList();

        // Assert
        Assert.That(allTraits, Has.Count.EqualTo(1)); // Only VitalityTrait now
    }

    [Test]
    public void Constructor_WithDefinitionIdAndCategory_SetsProperties()
    {
        // Arrange & Act
        var category = new Category("weapons");
        var definition = new BaseGameEntityDefinition("sword", category);

        // Assert
        Assert.That(definition.DefinitionId, Is.EqualTo("sword"));
        Assert.That(definition.Category, Is.EqualTo(category));
    }
}
