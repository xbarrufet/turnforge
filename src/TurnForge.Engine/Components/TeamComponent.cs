using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Components;

public class TeamComponent : ITeamComponent
{
    public string Team { get; set; }
    public string ControllerId { get; set; }

    // Required by TraitInitializationService (via BaseComponentTrait logic)
    public TeamComponent(TurnForge.Engine.Traits.Standard.TeamTrait trait)
    {
        Team = trait.InitialTeam;
        ControllerId = trait.InitialController;
    }
}
