

using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.Traits.Standard;

namespace TurnForge.Engine.Entities.TraitsComponents.Components;

/// <summary>
/// Runtime component for managing a Player's action pool.
/// Tracks current actions and handles regeneration based on scope.
/// </summary>
public class ActionPoolComponent : IGameEntityComponent
{
    public int CurrentActionPoints { get; set; }
    public int MaxActionPoints
    {
        get;
        internal set;
    }
    public ActionPoolScope Scope => Trait.Scope;
    public ActionPoolMode Mode => Trait.Mode;
    public ActionPoolTrait Trait { get; init; }
    
    
    public ActionPoolComponent()
    {
        Trait=new ActionPoolTrait();
        IsInitialized = false;
    }
    
    public ActionPoolComponent(ActionPoolTrait trait)
    {
        Trait = trait;
        CurrentActionPoints = trait.BaseAmount;
        MaxActionPoints = trait.BaseAmount;
        IsInitialized = true;
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

    public bool IsInitialized { get; }
}