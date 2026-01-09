using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities.Traits.Standard;
using TurnForge.Engine.Traits.Standard;

namespace TurnForge.Engine.Components;

public sealed class HealthComponent: IGameEntityComponent
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }

    public bool IsAlive => CurrentHealth > 0;
    
    // Default constructor for AutoMapper/Factory
    public HealthComponent()
    {
        MaxHealth = 1;
        CurrentHealth = 1;
    }

    public HealthComponent(VitalityTrait trait)
{
    MaxHealth = trait.BaseMaxHp;
    CurrentHealth = trait.BaseMaxHp;
    // Si tinguéssim propietat IsImmortal al component, l'assignaríem aquí també.
}

    public HealthComponent(int maxHealth)
    {
        if (maxHealth <= 0) throw new ArgumentOutOfRangeException(nameof(maxHealth));
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth = Math.Max(0, CurrentHealth - amount);
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
    }
}
