using System.Diagnostics;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Players;

/// <summary>
/// A Player entity that controls agents.
/// Players are top-level controllers, not GameEntities.
/// </summary>
[DebuggerDisplay("{Name} ({Team}) - {PlayerController}")]
public class Player
{
    /// <summary>
    /// Custom player identifier used for agent binding.
    /// </summary>
    public PlayerId PlayerId { get; }
    public IActionPool ActionPool { get; set; }

    public PlayerControllerType PlayerController { get; }
    public string Name { get; }
    public string Team { get; }
    
    // TODO: Add ActionPool property or logic if needed, previously handled by Traits.

    public Player(PlayerId playerId, PlayerControllerType playerController, string name, string team, IActionPool actionPool)
    {
        PlayerId = playerId;
        PlayerController = playerController;
        Name = name;
        Team = team;
        ActionPool = actionPool;
    }

    public Player Clone()
    {
        return new Player(PlayerId, PlayerController, Name, Team, ActionPool)
        {
            ActionPool = this.ActionPool // Assuming IActionPool is immutable or handled appropriately
        };
    }
}

public enum PlayerControllerType
{
    Human,
    AI
}
