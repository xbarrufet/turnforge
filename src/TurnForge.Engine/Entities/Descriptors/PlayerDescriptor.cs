using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors.Descriptors;

/// <summary>
/// Descriptor for spawning Player entities.
/// </summary>
public class PlayerDescriptor : GameEntityBuildDescriptor
{
    /// <summary>
    /// Custom player identifier for agent binding.
    /// </summary>
    public PlayerId PlayerId { get; init; }

    public PlayerDescriptor(string definitionId, PlayerId playerId) : base(definitionId)
    {
        PlayerId = playerId;
    }
}
