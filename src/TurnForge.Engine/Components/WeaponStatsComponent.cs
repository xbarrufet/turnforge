using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components;

public class WeaponStatsComponent : IGameEntityComponent
{
    public int Damage { get; }
    public int HitBonus { get; }
    public int Range { get; }

    public WeaponStatsComponent(int damage, int hitBonus, int range = 1)
    {
        Damage = damage;
        HitBonus = hitBonus;
        Range = range;
    }

    public WeaponStatsComponent(WeaponStatsTrait trait)
    {
        Damage = trait.Damage;
        HitBonus = trait.HitBonus;
        Range = trait.Range;
    }
}
