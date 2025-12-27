using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Components;

public class BaseActionPointsComponent(int maxActionPoints) : IActionPointsComponent
{
    public BaseActionPointsComponent(TurnForge.Engine.Traits.Standard.ActionPointsTrait trait)
        : this(trait.MaxActionPoints)
    {
    }

    public int CurrentActionPoints { get; set; } = maxActionPoints;
    public int MaxActionPoints { get; set; } = maxActionPoints;

    public void SpendActionPoints(int amount) { 
        if (CurrentActionPoints < amount)
        {
            throw new InvalidOperationException($"Not enough action points requested {amount} remaining {CurrentActionPoints}");
        }
        CurrentActionPoints -= amount;
    }

    public void RestoreActionPoints(int amount) { 
        CurrentActionPoints = Math.Clamp(CurrentActionPoints + amount, 0, MaxActionPoints);
    }

    public void ResetActionPoints() { 
        CurrentActionPoints = MaxActionPoints;
    }

    public void ResetActionPoints(int amount) { 
        // cases where the agent has temporary more action points
        CurrentActionPoints =amount;
    }

    public bool IsEmpty()
    {
        return CurrentActionPoints == 0 && MaxActionPoints == 0;
    }

    public bool CanAfford(int amount)
    {
        return CurrentActionPoints >= amount;
    }

}