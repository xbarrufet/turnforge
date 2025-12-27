using TurnForge.Engine.Services.Dice;
using TurnForge.Engine.Services.DiceCheck;
using TurnForge.Engine.Services.Rulebook;
using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.Traits.Standard.Checkers;
using Moq;

namespace TurnForge.Engine.Tests.Services.DiceCheck;

[TestFixture]
public class DiceCheckServiceTests
{
    private DiceCheckService _service = null!;
    private IDiceThrowService _diceService = null!;
    
    [SetUp]
    public void SetUp()
    {
        // Use seeded random for deterministic tests
        _diceService = new DiceThrowService(new Random(12345));
        _service = new DiceCheckService(_diceService);
    }
    
    // ─────────────────────────────────────────────────────────────
    // Fixed Threshold Tests
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Check_FixedThreshold_RollMeetsThreshold_Succeeds()
    {
        // 1d6 with seed 12345 rolls 1 (fails 4+)
        // Let's use a fixed value to test the logic
        var stat = CheckerStatTrait.Fixed("ToHit", 5, 4); // Fixed roll of 5, threshold 4+
        
        var result = _service.Check(stat, new CheckParams());
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.RollValue, Is.EqualTo(5));
        Assert.That(result.RequiredThreshold, Is.EqualTo(4));
        Assert.That(result.StatName, Is.EqualTo("ToHit"));
        Assert.That(result.Margin, Is.EqualTo(1)); // 5 - 4 = 1
    }
    
    [Test]
    public void Check_FixedThreshold_RollBelowThreshold_Fails()
    {
        var stat = CheckerStatTrait.Fixed("ToHit", 3, 4); // Fixed roll of 3, threshold 4+
        
        var result = _service.Check(stat, new CheckParams());
        
        Assert.That(result.Success, Is.False);
        Assert.That(result.RollValue, Is.EqualTo(3));
        Assert.That(result.RequiredThreshold, Is.EqualTo(4));
        Assert.That(result.Margin, Is.EqualTo(-1)); // 3 - 4 = -1
    }
    
    [Test]
    public void Check_FixedThreshold_RollEqualsThreshold_Succeeds()
    {
        var stat = CheckerStatTrait.Fixed("ToHit", 4, 4); // Exactly meets threshold
        
        var result = _service.Check(stat, new CheckParams());
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.Margin, Is.EqualTo(0));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Opposed Check Tests
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Check_OpposedCheck_RollBeatsOpponent_Succeeds()
    {
        var stat = CheckerStatTrait.Opposed("Leadership", 10); // Fixed roll of 10
        var parameters = new CheckParams(OpposedValue: 7);
        
        var result = _service.Check(stat, parameters);
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.RollValue, Is.EqualTo(10));
        Assert.That(result.RequiredThreshold, Is.EqualTo(7));
    }
    
    [Test]
    public void Check_OpposedCheck_RollLosesToOpponent_Fails()
    {
        var stat = CheckerStatTrait.Opposed("Leadership", 5); // Fixed roll of 5
        var parameters = new CheckParams(OpposedValue: 7);
        
        var result = _service.Check(stat, parameters);
        
        Assert.That(result.Success, Is.False);
    }
    
    [Test]
    public void Check_OpposedCheck_MissingParameter_Throws()
    {
        var stat = CheckerStatTrait.Opposed("Leadership", 10);
        
        Assert.Throws<ArgumentException>(() => 
            _service.Check(stat, new CheckParams())
        );
    }
    
    // ─────────────────────────────────────────────────────────────
    // Table Lookup Tests
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Check_TableLookup_UsesRulebookService()
    {
        var mockRulebook = new Mock<IRulebookService>();
        mockRulebook
            .Setup(r => r.GetThreshold("ToWound", 4, 3))
            .Returns(3); // S4 vs T3 = need 3+
        
        var stat = CheckerStatTrait.Table("ToWound", 5, "ToWound"); // Fixed roll of 5
        var parameters = new CheckParams(
            AttackerValue: 4,
            DefenderValue: 3,
            Rulebook: mockRulebook.Object
        );
        
        var result = _service.Check(stat, parameters);
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.RequiredThreshold, Is.EqualTo(3));
        mockRulebook.Verify(r => r.GetThreshold("ToWound", 4, 3), Times.Once);
    }
    
    [Test]
    public void Check_TableLookup_MissingRulebook_Throws()
    {
        var stat = CheckerStatTrait.Table("ToWound", 5, "ToWound");
        var parameters = new CheckParams(AttackerValue: 4, DefenderValue: 3);
        
        Assert.Throws<ArgumentException>(() => 
            _service.Check(stat, parameters)
        );
    }
    
    [Test]
    public void Check_TableLookup_SingleValue_Works()
    {
        var mockRulebook = new Mock<IRulebookService>();
        mockRulebook
            .Setup(r => r.GetThreshold("Leadership", 7))
            .Returns(7);
        
        var stat = CheckerStatTrait.Table("Morale", 8, "Leadership");
        var parameters = new CheckParams(
            AttackerValue: 7,
            Rulebook: mockRulebook.Object
        );
        
        var result = _service.Check(stat, parameters);
        
        Assert.That(result.Success, Is.True);
    }
    
    // ─────────────────────────────────────────────────────────────
    // Random Dice Tests
    // ─────────────────────────────────────────────────────────────
    
    [Test]
    public void Check_WithRandomDice_RollsCorrectly()
    {
        var stat = new CheckerStatTrait("ToHit", "1d6", new FixedThreshold(4));
        
        var result = _service.Check(stat, new CheckParams());
        
        // Roll should be between 1-6
        Assert.That(result.RollValue, Is.InRange(1, 6));
    }
    
    [Test]
    public void Check_WithDiceModifier_AppliesModifier()
    {
        // 1d6+2 should give 3-8
        var stat = new CheckerStatTrait("ToHit", "1d6+2", new FixedThreshold(4));
        
        var result = _service.Check(stat, new CheckParams());
        
        Assert.That(result.RollValue, Is.InRange(3, 8));
    }
}
