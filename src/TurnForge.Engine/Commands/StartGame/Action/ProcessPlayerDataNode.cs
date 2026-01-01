using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Action.Nodes;
using TurnForge.Engine.Commands.StartGame.Action.Inputs;

namespace TurnForge.Engine.Commands.StartGame.Action;

public class ProcessPlayerDataNode : InteractionNode<StartGameActionContext>
{
    public ProcessPlayerDataNode() : base("StartGame.ProcessPlayerData") { }

    protected override void ProcessNewInputs(StartGameActionContext context)
    {
        while (context.HasInput<AddPlayerInput>())
        {
            var input = context.ConsumeInput<AddPlayerInput>();
            if (input != null && !string.IsNullOrWhiteSpace(input.PlayerName))
            {
                // 1. Store player name for tracking (BuildGameNode creates actual Players)
                if (!context.PlayerNames.Contains(input.PlayerName))
                {
                    context.PlayerNames.Add(input.PlayerName);
                }
                
                // 2. Store agent descriptors for later deployment
                foreach (var agentDesc in input.AgentDescriptors)
                {
                    context.PendingAgentDeployments.Add(new AgentDeployment
                    {
                        Descriptor = agentDesc.Descriptor,
                        OwnerId = input.PlayerId,
                        Position = agentDesc.Position  // May be null (resolved later)
                    });
                }
            }
        }

        if (context.HasInput<ConfirmPlayersInput>())
        {
            context.ConsumeInput<ConfirmPlayersInput>();
            if (context.PlayerNames.Count > 0)
            {
                context.PlayersConfirmed = true;
            }
        }
    }

    protected override bool IsReadyToComplete(StartGameActionContext context)
    {
        return context.PlayersConfirmed;
    }

    protected override (string Reason, Type[] AllowedInputs) GetRequiredInteractions(StartGameActionContext context)
    {
        if (context.PlayerNames.Count == 0)
        {
            return ("Waiting for at least one player.", new[] { typeof(AddPlayerInput) });
        }

        return ("Waiting for more players or confirmation.", new[] { typeof(AddPlayerInput), typeof(ConfirmPlayersInput) });
    }
}