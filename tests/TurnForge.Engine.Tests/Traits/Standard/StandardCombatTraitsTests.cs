using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.Traits.Standard.Checkers;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Traits.Standard;

[TestFixture]
public class StandardCombatTraitsTests
{
    [Test]
    public void ToHitTrait_Fixed_ConfiguresCorrectly()
    {
        var trait = new ToHitTrait("1d6", 3);
        
        Assert.That(trait.StatName, Is.EqualTo("ToHit"));
        Assert.That(trait.DicePattern.ToString(), Is.EqualTo("1d6").IgnoreCase);
        Assert.That(trait.Condition, Is.InstanceOf<FixedThreshold>());
        Assert.That(((FixedThreshold)trait.Condition).Value, Is.EqualTo(3));
    }
    
    [Test]
    public void ToHitTrait_Opposed_ConfiguresCorrectly()
    {
        var trait = new ToHitTrait("1d20");
        
        Assert.That(trait.StatName, Is.EqualTo("ToHit"));
        Assert.That(trait.Condition, Is.InstanceOf<OpposedCheck>());
    }
    
    [Test]
    public void ToWoundTrait_Table_ConfiguresCorrectly()
    {
        var trait = new ToWoundTrait("1d6", "CustomWoundTable");
        
        Assert.That(trait.StatName, Is.EqualTo("ToWound"));
        Assert.That(trait.Condition, Is.InstanceOf<TableLookup>());
        Assert.That(((TableLookup)trait.Condition).TableName, Is.EqualTo("CustomWoundTable"));
    }
    
    [Test]
    public void ToWoundTrait_DefaultTable_ConfiguresCorrectly()
    {
        // Should default to "ToWound" table
        var trait = new ToWoundTrait("1d6");
        
        // Note: The constructor with just dice assumes OpposedCheck unless using named arg or the specific constructor.
        // Wait, I overloaded:
        // 1. (dice, threshold) -> Fixed
        // 2. (dice, tableName="ToWound") -> Table
        // 3. (dice) -> Opposed
        
        // Ambiguity check:
        // new ToWoundTrait("1d6") matches (dice, tableName="ToWound") because of optional param?
        // OR matches (dice) for Opposed?
        
        // Let's check the implementation of ToWoundTrait again.
    }
}
