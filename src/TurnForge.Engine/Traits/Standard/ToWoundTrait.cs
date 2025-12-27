using TurnForge.Engine.Traits.Standard.Checkers;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Defines the ability to wound a target after hitting.
/// StatName: "ToWound"
/// </summary>
public class ToWoundTrait : CheckerStatTrait
{
    private const string StatNameConst = "ToWound";

    /// <summary>
    /// Creates a ToWound trait with a fixed success threshold.
    /// Example: Wound on 4+ (dice="1d6", threshold=4)
    /// </summary>
    public ToWoundTrait(PotentialRandomValue dice, int fixedThreshold) 
        : base(StatNameConst, dice, new FixedThreshold(fixedThreshold))
    {
    }

    /// <summary>
    /// Creates a ToWound trait that uses a lookup table (standard for many wargames).
    /// Example: Strength vs Toughness lookup on "ToWound" table.
    /// </summary>
    public ToWoundTrait(PotentialRandomValue dice, string tableName) 
        : base(StatNameConst, dice, new TableLookup(tableName))
    {
    }
    
    /// <summary>
    /// Creates a ToWound trait that is immediately opposed by a defender's stat.
    /// </summary>
    public ToWoundTrait(PotentialRandomValue dice) 
        : base(StatNameConst, dice, new OpposedCheck())
    {
    }
}
