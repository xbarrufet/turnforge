using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Traits.Standard;

namespace TurnForge.Engine.Components;

/// <summary>
/// Runtime component for managing a Player's action pool.
/// Tracks current actions and handles regeneration based on scope.
/// </summary>
public class ActionPoolComponent : IActionPointsComponent
{
    public int CurrentActionPoints { get; set; }
    public int MaxActionPoints { get; set; }
    public ActionPoolScope Scope { get; }
    public ActionPoolMode Mode { get; }

    public ActionPoolComponent(ActionPoolTrait trait)
    {
        MaxActionPoints = trait.BaseAmount;
        CurrentActionPoints = trait.BaseAmount;
        Scope = trait.Scope;
        Mode = trait.Mode;
    }

    public ActionPoolComponent(int baseAmount, ActionPoolScope scope = ActionPoolScope.Turn, ActionPoolMode mode = ActionPoolMode.Fixed)
    {
        MaxActionPoints = baseAmount;
        CurrentActionPoints = baseAmount;
        Scope = scope;
        Mode = mode;
    }

    public void SpendActionPoints(int amount)
    {
        if (CurrentActionPoints < amount)
        {
            throw new InvalidOperationException(
                $"Not enough action points. Requested {amount}, remaining {CurrentActionPoints}");
        }
        CurrentActionPoints -= amount;
    }

    public void RestoreActionPoints(int amount)
    {
        CurrentActionPoints = Math.Clamp(CurrentActionPoints + amount, 0, MaxActionPoints);
    }

    public void ResetActionPoints()
    {
        CurrentActionPoints = MaxActionPoints;
    }

    public void ResetActionPoints(int amount)
    {
        CurrentActionPoints = amount;
    }

    public bool IsEmpty() => CurrentActionPoints == 0;

    public bool CanAfford(int amount) => CurrentActionPoints >= amount;

    /// <summary>
    /// Updates MaxActionPoints based on number of controlled agents.
    /// Only applicable when Mode is PerAgent.
    /// </summary>
    public void RecalculateForAgents(int agentCount)
    {
        if (Mode == ActionPoolMode.PerAgent)
        {
            MaxActionPoints = MaxActionPoints * agentCount;
        }
    }
}
