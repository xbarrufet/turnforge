using System.ComponentModel;
using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Components;

public class BaseMovementComponent : IMovementComponent
{
    public int MaxUnitsToMove { get; set; } = 4;
    public int CurrentUnitsMoved { get; set; } = 0;

    public BaseMovementComponent() { }

    public BaseMovementComponent(int maxUnits)
    {
        MaxUnitsToMove = maxUnits;
    }

    public bool CanMove(int cost)
    {
        return (CurrentUnitsMoved + cost) <= MaxUnitsToMove;
    }

    public void RegisterMove(int cost)
    {
        CurrentUnitsMoved += cost;
    }
    
    public void ResetTurn()
    {
        CurrentUnitsMoved = 0;
    }
}