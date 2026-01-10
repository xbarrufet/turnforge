using TurnForge.Engine.Entities.TraitsComponents.Components;
using TurnForge.Engine.Entities.TraitsComponents.Traits;

namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Scope of action pool regeneration.
/// </summary>
public enum ActionPoolScope
{
    /// <summary>Total per game (like lives)</summary>
    Game,
    /// <summary>Regenerates when all players have played one turn</summary>
    Round,
    /// <summary>Regenerates each player turn</summary>
    Turn,
    /// <summary>Regenerates each FSM phase</summary>
    Phase
}

/// <summary>
/// Mode of action pool calculation.
/// </summary>
public enum ActionPoolMode
{
    /// <summary>Fixed number of actions (Parchis: 1)</summary>
    Fixed,
    /// <summary>X actions per controlled agent (Zombicide: 3 per survivor)</summary>
    PerAgent
}

/// <summary>
/// Trait defining the action pool configuration for a Player.
/// Maps to ActionPoolComponent for runtime state.
/// </summary>
public class ActionPoolTrait : BaseComponentTrait<ActionPoolComponent>
{
    public int BaseAmount { get; init; }
    public ActionPoolScope Scope { get; init; }
    public ActionPoolMode Mode { get; init; }

    public ActionPoolTrait() { }

    public ActionPoolTrait(
        int baseAmount = 1,
        ActionPoolScope scope = ActionPoolScope.Turn,
        ActionPoolMode mode = ActionPoolMode.Fixed)
    {
        BaseAmount = baseAmount;
        Scope = scope;
        Mode = mode;
        IsInitialized = true;
    }
}