namespace TurnForge.Engine.Entities.Actors.Components;

public sealed class HealthComponent
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; }

    public bool IsAlive => CurrentHealth > 0;

    public HealthComponent(int maxHealth)
    {
        if (maxHealth <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxHealth));

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
