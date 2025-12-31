
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.StartGame.Workflow;
using TurnForge.Engine.Commands.ValueObjects;
using TurnForge.Engine.Core.Workflow.Builders;
using TurnForge.Engine.Core.Workflow.Interfaces;

namespace TurnForge.Engine.Commands.StartGame;

using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Core.Workflow.Nodes;
using TurnForge.Engine.ValueObjects;

public sealed class StartGameCommand : ICommand
{
    private readonly IWorkflow _workflow;

    public StartGameCommand(IBoardFactory boardFactory, IEntityApplier entityApplier)
    {
        var processPlayer = new ProcessPlayerDataNode();
        var processBoard = new ProcessBoardDataNode(boardFactory);
        var deployEntities = new DeployEntitiesNode(new NodeId("StartGame.DeployEntities"), entityApplier);
        var buildGame = new BuildGameNode();

        _workflow = WorkflowBuilder.Create("StartGame")
                .AddNode(processPlayer)
                .AddNode(processBoard)
                .AddNode(deployEntities)
                .AddNode(buildGame)
                .Build();
    }

    public CommandType CommandType => CommandType.StartGame;
}