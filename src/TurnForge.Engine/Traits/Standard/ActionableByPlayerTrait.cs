using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Trait indicating this agent is controllable by a specific player.
/// The PlayerId links the agent to its owning player.
/// </summary>
public class ActionableByPlayerTrait(PlayerId playerId) : BaseDataTrait
{
    /// <summary>
    /// The player that controls this agent.
    /// </summary>
    public PlayerId PlayerId { get; } = playerId;
}
