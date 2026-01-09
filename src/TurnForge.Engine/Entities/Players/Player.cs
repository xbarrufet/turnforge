using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Actors;

/// <summary>
/// A Player entity that controls agents.
/// Players are top-level controllers, not GameEntities.
/// </summary>
public class Player
{
    /// <summary>
    /// Custom player identifier used for agent binding.
    /// </summary>
    public PlayerId PlayerId { get; }
    
    public string Name { get; }
    
    public int ActionPoints { get; set; }
    public int MaxActionPoints { get; set; }

    // TODO: Add ActionPool property or logic if needed, previously handled by Traits.
    
    public Player(PlayerId playerId, string name)
    {
        PlayerId = playerId;
        Name = name;
        MaxActionPoints = 0; // Default, expected to be configured or loaded
        ActionPoints = 0;
    }

    public Player Clone()
    {
        return new Player(PlayerId, Name)
        {
            ActionPoints = this.ActionPoints,
            MaxActionPoints = this.MaxActionPoints
        };
    }
}
