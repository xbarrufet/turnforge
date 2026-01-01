using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Descriptors;

/// <summary>
/// Descriptor for a ConnectionEntity to be spawned.
/// Used in MissionData to define connections with semantic metadata.
/// </summary>
public record ConnectionDescriptor(
    TileId From,
    TileId To,
    string Category,                    // "forward", "backward", "finish_entry", etc.
    string? RestrictedToTeam = null,    // "red", "blue", etc. - null means all can use
    string? DefinitionId = null         // Optional entity definition ID
)
{
    /// <summary>
    /// Create a forward connection.
    /// </summary>
    public static ConnectionDescriptor Forward(string from, string to) 
        => new(new TileId(from), new TileId(to), "forward");
    
    /// <summary>
    /// Create a backward connection.
    /// </summary>
    public static ConnectionDescriptor Backward(string from, string to) 
        => new(new TileId(from), new TileId(to), "backward");
    
    /// <summary>
    /// Create a finish entry connection restricted to a team.
    /// </summary>
    public static ConnectionDescriptor FinishEntry(string from, string to, string team) 
        => new(new TileId(from), new TileId(to), "finish_entry", team);
}
