namespace TurnForge.Engine.Entities.Components.Interfaces;

public interface IMovementComponent : IGameEntityComponent
{
    int MaxUnitsToMove { get; set; }
    int CurrentUnitsMoved { get; set; }
    bool CanMove(int cost);
    void RegisterMove(int cost);
    void ResetTurn();
}
