using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions;

/// <summary>
/// Base definition for Player entities.
/// Players control agents and have an action pool.
/// They are NOT located on the board (no PositionTrait).
/// </summary>
public abstract class PlayerDefinition : BaseGameEntityDefinition
{
    /// <summary>
    /// Custom identifier for this player, used to link agents via ActionableByPlayerTrait.
    /// </summary>
    public PlayerId PlayerId { get; }

    protected PlayerDefinition(string definitionId, string category, PlayerId playerId) 
        : base(definitionId, category)
    {
        PlayerId = playerId;
        // Player has ActionPoolTrait for managing actions
        // Default: 1 action per turn, fixed mode
        AddTrait(new ActionPoolTrait(1));
    }

    protected PlayerDefinition(string definitionId, PlayerId playerId) 
        : base(definitionId, "Player")
    {
        PlayerId = playerId;
        // Player has ActionPoolTrait for managing actions
        // Default: 1 action per turn, fixed mode
        AddTrait(new ActionPoolTrait());
    }


    protected PlayerDefinition(string definitionId, string category, PlayerId playerId, ActionPoolTrait actionPool) 
        : base(definitionId, category)
    {
        PlayerId = playerId;
        AddTrait(actionPool);
    }

    protected PlayerDefinition(string definitionId, PlayerId playerId, ActionPoolTrait actionPool) 
        : base(definitionId, "Player")
    {
        PlayerId = playerId;
        AddTrait(actionPool);
    }
}

