using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Players;

public class PlayerFactory
{
    public static Player BuildNewPlayer(PlayerId playerId, PlayerControllerType playerControllerType, string team, string name, ActionPoolType actionPoolType, int maxActions)
    {
        var actionPool = ActionPoolFactory.BuildActionPool(actionPoolType, maxActions);
        return new Player(playerId, playerControllerType, team, name, actionPool);
    }

    public static Player BuildNewPlayer(string name, PlayerControllerType playerControllerType, string team, ActionPoolType actionPoolType, int maxActions)
    {
        var actionPool = ActionPoolFactory.BuildActionPool(actionPoolType, maxActions);
        return new Player(PlayerId.New(), playerControllerType, team, name, actionPool);
    }
}