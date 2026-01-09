using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Commands.StartGame.Action.Inputs;

namespace TurnForge.Engine.Commands.StartGame.Action;

using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

public static class StartGameAction
{
    public static IAction Create(IBoardFactory boardFactory, IEntityApplier entityApplier)
    {
        var processPlayer = new ProcessPlayerDataNode();
        var processBoard = new ProcessBoardDataNode(boardFactory);
        var deployEntities = new DeployEntitiesNode(new NodeId("StartGame.DeployEntities"), entityApplier);

        return ActionBuilder.Create("StartGame")
                .AddNode(processPlayer)
                .AddNode(processBoard)
                .AddNode(deployEntities)
                .Build();
    }
}
