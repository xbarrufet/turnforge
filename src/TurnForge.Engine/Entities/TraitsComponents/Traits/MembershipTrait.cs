using TurnForge.Engine.Components;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.Traits.Interfaces;

namespace TurnForge.Engine.Entities.Traits.Standard;

/// <summary>
/// Defines the faction and controller ownership of an entity.
/// Mapped to TeamComponent.
/// </summary>
public class MembershipTrait(PlayerId playerId, TeamId teamId) : ITrait
{
    
    /// <summary>
    /// The player who owns this entity (for query purposes).
    /// </summary>
    public PlayerId ControlledBy { get; } = playerId;
    public TeamId MemberOf { get; } = teamId;

    // Default
    public MembershipTrait() : this(PlayerId.Empty, TeamId.Empty) { }
}

