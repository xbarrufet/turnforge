using TurnForge.Engine.Services.Dice.ValueObjects;

namespace TurnForge.Engine.Tests.Services.Dice;

[TestFixture]
public class DiceThrowLimitTests
{
    [Test]
    public void Parse_ValidNotation_ParsesCorrectly()
    {
        var limit = DiceThrowLimit.Parse("10+");
        
        Assert.That(limit.Threshold, Is.EqualTo(10));
    }
    
    [Test]
    public void Parse_WithSpaces_ParsesCorrectly()
    {
        var limit = DiceThrowLimit.Parse("  15+  ");
        
        Assert.That(limit.Threshold, Is.EqualTo(15));
    }
    
    [Test]
    public void IsPassed_TotalMeetsThreshold_ReturnsTrue()
    {
        var limit = DiceThrowLimit.Parse("10+");
        
        Assert.That(limit.IsPassed(10), Is.True);
        Assert.That(limit.IsPassed(15), Is.True);
    }
    
    [Test]
    public void IsPassed_TotalBelowThreshold_ReturnsFalse()
    {
        var limit = DiceThrowLimit.Parse("10+");
        
        Assert.That(limit.IsPassed(9), Is.False);
        Assert.That(limit.IsPassed(1), Is.False);
    }
    
    [Test]
    public void ToString_ReturnsNotation()
    {
        var limit = DiceThrowLimit.Parse("10+");
        Assert.That(limit.ToString(), Is.EqualTo("10+"));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Error Cases
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Parse_MissingPlus_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => DiceThrowLimit.Parse("10"));
    }
    
    [Test]
    public void Parse_InvalidNumber_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => DiceThrowLimit.Parse("abc+"));
    }
    
    [Test]
    public void Parse_Empty_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => DiceThrowLimit.Parse(""));
    }
    
    [Test]
    public void TryParse_InvalidNotation_ReturnsFalse()
    {
        var success = DiceThrowLimit.TryParse("invalid", out var result);
        
        Assert.That(success, Is.False);
        Assert.That(result, Is.Null);
    }
}
