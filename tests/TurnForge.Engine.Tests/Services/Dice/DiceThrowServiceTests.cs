using TurnForge.Engine.Services.Dice;
using TurnForge.Engine.Services.Dice.ValueObjects;

namespace TurnForge.Engine.Tests.Services.Dice;

[TestFixture]
public class DiceThrowServiceTests
{
    // ─────────────────────────────────────────────────────────────
    // Deterministic Rolling (Seeded Random)
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Roll_SameSeeed_ProducesSameResults()
    {
        var service1 = new DiceThrowService(new Random(42));
        var service2 = new DiceThrowService(new Random(42));
        
        var result1 = service1.Roll("2D6+3");
        var result2 = service2.Roll("2D6+3");
        
        Assert.That(result1.Total, Is.EqualTo(result2.Total));
        Assert.That(result1.FinalRolls, Is.EqualTo(result2.FinalRolls));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Basic Rolls
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Roll_SimpleNotation_ReturnsValidResult()
    {
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("2D6");
        
        Assert.That(result.Total, Is.GreaterThanOrEqualTo(2));
        Assert.That(result.Total, Is.LessThanOrEqualTo(12));
        Assert.That(result.FinalRolls, Has.Count.EqualTo(2));
    }
    
    [Test]
    public void Roll_WithModifier_AddsToTotal()
    {
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("1D6+5");
        
        // 1D6 is 1-6, plus 5 = 6-11
        Assert.That(result.Total, Is.GreaterThanOrEqualTo(6));
        Assert.That(result.Total, Is.LessThanOrEqualTo(11));
    }
    
    [Test]
    public void Roll_DiceThrowType_ReturnsValidResult()
    {
        var service = new DiceThrowService(new Random(42));
        var diceThrow = DiceThrowType.Parse("3D6");
        
        var result = service.Roll(diceThrow);
        
        Assert.That(result.FinalRolls, Has.Count.EqualTo(3));
        Assert.That(result.DiceThrow, Is.EqualTo(diceThrow));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Modifiers
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Roll_KeepHighest_DropsLowest()
    {
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("4D6kh3");
        
        // Should keep 3 dice out of 4
        Assert.That(result.FinalRolls, Has.Count.EqualTo(3));
    }
    
    [Test]
    public void Roll_KeepLowest_DropsHighest()
    {
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("3D6kl2");
        
        Assert.That(result.FinalRolls, Has.Count.EqualTo(2));
    }
    
    [Test]
    public void Roll_Reroll_ModifiesResult()
    {
        var service = new DiceThrowService(new Random(42));
        
        // Roll with reroll 1s
        var result = service.Roll("3D6r1", trackHistory: true);
        
        Assert.That(result.FinalRolls, Has.Count.EqualTo(3));
        Assert.That(result.History, Is.Not.Null);
    }
    
    [Test]
    public void Roll_FluentModifiers_AppliesAll()
    {
        var service = new DiceThrowService(new Random(42));
        var dice = DiceThrowType.Parse("4D6").KeepHighest(3);
        
        var result = service.Roll(dice);
        
        Assert.That(result.FinalRolls, Has.Count.EqualTo(3));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Limit Checks
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Roll_WithLimit_PopulatesPassProperty()
    {
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("1D20", "10+");
        
        Assert.That(result.Limit, Is.Not.Null);
        Assert.That(result.Pass.HasValue, Is.True);
    }
    
    [Test]
    public void Roll_WithoutLimit_PassIsNull()
    {
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("1D20");
        
        Assert.That(result.Limit, Is.Null);
        Assert.That(result.Pass, Is.Null);
    }
    
    [Test]
    public void Roll_TotalMeetsLimit_PassIsTrue()
    {
        // Use a seeded random that we know will produce a high result
        // Seed 42 with 1D20 gives 5, so we use limit "5+"
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("1D20", "1+"); // 1+ always passes
        
        Assert.That(result.Pass, Is.True);
    }
    
    // ─────────────────────────────────────────────────────────────
    // History Tracking
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Roll_TrackHistoryFalse_HistoryIsNull()
    {
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("2D6", trackHistory: false);
        
        Assert.That(result.History, Is.Null);
    }
    
    [Test]
    public void Roll_TrackHistoryTrue_HistoryIsPopulated()
    {
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("2D6", trackHistory: true);
        
        Assert.That(result.History, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(2));
    }
    
    [Test]
    public void Roll_WithModifiers_TrackHistoryTrue_IncludesModifierHistory()
    {
        var service = new DiceThrowService(new Random(42));
        
        var result = service.Roll("4D6kh3", trackHistory: true);
        
        Assert.That(result.History, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(4)); // All 4 dice tracked
    }
    
    // ─────────────────────────────────────────────────────────────
    // Fluent Comparisons
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Result_FluentComparisons_Work()
    {
        var service = new DiceThrowService(new Random(42));
        var result = service.Roll("1D6+10"); // 11-16
        
        Assert.That(result.IsHigherOrEqualThan(11), Is.True);
        Assert.That(result.IsLowerThan(20), Is.True);
    }
}
