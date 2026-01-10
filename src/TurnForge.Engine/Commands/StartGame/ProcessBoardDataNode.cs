using System;
using System.Collections.Generic;
using TurnForge.Engine.Commands.StartGame.Action.Inputs;
using TurnForge.Engine.Core.Action; // ADDED
using TurnForge.Engine.Core.Action.Nodes;
using TurnForge.Engine.Entities; // ADDED for GameStateView
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

    protected override void ProcessNewInputs(ActionContext context, GameStateView state)
    {
        // Use typed context for all operations
        if (context is not StartGameActionContext typedContext)
        {
            throw new InvalidOperationException("ProcessBoardDataNode requires StartGameActionContext");
        }

        // Check typed MapInput property first
        if (typedContext.BoardData != null)
        {
            ProcessMapInput(typedContext.BoardData, typedContext, state);
            typedContext.BoardData = null; // Handled
            return;
        }

        // Check input queue
        if (context.HasInput<BoardDataInput>())
        {
            var input = context.ConsumeInput<BoardDataInput>();
            if (input != null)
            {
                ProcessMapInput(input, typedContext, state);
            }
        }
    }

    private void ProcessMapInput(BoardDataInput input, StartGameActionContext context, GameStateView state)
    {
        if (input != null)
        {
            // Use typed property instead of string key
            context.MapId = input.MapId;

            // 1. Create board
            var board = _boardFactory.CreateGameBoard(input.BoardDescriptor);
            context.GameBoard = board;

            // 2. Store zones for later deployment (BuildInitialStateNode)
            foreach (var zoneDeploy in input.Zones)
            {
                context.PendingZoneDeployments.Add(zoneDeploy);
            }

            // 3. Store connections for later deployment (BuildInitialStateNode)
            foreach (var connDeploy in input.Connections)
            {
                context.PendingConnectionDeployments.Add(connDeploy);
            }

            // 5. Store props for later deployment (DeployEntitiesNode)
           /* if (input.BoardDefinition.Props != null)
            {
                // Use typed property for pending props
                foreach (var prop in input.BoardDefinition.Props)
                {
                    context.PendingPropDeployments.Add(new PropDeployment(
                        prop.Definition,
                        prop.FixedPosition
                    ));
                }
            }*/


        }
    }

    protected override bool IsReadyToComplete(ActionContext context)
    {
        if (context is StartGameActionContext typedContext)
        {
            return !string.IsNullOrEmpty(typedContext.MapId);
        }
        return false;
    }

    protected override (string Reason, Type[] AllowedInputs) GetRequiredInteractions(ActionContext context)
    {
        return ("Please select a map and mission.", new[] { typeof(BoardDataInput) });
    }
}