using TurnForge.Engine.Services.Dice.Modifiers;

namespace TurnForge.Engine.Tests.Services.Dice;

[TestFixture]
public class KeepLowestModifierTests
{
    [Test]
    public void Apply_KeepTwo_DropsHighest()
    {
        var modifier = new KeepLowestModifier(2);
        var rolls = new List<int> { 3, 6, 1 };
        var random = new Random(42);
        
        var result = modifier.Apply(rolls, 6, random);
        
        Assert.That(result.FinalRolls, Has.Count.EqualTo(2));
        Assert.That(result.FinalRolls, Does.Contain(1));
        Assert.That(result.FinalRolls, Does.Contain(3));
        Assert.That(result.FinalRolls, Does.Not.Contain(6));
    }
    
    [Test]
    public void Apply_KeepAll_KeepsEverything()
    {
        var modifier = new KeepLowestModifier(3);
        var rolls = new List<int> { 3, 6, 1 };
        var random = new Random(42);
        
        var result = modifier.Apply(rolls, 6, random);
        
        Assert.That(result.FinalRolls, Has.Count.EqualTo(3));
    }
    
    [Test]
    public void Apply_TracksHistory()
    {
        var modifier = new KeepLowestModifier(2);
        var rolls = new List<int> { 3, 6, 1 };
        var random = new Random(42);
        
        var result = modifier.Apply(rolls, 6, random);
        
        Assert.That(result.History, Has.Count.EqualTo(3));
        Assert.That(result.History.Count(h => h.Reason == "Kept"), Is.EqualTo(2));
        Assert.That(result.History.Count(h => h.Reason == "Dropped"), Is.EqualTo(1));
    }
}
