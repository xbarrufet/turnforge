using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Spawn;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Action;

/// <summary>
/// Factory for creating the StartGame action.
/// </summary>
public static class StartGameAction
{
    public const string ActionId = "Core.StartGame";

    public static IAction Create(IBoardFactory boardFactory, ISpawnService spawnService)
    {
        var processPlayer = new ProcessPlayerDataNode();
        var processBoard = new ProcessBoardDataNode(boardFactory);
        var buildInitialState = new BuildInitialStateNode(new NodeId("BuildInitialState"), spawnService);

        return ActionBuilder.Create(ActionId)
                .WithContext(() => new StartGameActionContext())
                .AddNode(processPlayer)
                .AddNode(processBoard)
                .AddNode(buildInitialState)
                .Build();
    }
}
