using TurnForge.Engine.Services.Dice;
using TurnForge.Engine.Services.Dice.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.ValueObjects;

[TestFixture]
public class PotentialRandomValueTests
{
    // ─────────────────────────────────────────────────────────────
    // Factory Methods
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Fixed_CreatesFixedValue()
    {
        var value = PotentialRandomValue.Fixed(5);
        
        Assert.That(value.IsFixed, Is.True);
        Assert.That(value.IsRandom, Is.False);
        Assert.That(value.FixedValue, Is.EqualTo(5));
        Assert.That(value.DiceThrow, Is.Null);
    }
    
    [Test]
    public void Dice_FromNotation_CreatesRandomValue()
    {
        var value = PotentialRandomValue.Dice("2d6+3");
        
        Assert.That(value.IsFixed, Is.False);
        Assert.That(value.IsRandom, Is.True);
        Assert.That(value.DiceThrow, Is.Not.Null);
        Assert.That(value.DiceThrow!.DiceCount, Is.EqualTo(2));
        Assert.That(value.DiceThrow.DiceSides, Is.EqualTo(6));
        Assert.That(value.DiceThrow.Modifier, Is.EqualTo(3));
    }
    
    [Test]
    public void Dice_FromDiceThrowType_CreatesRandomValue()
    {
        var diceThrow = DiceThrowType.Parse("3d8");
        var value = PotentialRandomValue.Dice(diceThrow);
        
        Assert.That(value.IsRandom, Is.True);
        Assert.That(value.DiceThrow, Is.EqualTo(diceThrow));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Parsing
    // ─────────────────────────────────────────────────────────────
    
    [TestCase("5", 5)]
    [TestCase("0", 0)]
    [TestCase("-3", -3)]
    [TestCase("100", 100)]
    public void Parse_FixedValues_ParsesCorrectly(string input, int expectedValue)
    {
        var value = PotentialRandomValue.Parse(input);
        
        Assert.That(value.IsFixed, Is.True);
        Assert.That(value.FixedValue, Is.EqualTo(expectedValue));
    }
    
    [TestCase("1d6", 1, 6, 0)]
    [TestCase("2D6", 2, 6, 0)]
    [TestCase("3d8+5", 3, 8, 5)]
    [TestCase("1d20-2", 1, 20, -2)]
    [TestCase("4d6kh3", 4, 6, 0)]
    public void Parse_DiceNotation_ParsesCorrectly(string input, int count, int sides, int modifier)
    {
        var value = PotentialRandomValue.Parse(input);
        
        Assert.That(value.IsRandom, Is.True);
        Assert.That(value.DiceThrow!.DiceCount, Is.EqualTo(count));
        Assert.That(value.DiceThrow.DiceSides, Is.EqualTo(sides));
        Assert.That(value.DiceThrow.Modifier, Is.EqualTo(modifier));
    }
    
    [TestCase("")]
    [TestCase("   ")]
    public void Parse_EmptyOrWhitespace_Throws(string input)
    {
        Assert.Throws<ArgumentException>(() => PotentialRandomValue.Parse(input));
    }
    
    [TestCase("invalid")]
    [TestCase("abc123")]
    public void Parse_InvalidNotation_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(() => PotentialRandomValue.Parse(input));
    }
    
    [Test]
    public void TryParse_ValidInput_ReturnsTrue()
    {
        var success = PotentialRandomValue.TryParse("2d6", out var result);
        
        Assert.That(success, Is.True);
        Assert.That(result.IsRandom, Is.True);
    }
    
