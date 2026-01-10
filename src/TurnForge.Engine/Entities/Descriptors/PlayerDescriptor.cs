using TurnForge.Engine.Entities.Players.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

/// <summary>
/// Descriptor for spawning Player entities.
/// </summary>
public class PlayerDescriptor : GameEntityBuildDescriptor
{
    /// <summary>
    /// Custom player identifier for agent binding.
    /// </summary>
    public PlayerId PlayerId { get; init; }

    public PlayerDescriptor(string definitionId, PlayerId playerId, string name) : base(definitionId,name)
    {
        PlayerId = playerId;
    }
    
    public PlayerDescriptor(string definitionId, PlayerId playerId) : base(definitionId,playerId)
    {
        PlayerId = playerId;
    }
}
