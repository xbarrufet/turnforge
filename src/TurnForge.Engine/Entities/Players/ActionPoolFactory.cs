namespace TurnForge.Engine.Entities.Players;

public class ActionPoolFactory
{
    public static IActionPool BuildActionPool(ActionPoolType actionPoolType, int maxActions)
    {
        return actionPoolType switch
        {
            ActionPoolType.ByAgentAmount => throw new NotImplementedException(),
            ActionPoolType.FixAmount => new FixedActionPool(maxActions),
            _ => throw new ArgumentOutOfRangeException(nameof(actionPoolType), $"Not expected action pool type value: {actionPoolType}"),
        };
    }
}