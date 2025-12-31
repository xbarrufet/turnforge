namespace TurnForge.Engine.Values;

/// <summary>
/// Represents a single attribute statistic (e.g., Strength, Damage).
/// Can hold a fixed integer value OR a dynamic dice formula.
/// </summary>
public record struct AttributeValue
{
    /// <summary>
    /// The base, unmodified value of the attribute.
    /// </summary>
    public int BaseValue { get; init; } // int? ? 

    /// <summary>
    /// The current effective value of the attribute (e.g. after damage/buffs).
    /// </summary>
    public int CurrentValue { get; init; }

    /// <summary>
    /// Optional dice formula for this attribute (e.g. "1D6" for Damage).
    /// If present, this attribute acts as a generator/formula rather than a static stat.
    /// </summary>
    public DiceThrowType? Dice { get; init; }

    /// <summary>
    /// Creates a numeric attribute.
    /// </summary>
    public AttributeValue(int value) 
    {
        BaseValue = value;
        CurrentValue = value;
        Dice = null;
    }

    /// <summary>
    /// Creates a dice-based attribute.
    /// </summary>
    public AttributeValue(DiceThrowType dice)
    {
        BaseValue = 0; 
        CurrentValue = 0; 
        Dice = dice;
    }

    /// <summary>
    /// Creates a modified copy with a new CurrentValue.
    /// </summary>
    public AttributeValue WithCurrent(int newCurrent) => this with { CurrentValue = newCurrent };
    
    public bool IsDiceAttribute => Dice != null;
    
    public override string ToString()
    {
        return IsDiceAttribute ? Dice!.ToString() : CurrentValue.ToString();
    }
}
