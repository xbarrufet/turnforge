using TurnForge.Engine.Components;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Defines the faction and controller ownership of an entity.
/// Mapped to TeamComponent.
/// </summary>
public class TeamTrait : BaseComponentTrait<TeamComponent>
{
    public string InitialTeam { get; }
    public string InitialController { get; }
    
    /// <summary>
    /// The player who owns this entity (for query purposes).
    /// </summary>
    public PlayerId? OwnerId { get; }

    public TeamTrait(string team, string controller, PlayerId? ownerId = null)
    {
        InitialTeam = team;
        InitialController = controller;
        OwnerId = ownerId;
    }

    // Default
    public TeamTrait() : this("Neutral", "AI", null) { }
}

