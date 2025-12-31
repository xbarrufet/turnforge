using System;
using TurnForge.Engine.Core.Workflow.Nodes;
using TurnForge.Engine.Commands.StartGame.Workflow.Inputs;
using TurnForge.Engine.Commands.StartGame.Workflow.Operations;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Engine.Commands.StartGame.Workflow;

public class ProcessBoardDataNode : InteractionNode<StartGameWorkflowContext>
{
    private readonly IBoardFactory _boardFactory;

    public ProcessBoardDataNode(IBoardFactory boardFactory) : base("StartGame.ProcessBoardData") 
    {
        _boardFactory = boardFactory;
    }

    protected override void ProcessNewInputs(StartGameWorkflowContext context)
    {
        if (context.HasInput<SelectMapInput>())
        {
            var input = context.ConsumeInput<SelectMapInput>();
            if (input != null)
            {
                context.MapId = input.MapId;
                
                // 1. Create board using factory (or passed definition if simpler)
                // Assuming factory creates from definition for now, or just use definiton.
                // Factory usually takes ID, but input has Definition?
                // Plan said: boardFactory.Create(input.BoardDefinition)
                var board = _boardFactory.CreateGameBoard(input.BoardDefinition);
                
                // 2. Record creation operation (Board + Mission)
                var createOp = new CreateBoardOperation(board, input.MissionData);
                context.Overlay.Record(createOp);
                
                // 3. Resolve agent positions using mission data
                if (input.MissionData.PlayerSpawnZones != null)
                {
                    foreach (var deployment in context.PendingAgentDeployments)
                    {
                        if (deployment.Position == null)
                        {
                            // Lookup spawn zone from mission for this player
                            if (input.MissionData.PlayerSpawnZones.TryGetValue(deployment.OwnerId, out var spawnPos))
                            {
                                deployment.Position = spawnPos;
                            }
                        }
                    }
                }
                
                // 4. Store props for later deployment (DeployEntitiesNode)
                if (input.BoardDefinition.Props != null)
                {
                    foreach (var prop in input.BoardDefinition.Props)
                    {
                        context.PendingPropDeployments.Add(new PropDeployment(
                            prop.Definition,
                            prop.FixedPosition
                        ));
                    }
                }
            }
        }
    }

    protected override bool IsReadyToComplete(StartGameWorkflowContext context)
    {
         return !string.IsNullOrEmpty(context.MapId);
    }

    protected override (string Reason, Type[] AllowedInputs) GetRequiredInteractions(StartGameWorkflowContext context)
    {
        return ("Please select a map and mission.", new[] { typeof(SelectMapInput) });
    }
}