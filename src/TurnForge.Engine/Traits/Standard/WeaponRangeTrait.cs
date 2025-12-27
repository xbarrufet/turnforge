using TurnForge.Engine.Traits;

namespace TurnForge.Engine.Traits.Standard;

public class WeaponRangeTrait : BaseTrait
{
    public Dictionary<string, EffectiveRange> Ranges { get; set; }
    
    public bool Melee { get; set; }

    public bool IsMelee => Melee;
    public bool IsRanged => !Melee;

    
    public WeaponRangeTrait() {
        Ranges = new Dictionary<string, EffectiveRange>();
        Melee = true;
    }

    public WeaponRangeTrait(EffectiveRange range, bool melee=false) {
        Ranges = new Dictionary<string, EffectiveRange> { { "", range } };
        Melee = melee;
    }
    public WeaponRangeTrait(Dictionary<string, EffectiveRange> ranges, bool melee=false)
    {
        Ranges = ranges;
        Melee = melee;
    }


}

public record EffectiveRange(int HighLimit, int LowLimit=0)
{
    public int LowLimit { get; init; }
    public int HighLimit { get; init; }
}