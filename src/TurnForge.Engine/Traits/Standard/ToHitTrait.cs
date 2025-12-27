using TurnForge.Engine.Traits.Standard.Checkers;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Defines the accuracy/skill required to hit with an attack.
/// StatName: "ToHit"
/// </summary>
public class ToHitTrait : CheckerStatTrait
{
    private const string StatNameConst = "ToHit";

    /// <summary>
    /// Creates a ToHit trait with a fixed success threshold.
    /// Example: Hit on 3+ (dice="1d6", threshold=3)
    /// </summary>
    public ToHitTrait(PotentialRandomValue dice, int fixedThreshold) 
        : base(StatNameConst, dice, new FixedThreshold(fixedThreshold))
    {
    }

    /// <summary>
    /// Creates a ToHit trait that is opposed by a defender's stat.
    /// Example: Hit if roll >= Defender's Defense
    /// </summary>
    public ToHitTrait(PotentialRandomValue dice) 
        : base(StatNameConst, dice, new OpposedCheck())
    {
    }
    
    /// <summary>
    /// Creates a ToHit trait that uses a lookup table.
    /// Example: Hit based on "BallisticSkill" table.
    /// </summary>
    public ToHitTrait(PotentialRandomValue dice, string tableName) 
        : base(StatNameConst, dice, new TableLookup(tableName))
    {
    }
}
