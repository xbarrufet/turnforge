using TurnForge.Engine.Components;

namespace TurnForge.Engine.Traits.Standard;

public class WeaponStatsTrait : BaseComponentTrait<WeaponStatsComponent>
{
    public int Damage { get; set; }
    public int HitBonus { get; set; }
    public int Range { get; set; } = 1;

    public WeaponStatsTrait() { }

    public WeaponStatsTrait(int damage, int hitBonus, int range = 1)
    {
        Damage = damage;
        HitBonus = hitBonus;
        Range = range;
    }
}
