using TurnForge.Engine.Core.Action; 
using TurnForge.Engine.Core.Action.Nodes;
using TurnForge.Engine.Commands.StartGame.Action.Inputs;
using TurnForge.Engine.ValueObjects; 

namespace TurnForge.Engine.Commands.StartGame.Action;

public class ProcessPlayerDataNode : InteractionNode<ActionContext>
{
    public ProcessPlayerDataNode() : base("StartGame.ProcessPlayerData") { }

    protected override void ProcessNewInputs(ActionContext context)
    {
        // Initialize state lists if missing
        if (!context.Has("PlayerNames")) context.Set("PlayerNames", new List<string>());
        if (!context.Has("PreparedPlayers")) context.Set("PreparedPlayers", new List<(PlayerId, string)>());
        if (!context.Has("PendingAgentDeployments")) context.Set("PendingAgentDeployments", new List<AgentDeployment>());

        while (context.HasInput<AddPlayerInput>())
        {
            var input = context.ConsumeInput<AddPlayerInput>();
            if (input != null && !string.IsNullOrWhiteSpace(input.PlayerName))
            {
                var playerNames = context.Get<List<string>>("PlayerNames");
                
                // 1. Store player info
                if (!playerNames.Contains(input.PlayerName))
                {
                    playerNames.Add(input.PlayerName);
                    
                    var preparedPlayers = context.Get<List<(PlayerId, string)>>("PreparedPlayers");
                    preparedPlayers.Add((input.PlayerId, input.PlayerName));
                }
                
                // 2. Store agent descriptors
                var pendingAgents = context.Get<List<AgentDeployment>>("PendingAgentDeployments");
                foreach (var agentDesc in input.AgentDescriptors)
                {
                    pendingAgents.Add(new AgentDeployment
                    {
                        Descriptor = agentDesc.Descriptor,
                        OwnerId = input.PlayerId,
                        Position = agentDesc.Position 
                    });
                }
            }
        }

        if (context.HasInput<ConfirmPlayersInput>())
        {
            context.ConsumeInput<ConfirmPlayersInput>();
            var playerNames = context.Get<List<string>>("PlayerNames");
            if (playerNames.Count > 0)
            {
                context.Set("PlayersConfirmed", true);
            }
        }
    }

    protected override bool IsReadyToComplete(ActionContext context)
    {
        return context.TryGet<bool>("PlayersConfirmed", out var confirmed) && confirmed;
    }

    protected override (string Reason, Type[] AllowedInputs) GetRequiredInteractions(ActionContext context)
    {
        var playerNames = context.Has("PlayerNames") ? context.Get<List<string>>("PlayerNames") : new List<string>();
        
        if (playerNames.Count == 0)
        {
            return ("Waiting for at least one player.", new[] { typeof(AddPlayerInput) });
        }

        return ("Waiting for more players or confirmation.", new[] { typeof(AddPlayerInput), typeof(ConfirmPlayersInput) });
    }
}