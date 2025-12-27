using TurnForge.Engine.Components;

namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Defines the faction and controller ownership of an entity.
/// Mapped to TeamComponent.
/// </summary>
public class TeamTrait : BaseComponentTrait<TeamComponent>
{
    public string InitialTeam { get; }
    public string InitialController { get; }

    public TeamTrait(string team, string controller)
    {
        InitialTeam = team;
        InitialController = controller;
    }

    // Default
    public TeamTrait() : this("Neutral", "AI") { }
}
