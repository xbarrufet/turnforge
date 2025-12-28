using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.Traits.Standard.Checkers;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Traits.Standard;

[TestFixture]
public class TraitBuildersTests
{
    [Test]
    public void ToHitTrait_Builder_CreatesCorrectTrait()
    {
        var trait = new ToHitTrait.Builder()
            .Dice("2d6")
            .On(4)
            .Scope(new AnyOfThem())
            .Build();
            
        Assert.That(trait.StatName, Is.EqualTo("ToHit"));
        Assert.That(trait.DicePattern.ToString(), Is.EqualTo("2D6").IgnoreCase);
        Assert.That(trait.Condition, Is.InstanceOf<FixedThreshold>());
        Assert.That(((FixedThreshold)trait.Condition).Value, Is.EqualTo(4));
        Assert.That(trait.CheckScope, Is.InstanceOf<AnyOfThem>());
    }

    [Test]
    public void ToWoundTrait_Builder_Opposed_CreatesCorrectTrait()
    {
        var trait = new ToWoundTrait.Builder()
            .Dice("1d6")
            .VsSomething()
            .Build();
            
        Assert.That(trait.StatName, Is.EqualTo("ToWound"));
        Assert.That(trait.Condition, Is.InstanceOf<OpposedCheck>());
    }

    [Test]
    public void WeaponRangeTrait_Builder_CreatesCorrectTrait()
    {
        var trait = new WeaponRangeTrait.Builder()
            .Range(0, 1) // Min 0, Max 1
            .IsMelee()
            .Build();
            
        Assert.That(trait.IsMelee, Is.True);
        Assert.That(trait.Ranges["Standard"].LowLimit, Is.EqualTo(0));
        Assert.That(trait.Ranges["Standard"].HighLimit, Is.EqualTo(1));
    }
    
    [Test]
    public void WeaponRangeTrait_Builder_SingleVal_CreatesCorrectTrait()
    {
        var trait = new WeaponRangeTrait.Builder()
            .Range(5) // Min 5, Max 5
            .Build();
            
        Assert.That(trait.Ranges["Standard"].LowLimit, Is.EqualTo(5));
        Assert.That(trait.Ranges["Standard"].HighLimit, Is.EqualTo(5));
    }

    [Test]
    public void DamageTrait_Builder_CreatesCorrectTrait()
    {
        var trait = new DamageTrait.Builder()
            .WithProfile("Primary", 2, "Physical")
            .WithProfile("Fire", "1d6", "Fire")
            .Build();
            
        Assert.That(trait.Profiles.Count, Is.EqualTo(2));
        Assert.That(trait.Profiles[0].Name, Is.EqualTo("Primary"));
        Assert.That(trait.Profiles[1].Category, Is.EqualTo("Fire"));
    }
    
    [Test]
    public void IdentityTrait_Builder_CreatesCorrectTrait()
    {
        var trait = new IdentityTrait.Builder()
            .Category("Hero")
            .Build();
            
        Assert.That(trait.Category, Is.EqualTo("Hero"));
    }
}
