using TurnForge.Engine.Services.Dice.Modifiers;

namespace TurnForge.Engine.Tests.Services.Dice;

[TestFixture]
public class RerollModifierTests
{
    [Test]
    public void Apply_ValueAboveThreshold_NoReroll()
    {
        var modifier = new RerollModifier(1);
        var rolls = new List<int> { 5, 6 };
        var random = new Random(42);
        
        var result = modifier.Apply(rolls, 6, random);
        
        Assert.That(result.FinalRolls, Is.EqualTo(new[] { 5, 6 }));
        Assert.That(result.History.All(h => h.Reason == "Kept"), Is.True);
    }
    
    [Test]
    public void Apply_ValueAtThreshold_Rerolls()
    {
        // Use a seeded random to get predictable results
        var modifier = new RerollModifier(1);
        var rolls = new List<int> { 1 };
        var random = new Random(42);
        
        var result = modifier.Apply(rolls, 6, random);
        
        // Should have rerolled the 1
        Assert.That(result.FinalRolls, Has.Count.EqualTo(1));
        Assert.That(result.History[0].OriginalValue, Is.EqualTo(1));
        Assert.That(result.History[0].Reason, Does.StartWith("Rerolled"));
    }
    
    [Test]
    public void Apply_RerollsOnce_ByDefault()
    {
        // Even if reroll result is still low, only reroll once by default
        var modifier = new RerollModifier(2, MaxTimes: 1);
        var rolls = new List<int> { 1 };
        var random = new Random(42);
        
        var result = modifier.Apply(rolls, 6, random);
        
        // Whatever the result, it should only have rerolled once
        Assert.That(result.History, Has.Count.EqualTo(1));
    }
    
    [Test]
    public void Apply_RerollsMultipleTimes_WhenConfigured()
    {
        // With maxTimes > 1, keeps rerolling while below threshold
        var modifier = new RerollModifier(3, MaxTimes: 5);
        var rolls = new List<int> { 1 };
        var random = new Random(42);
        
        var result = modifier.Apply(rolls, 6, random);
        
        // Should have attempted rerolls
        Assert.That(result.FinalRolls, Has.Count.EqualTo(1));
    }
    
    [Test]
    public void Apply_InvalidThreshold_Throws()
    {
        var modifier = new RerollModifier(0);
        
        Assert.Throws<ArgumentException>(() => 
            modifier.Apply(new List<int> { 1 }, 6, new Random()));
    }
}
