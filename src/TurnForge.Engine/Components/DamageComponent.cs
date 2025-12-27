using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components;

public class DamageComponent : IGameEntityComponent
{
    public PotentialRandomValue Damage { get; }
    public string DamageCategory { get; }

    public DamageComponent(PotentialRandomValue damage, string damageCategory)
    {
        Damage = damage;
        DamageCategory = damageCategory;
    }

    public DamageComponent(DamageTrait trait)
    {
        Damage = trait.Damage;
        DamageCategory = trait.DamageCategory;
    }
}
