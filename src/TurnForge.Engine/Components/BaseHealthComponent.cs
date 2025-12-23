using TurnForge.Engine.Entities.Components.Interfaces;

namespace TurnForge.Engine.Entities.Components;

public sealed class BaseHealthComponent : IHealthComponent
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }

    public bool IsAlive => CurrentHealth > 0;
    
    // Default constructor for AutoMapper/Factory
    public BaseHealthComponent()
    {
        MaxHealth = 1;
        CurrentHealth = 1;
    }

    public BaseHealthComponent(int maxHealth)
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