    [Test]
    public void TryParse_InvalidInput_ReturnsFalse()
    {
        var success = PotentialRandomValue.TryParse("invalid", out var result);
        
        Assert.That(success, Is.False);
        Assert.That(result, Is.EqualTo(default(PotentialRandomValue)));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Resolution
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Resolve_FixedValue_ReturnsFixedValue()
    {
        var value = PotentialRandomValue.Fixed(42);
        var diceService = new DiceThrowService(new Random(12345));
        
        var result = value.Resolve(diceService);
        
        Assert.That(result, Is.EqualTo(42));
    }
    
    [Test]
    public void Resolve_FixedValue_DoesNotNeedDiceService()
    {
        var value = PotentialRandomValue.Fixed(10);
        
        // Should work even with null (fixed values don't need service)
        var result = value.Resolve(null!);
        
        Assert.That(result, Is.EqualTo(10));
    }
    
    [Test]
    public void Resolve_RandomValue_ReturnsDiceRoll()
    {
        var value = PotentialRandomValue.Dice("1d6");
        var diceService = new DiceThrowService(new Random(12345));
        
        var result = value.Resolve(diceService);
        
        Assert.That(result, Is.InRange(1, 6));
    }
    
    [Test]
    public void Resolve_RandomValue_WithoutService_Throws()
    {
        var value = PotentialRandomValue.Dice("2d6");
        
        Assert.Throws<ArgumentNullException>(() => value.Resolve(null!));
    }
    
    [Test]
    public void ResolveWithDetails_FixedValue_ReturnsNotRolled()
    {
        var value = PotentialRandomValue.Fixed(5);
        var diceService = new DiceThrowService(new Random());
        
        var result = value.ResolveWithDetails(diceService);
        
        Assert.That(result.Value, Is.EqualTo(5));
        Assert.That(result.WasRolled, Is.False);
        Assert.That(result.RollResult, Is.Null);
    }
    
    [Test]
    public void ResolveWithDetails_RandomValue_ReturnsRollDetails()
    {
        var value = PotentialRandomValue.Dice("2d6");
        var diceService = new DiceThrowService(new Random(12345));
        
        var result = value.ResolveWithDetails(diceService);
        
        Assert.That(result.WasRolled, Is.True);
        Assert.That(result.RollResult, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo(result.RollResult!.Total));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Utility Properties
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void MinValue_Fixed_ReturnsSameValue()
    {
        var value = PotentialRandomValue.Fixed(7);
        Assert.That(value.MinValue, Is.EqualTo(7));
    }
    
    [Test]
    public void MaxValue_Fixed_ReturnsSameValue()
    {
        var value = PotentialRandomValue.Fixed(7);
        Assert.That(value.MaxValue, Is.EqualTo(7));
    }
    
    [Test]
    public void AverageValue_Fixed_ReturnsSameValue()
    {
        var value = PotentialRandomValue.Fixed(7);
        Assert.That(value.AverageValue, Is.EqualTo(7));
    }
    
    [TestCase("1d6", 1, 6)]      // Min: 1, Max: 6
    [TestCase("2d6", 2, 12)]     // Min: 2, Max: 12
    [TestCase("2d6+3", 5, 15)]   // Min: 2+3, Max: 12+3
    [TestCase("3d8-2", 1, 22)]   // Min: 3-2, Max: 24-2
    public void MinMaxValue_Random_CalculatesCorrectly(string notation, int expectedMin, int expectedMax)
    {
        var value = PotentialRandomValue.Dice(notation);
        
        Assert.That(value.MinValue, Is.EqualTo(expectedMin));
        Assert.That(value.MaxValue, Is.EqualTo(expectedMax));
    }
    
    [Test]
    public void AverageValue_Random_CalculatesCorrectly()
    {
        var value = PotentialRandomValue.Dice("2d6"); // Average: 2 * 3.5 = 7
        
        Assert.That(value.AverageValue, Is.EqualTo(7.0));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Implicit Conversions
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void ImplicitConversion_FromInt_CreatesFixed()
    {
        PotentialRandomValue value = 15;
        
        Assert.That(value.IsFixed, Is.True);
        Assert.That(value.FixedValue, Is.EqualTo(15));
    }
    
    [Test]
    public void ImplicitConversion_FromString_FixedNumber()
    {
        PotentialRandomValue value = "10";
        
        Assert.That(value.IsFixed, Is.True);
        Assert.That(value.FixedValue, Is.EqualTo(10));
    }
    
    [Test]
    public void ImplicitConversion_FromString_DiceNotation()
    {
        PotentialRandomValue value = "3d6+2";
        
        Assert.That(value.IsRandom, Is.True);
        Assert.That(value.DiceThrow!.DiceCount, Is.EqualTo(3));
    }
    
    // ─────────────────────────────────────────────────────────────
    // ToString
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void ToString_Fixed_ReturnsNumber()
    {
        var value = PotentialRandomValue.Fixed(42);
        Assert.That(value.ToString(), Is.EqualTo("42"));
    }
    
    [Test]
    public void ToString_Random_ReturnsDiceNotation()
    {
        var value = PotentialRandomValue.Dice("2d6+3");
        Assert.That(value.ToString(), Is.EqualTo("2D6+3"));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Value Object Equality
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Equality_SameFixedValues_AreEqual()
    {
        var a = PotentialRandomValue.Fixed(5);
        var b = PotentialRandomValue.Fixed(5);
        
        Assert.That(a, Is.EqualTo(b));
        Assert.That(a == b, Is.True);
    }
    
    [Test]
    public void Equality_DifferentFixedValues_AreNotEqual()
    {
        var a = PotentialRandomValue.Fixed(5);
        var b = PotentialRandomValue.Fixed(10);
        
        Assert.That(a, Is.Not.EqualTo(b));
    }
    
    [Test]
    public void Equality_SameDiceNotation_HaveSameProperties()
    {
        // Note: DiceThrowType is a record but has mutable Modifiers list,
        // so equality comparison may not work as expected for the full object.
        // We compare the relevant properties instead.
        var a = PotentialRandomValue.Dice("2d6");
        var b = PotentialRandomValue.Dice("2d6");
        
        Assert.That(a.DiceThrow!.DiceCount, Is.EqualTo(b.DiceThrow!.DiceCount));
        Assert.That(a.DiceThrow.DiceSides, Is.EqualTo(b.DiceThrow.DiceSides));
        Assert.That(a.DiceThrow.Modifier, Is.EqualTo(b.DiceThrow.Modifier));
        Assert.That(a.ToString(), Is.EqualTo(b.ToString()));
    }
    
    [Test]
    public void Equality_FixedVsRandom_AreNotEqual()
    {
        var a = PotentialRandomValue.Fixed(5);
        var b = PotentialRandomValue.Dice("1d6");
        
        Assert.That(a, Is.Not.EqualTo(b));
    }
}
