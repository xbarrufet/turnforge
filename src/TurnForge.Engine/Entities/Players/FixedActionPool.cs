using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Players;

public class FixedActionPool(int maxActions) : IActionPool
{
    private int _availableActions = maxActions;

    public bool HasEnoughActions(int amount = 1)
    {
        return _availableActions > 0;
    }

    public bool HasTargetEnoughActions(EntityId entityId, int amount = 1)
    {
        return HasEnoughActions(amount);
    }

    public void ConsumeActions(EntityId entityId, int amount = 1)
    {
        if (!HasTargetEnoughActions(entityId, amount))
            throw new InvalidOperationException("Not enough available actions to consume.");
        _availableActions -= amount;
    }

    public void GrantActions(EntityId entityId, int amount = 1)
    {
        _availableActions += amount;
    }

    public int MaxActions { get; } = maxActions;

    public int GetAvailableActions(EntityId entityId)
    {
        return _availableActions;
    }

    public void ResetAction()
    {
        _availableActions = MaxActions;
    }
}