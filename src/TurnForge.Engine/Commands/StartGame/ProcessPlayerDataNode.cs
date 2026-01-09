using TurnForge.Engine.Core.Action; 
using TurnForge.Engine.Core.Action.Nodes;
using TurnForge.Engine.Commands.StartGame.Action.Inputs;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.Entities.Players;
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
                var player = PlayerFactory.BuildNewPlayer(input.PlayerId, 
                                                     input.PlayerName,
                                                     ActionPoolTypeExtensions.FromString(input.ActionPoolType),
                                                     input.MaxActions);
                // store in overlay
                context.Overlay.Record(new AddPlayerOperation(player));
                // set as confirmed
                context.Set("PlayersConfirmed", true);
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