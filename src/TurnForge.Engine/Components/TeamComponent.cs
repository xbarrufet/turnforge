using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components;

public class TeamComponent : ITeamComponent
{
    public string Team { get; set; }
    public string ControllerId { get; set; }
    
    /// <summary>
    /// The player who owns/controls this entity.
    /// </summary>
    public PlayerId? OwnerId { get; set; }

    // Required by TraitInitializationService (via BaseComponentTrait logic)
    public TeamComponent(TurnForge.Engine.Traits.Standard.TeamTrait trait)
    {
        Team = trait.InitialTeam;
        ControllerId = trait.InitialController;
        OwnerId = trait.OwnerId;
    }
}

