using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.Player;

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

    protected static Category PlayerCategory = new("Player");
    
    protected PlayerDefinition(string definitionId, PlayerId playerId) 
        : base(definitionId, PlayerCategory)
    {
        PlayerId = playerId;
        // Player has ActionPoolTrait for managing actions
        // Default: 1 action per turn, fixed mode
        AddTrait(new ActionPoolTrait(1));
    }
    


    protected PlayerDefinition(string definitionId, Category category, PlayerId playerId, ActionPoolTrait actionPool) 
        : base(definitionId, category)
    {
        PlayerId = playerId;
        AddTrait(actionPool);
    }

    protected PlayerDefinition(string definitionId, PlayerId playerId, ActionPoolTrait actionPool) 
        : base(definitionId, PlayerCategory)
    {
        PlayerId = playerId;
        AddTrait(actionPool);
    }
}

