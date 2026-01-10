using System;
using System.Collections.Generic;
using TurnForge.Engine.Commands.StartGame.Action.Inputs;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Players;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Action;

public class StartGameActionContext : ActionContext
{
    /// <summary>
    /// List of player names added to the game.
    /// Backed by internal dictionary for consistency.
    /// </summary>
    public List<string> PlayerNames
    {
        get
        {
            if (!TryGet<List<string>>(nameof(PlayerNames), out var v))
            {
                v = new List<string>();
                Set(nameof(PlayerNames), v);
            }
            return v;
        }
        set => Set(nameof(PlayerNames), value);
    }

    /// <summary>
    /// Indicates whether the player configuration has been confirmed.
    /// Backed by internal dictionary for type-safe access.
    /// </summary>
    public bool PlayersConfirmed
    {
        get => TryGet<bool>(nameof(PlayersConfirmed), out var v) && v;
        set => Set(nameof(PlayersConfirmed), value);
    }

    /// <summary>
    /// The selected map identifier.
    /// Backed by internal dictionary for consistency.
    /// </summary>
    public string MapId
    {
        get => TryGet<string>(nameof(MapId), out var v) ? v : string.Empty;
        set => Set(nameof(MapId), value);
    }

    /// <summary>
    /// Deployment pending lists.
    /// Backed by internal dictionary for consistency.
    /// </summary>
    public List<AgentDeployment> PendingAgentDeployments
    {
        get
        {
            if (!TryGet<List<AgentDeployment>>(nameof(PendingAgentDeployments), out var v))
            {
                v = new List<AgentDeployment>();
                Set(nameof(PendingAgentDeployments), v);
            }
            return v;
        }
    }

    public List<PropDeployment> PendingPropDeployments
    {
        get
        {
            if (!TryGet<List<PropDeployment>>(nameof(PendingPropDeployments), out var v))
            {
                v = new List<PropDeployment>();
                Set(nameof(PendingPropDeployments), v);
            }
            return v;
        }
    }

    public List<ZoneDeployment> PendingZoneDeployments
    {
        get
        {
            if (!TryGet<List<ZoneDeployment>>(nameof(PendingZoneDeployments), out var v))
            {
                v = new List<ZoneDeployment>();
                Set(nameof(PendingZoneDeployments), v);
            }
            return v;
        }
    }

    public List<ConnectionDeployment> PendingConnectionDeployments
    {
        get
        {
            if (!TryGet<List<ConnectionDeployment>>(nameof(PendingConnectionDeployments), out var v))
            {
                v = new List<ConnectionDeployment>();
                Set(nameof(PendingConnectionDeployments), v);
            }
            return v;
        }
    }

    // Configuration Inputs (Raw Data)
    public List<AddPlayerInput> PlayerInputs
    {
        get
        {
            if (!TryGet<List<AddPlayerInput>>(nameof(PlayerInputs), out var v))
            {
                v = new List<AddPlayerInput>();
                Set(nameof(PlayerInputs), v);
            }
            return v;
        }
        set => Set(nameof(PlayerInputs), value);
    }

    public BoardDataInput BoardData
    {
        get => TryGet<BoardDataInput>(nameof(BoardData), out var v) ? v : null;
        set
        {
            if (value != null)
            {
                Set(nameof(BoardData), value);
            }
            else
            {
                Remove(nameof(BoardData));
            }
        }
    }

    public MissionDataInput? MissionData
    {
        get => TryGet<MissionDataInput>(nameof(MissionData), out var v) ? v : null;
        set
        {
            if (value != null)
            {
                Set(nameof(MissionData), value);
            }
            else
            {
                Remove(nameof(MissionData));
            }
        }
    }

    public IGameBoard GameBoard
    {
        get => TryGet<IGameBoard>(nameof(GameBoard), out var v) ? v : null;
        set => Set(nameof(GameBoard), value);
    }

    public List<Player> Players
    {
        get => TryGet<List<Player>>(nameof(Players), out var v) ? v : null;
        set => Set(nameof(Players), value);
    }

    public StartGameActionContext()
    {
    }
}
