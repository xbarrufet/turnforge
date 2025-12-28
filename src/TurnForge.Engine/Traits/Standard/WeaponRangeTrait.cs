using TurnForge.Engine.Traits;

namespace TurnForge.Engine.Traits.Standard;

public class WeaponRangeTrait : BaseTrait
{
    public Dictionary<string, EffectiveRange> Ranges { get; set; } = new();
    
    public bool Melee { get; set; }

    public bool IsMelee => Melee;
    public bool IsRanged => !Melee;

    public WeaponRangeTrait() 
    {
        Melee = true;
    }

    public WeaponRangeTrait(EffectiveRange range, bool melee = false) 
    {
        Ranges = new Dictionary<string, EffectiveRange> { { "Standard", range } };
        Melee = melee;
    }

    public WeaponRangeTrait(int min, int max, bool melee = false)
        : this(new EffectiveRange(max, min), melee) { }

    public WeaponRangeTrait(Dictionary<string, EffectiveRange> ranges, bool melee = false)
    {
        Ranges = ranges;
        Melee = melee;
    }

    public class Builder
    {
        private int _min = 0;
        private int _max = 1;
        private bool _melee = false;

        public Builder Min(int min) { _min = min; return this; }
        public Builder Max(int max) { _max = max; return this; }
        public Builder IsMelee() { _melee = true; return this; }
        public Builder Range(int val) { _min = val; _max = val; return this; }
        public Builder Range(int min, int max) { _min = min; _max = max; return this; }
        
        public WeaponRangeTrait Build() => new(_min, _max, _melee);
    }
}

public record EffectiveRange(int HighLimit, int LowLimit = 0);