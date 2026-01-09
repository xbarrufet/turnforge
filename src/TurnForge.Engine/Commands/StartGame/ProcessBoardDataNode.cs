using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Action; // ADDED
using TurnForge.Engine.Core.Action.Nodes;
using TurnForge.Engine.Commands.StartGame.Action.Inputs;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.Entities.Spawn;

namespace TurnForge.Engine.Commands.StartGame.Action;

public class ProcessBoardDataNode : InteractionNode<ActionContext>
{
    private readonly IBoardFactory _boardFactory;

    public ProcessBoardDataNode(IBoardFactory boardFactory) : base("StartGame.ProcessBoardData") 
    {
        _boardFactory = boardFactory;
    }

    protected override void ProcessNewInputs(ActionContext context)
    {
        if (!context.Has("PendingPropDeployments")) context.Set("PendingPropDeployments", new List<PropDeployment>());

        if (context.HasInput<SelectMapInput>())
        {
            var input = context.ConsumeInput<SelectMapInput>();
            if (input != null)
            {
                context.Set("MapId", input.MapId);
                
                // 1. Create board
                var board = _boardFactory.CreateGameBoard(input.BoardDefinition);
                
                // 2. Record creation operation (Board + Mission)
                var createOp = new CreateBoardOperation(board, input.Mission);
                context.Overlay.Record(createOp);
                
                // 3. Resolve agent positions using mission data
                if (input.Mission != null && input.Mission.PlayerSpawnZones != null)
                {
                    if (context.TryGet<List<AgentDeployment>>("PendingAgentDeployments", out var pendingAgents))
                    {
                        foreach (var deployment in pendingAgents)
                        {
                            if (deployment.Position == null)
                            {
                                // Lookup spawn zone from mission for this player
                                if (input.Mission.PlayerSpawnZones.TryGetValue(deployment.OwnerId, out var spawnPos))
                                {
                                    deployment.Position = spawnPos;
                                }
                            }
                        }
                    }
                }
                
                // 4. Store props for later deployment (DeployEntitiesNode)
                if (input.BoardDefinition.Props != null)
                {
                    var pendingProps = context.Get<List<PropDeployment>>("PendingPropDeployments");
                    foreach (var prop in input.BoardDefinition.Props)
                    {
                        pendingProps.Add(new PropDeployment(
                            prop.Definition,
                            prop.FixedPosition
                        ));
                    }
                }
                
                // 5. Spawn Connection Entities from MissionData
                if (input.Mission != null && input.Mission.ConnectionRequests != null && input.Mission.ConnectionRequests.Count > 0)
                {
                    var connectionSpawner = new ConnectionSpawner();
                    connectionSpawner.SpawnConnections(input.Mission.ConnectionRequests, context.Overlay);
                }
            }
        }
    }

    protected override bool IsReadyToComplete(ActionContext context)
    {
         return context.Has("MapId") && !string.IsNullOrEmpty(context.Get<string>("MapId"));
    }

    protected override (string Reason, Type[] AllowedInputs) GetRequiredInteractions(ActionContext context)
    {
        return ("Please select a map and mission.", new[] { typeof(SelectMapInput) });
    }
}