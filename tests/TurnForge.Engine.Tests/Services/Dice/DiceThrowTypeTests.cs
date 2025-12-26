using TurnForge.Engine.Services.Dice.ValueObjects;

namespace TurnForge.Engine.Tests.Services.Dice;

[TestFixture]
public class DiceThrowTypeTests
{
    // ─────────────────────────────────────────────────────────────
    // Basic Parsing
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Parse_SimpleNotation_ParsesCorrectly()
    {
        var result = DiceThrowType.Parse("2D6");
        
        Assert.That(result.DiceCount, Is.EqualTo(2));
        Assert.That(result.DiceSides, Is.EqualTo(6));
        Assert.That(result.Modifier, Is.EqualTo(0));
        Assert.That(result.Modifiers, Is.Empty);
    }
    
    [Test]
    public void Parse_SingleDie_DefaultsToOne()
    {
        var result = DiceThrowType.Parse("D20");
        
        Assert.That(result.DiceCount, Is.EqualTo(1));
        Assert.That(result.DiceSides, Is.EqualTo(20));
    }
    
    [Test]
    public void Parse_WithPositiveModifier_ParsesCorrectly()
    {
        var result = DiceThrowType.Parse("3D6+5");
        
        Assert.That(result.DiceCount, Is.EqualTo(3));
        Assert.That(result.DiceSides, Is.EqualTo(6));
        Assert.That(result.Modifier, Is.EqualTo(5));
    }
    
    [Test]
    public void Parse_WithNegativeModifier_ParsesCorrectly()
    {
        var result = DiceThrowType.Parse("2D4-1");
        
        Assert.That(result.Modifier, Is.EqualTo(-1));
    }
    
    [Test]
    public void Parse_LowercaseD_ParsesCorrectly()
    {
        var result = DiceThrowType.Parse("2d6");
        
        Assert.That(result.DiceCount, Is.EqualTo(2));
        Assert.That(result.DiceSides, Is.EqualTo(6));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Modifier Parsing
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Parse_KeepHighest_AddsModifier()
    {
        var result = DiceThrowType.Parse("4D6kh3");
        
        Assert.That(result.DiceCount, Is.EqualTo(4));
        Assert.That(result.Modifiers, Has.Count.EqualTo(1));
        Assert.That(result.Modifiers[0], Is.TypeOf<TurnForge.Engine.Services.Dice.Modifiers.KeepHighestModifier>());
    }
    
    [Test]
    public void Parse_KeepLowest_AddsModifier()
    {
        var result = DiceThrowType.Parse("3D6kl2");
        
        Assert.That(result.Modifiers, Has.Count.EqualTo(1));
        Assert.That(result.Modifiers[0], Is.TypeOf<TurnForge.Engine.Services.Dice.Modifiers.KeepLowestModifier>());
    }
    
    [Test]
    public void Parse_Reroll_AddsModifier()
    {
        var result = DiceThrowType.Parse("2D6r1");
        
        Assert.That(result.Modifiers, Has.Count.EqualTo(1));
        Assert.That(result.Modifiers[0], Is.TypeOf<TurnForge.Engine.Services.Dice.Modifiers.RerollModifier>());
    }
    
    [Test]
    public void Parse_CombinedNotation_ParsesAll()
    {
        var result = DiceThrowType.Parse("4D6kh3+2");
        
        Assert.That(result.DiceCount, Is.EqualTo(4));
        Assert.That(result.DiceSides, Is.EqualTo(6));
        Assert.That(result.Modifier, Is.EqualTo(2));
        Assert.That(result.Modifiers, Has.Count.EqualTo(1));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Fluent Builders
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void KeepHighest_Fluent_AddsModifier()
    {
        var dice = DiceThrowType.Parse("4D6").KeepHighest(3);
        
        Assert.That(dice.Modifiers, Has.Count.EqualTo(1));
        Assert.That(dice.Modifiers[0], Is.TypeOf<TurnForge.Engine.Services.Dice.Modifiers.KeepHighestModifier>());
    }
    
    [Test]
    public void Reroll_Fluent_AddsModifier()
    {
        var dice = DiceThrowType.Parse("2D6").Reroll(1, maxTimes: 2);
        
        Assert.That(dice.Modifiers, Has.Count.EqualTo(1));
        var reroll = (TurnForge.Engine.Services.Dice.Modifiers.RerollModifier)dice.Modifiers[0];
        Assert.That(reroll.Threshold, Is.EqualTo(1));
        Assert.That(reroll.MaxTimes, Is.EqualTo(2));
    }
    
    // ─────────────────────────────────────────────────────────────
    // ToString
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void ToString_ReturnsNotation()
    {
        var dice = DiceThrowType.Parse("3D6+5");
        Assert.That(dice.ToString(), Is.EqualTo("3D6+5"));
    }
    
    [Test]
    public void ToString_WithModifiers_IncludesAll()
    {
        var dice = DiceThrowType.Parse("4D6kh3+2");
        Assert.That(dice.ToString(), Is.EqualTo("4D6kh3+2"));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Error Cases
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Parse_InvalidNotation_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => DiceThrowType.Parse("invalid"));
    }
    
    [Test]
    public void Parse_EmptyString_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => DiceThrowType.Parse(""));
    }
    
    [Test]
    public void TryParse_InvalidNotation_ReturnsFalse()
    {
        var success = DiceThrowType.TryParse("invalid", out var result);
        
        Assert.That(success, Is.False);
        Assert.That(result, Is.Null);
    }
}
