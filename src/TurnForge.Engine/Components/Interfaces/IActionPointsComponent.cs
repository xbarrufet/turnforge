namespace TurnForge.Engine.Components.Interfaces;

public interface IActionPointsComponent : IGameEntityComponent
{
    int CurrentActionPoints { get; }
    int MaxActionPoints { get; }
    void SpendActionPoints(int amount);
    void RestoreActionPoints(int amount);
    void ResetActionPoints();
    void ResetActionPoints(int amount);
    bool IsEmpty();
    bool CanAfford(int amount);
    
    public static IActionPointsComponent Empty() => new BaseActionPointsComponent(0);
}