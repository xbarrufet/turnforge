using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Commands.StartGame.Action;

public class StartGameActionContext : ActionContext
{
    public List<string> PlayerNames { get; set; } = new();
    public bool PlayersConfirmed { get; set; }
    public string MapId { get; set; } = string.Empty;
    
    // Deployment pending lists
    public List<AgentDeployment> PendingAgentDeployments { get; } = new();
    public List<PropDeployment> PendingPropDeployments { get; } = new();

    public StartGameActionContext(Guid id, GameState gameState) : base()
    {
        InitializeState(gameState);
    }

    public override object? GetResult()
    {
        return null;
    }
}
