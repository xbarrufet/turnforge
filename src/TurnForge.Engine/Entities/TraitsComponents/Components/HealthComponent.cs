using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Traits;

namespace TurnForge.Engine.Entities.TraitsComponents.Components;

public sealed class HealthComponent : IGameEntityComponent
{
    public int CurrentHealth { get; set; }
    public VitalityTrait Trait { get; set; }

    public bool IsAlive => CurrentHealth > 0;
    public bool IsInitialized { get; init; }

    // empty constructor needed for automapper
    public HealthComponent()
    {
        VitalityTrait vitalityTrait = new VitalityTrait();
        IsInitialized = false;
    }
    public static HealthComponent Empty = new HealthComponent();

    public HealthComponent(VitalityTrait trait)
    {
        Trait = trait;
        CurrentHealth = trait.BaseMaxHp;
        IsInitialized = true;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth = Math.Max(0, CurrentHealth - amount);
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth = Math.Min(Trait!.BaseMaxHp, CurrentHealth + amount);
    }

}
