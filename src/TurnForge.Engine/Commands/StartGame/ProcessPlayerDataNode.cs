using TurnForge.Engine.Commands.StartGame.Action.Inputs;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Nodes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.Entities.Players;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Action;

public class ProcessPlayerDataNode : InteractionNode<ActionContext>
{
    public ProcessPlayerDataNode() : base("StartGame.ProcessPlayerData") { }

    protected override void ProcessNewInputs(ActionContext context, GameStateView state)
    {
        // Use typed context for all operations
        if (context is not StartGameActionContext typedContext)
        {
            throw new InvalidOperationException("ProcessPlayerDataNode requires StartGameActionContext");
        }

        // Process Typed Inputs
        if (typedContext.PlayerInputs.Count > 0)
        {
            foreach (var input in typedContext.PlayerInputs)
            {
                ProcessPlayerInput(input, typedContext, state);
            }
            typedContext.PlayerInputs.Clear();
        }
        typedContext.PlayersConfirmed = true;
    }

    private bool ProcessPlayerInput(AddPlayerInput input, StartGameActionContext context, GameStateView state)
    {
        if (input != null && !string.IsNullOrWhiteSpace(input.PlayerName))
        {
            var player = PlayerFactory.BuildNewPlayer(input.PlayerId,
                                                    input.PlayerController,
                                                    input.Team,
                                                    input.PlayerName,
                                                 ActionPoolTypeExtensions.FromString(input.ActionPoolType),
                                                 input.MaxActions);
            // Store in overlay using state view
            if (context.Players == null)
            {
                context.Players = new();
            }
            context.Players.Add(player);

            // Store agent descriptors using typed property
            foreach (var agentDesc in input.AgentDescriptors)
            {
                context.PendingAgentDeployments.Add(new AgentDeployment
                {
                    Descriptor = agentDesc.Descriptor,
                    OwnerId = input.PlayerId,
                    Team = input.Team,
                    Position = agentDesc.Position
                });
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    protected override bool IsReadyToComplete(ActionContext context)
    {
        if (context is StartGameActionContext typedContext)
        {
            return typedContext.PlayersConfirmed;
        }
        return false;
    }

    protected override (string Reason, Type[] AllowedInputs) GetRequiredInteractions(ActionContext context)
    {
        if (context is not StartGameActionContext typedContext)
        {
            return ("Invalid context type.", Array.Empty<Type>());
        }

        if (typedContext.PlayerNames.Count == 0)
        {
            return ("Waiting for at least one player.", new[] { typeof(AddPlayerInput) });
        }

        return ("Waiting for more players or confirmation.", new[] { typeof(AddPlayerInput), typeof(ConfirmPlayersInput) });
    }
}