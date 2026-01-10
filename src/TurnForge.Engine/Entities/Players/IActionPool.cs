using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Players;

public interface IActionPool
{

    public const string FixAmount = "fixamount";
    public const string ByAgentAmount = "byagentamount";


    bool HasEnoughActions(int amount = 1);
    bool HasTargetEnoughActions(EntityId entityId, int amount = 1);
    void ConsumeActions(EntityId entityId, int amount = 1);
    void GrantActions(EntityId entityId, int amount = 1);
    int MaxActions { get; }
    int GetAvailableActions(EntityId entityId);
    void ResetAction();

}


public enum ActionPoolType
{
    ByAgentAmount,
    FixAmount
}

public class ActionPoolTypeExtensions
{
    public static ActionPoolType FromString(string value)
    {
        return value.ToLower() switch
        {
            IActionPool.ByAgentAmount => ActionPoolType.ByAgentAmount,
            IActionPool.FixAmount => ActionPoolType.FixAmount,
            _ => throw new ArgumentOutOfRangeException(nameof(value), $"Not expected action pool type value: {value}"),
        };
    }
}

