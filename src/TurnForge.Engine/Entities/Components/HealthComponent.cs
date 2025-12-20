using TurnForge.Engine.Entities.Components.Definitions;
using TurnForge.Engine.Entities.Components.Interfaces;

namespace TurnForge.Engine.Entities.Components;

public sealed class HealthComponent : IGameEntityComponent
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; }

    public bool IsAlive => CurrentHealth > 0;
    public HealhtComponentDefinition Definition { get; }
    
    public HealthComponent(HealhtComponentDefinition definition)
    {
        if (definition == null) throw new ArgumentNullException(nameof(definition));
        if (definition.MaxHealth <= 0) throw new ArgumentOutOfRangeException(nameof(definition.MaxHealth));

        MaxHealth = definition.MaxHealth;
        CurrentHealth = definition.MaxHealth;
        Definition= definition;
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
