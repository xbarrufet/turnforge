namespace TurnForge.Engine.Entities.Components.Interfaces;

public interface IHealthComponent : IGameEntityComponent
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    bool IsAlive { get; }
    void TakeDamage(int amount);
    void Heal(int amount);

    public static IHealthComponent Empty()
    {
        return new BaseHealthComponent();
    }
}
