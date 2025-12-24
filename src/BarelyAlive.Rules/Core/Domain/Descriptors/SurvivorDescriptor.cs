using TurnForge.Engine.Entities.Actors.Descriptors;

namespace BarelyAlive.Rules.Core.Domain.Descriptors;

/// <summary>
/// Descriptor for Survivor entities with custom spawn properties.
/// </summary>
/// <remarks>
/// Extends AgentDescriptor with Survivor-specific properties that can be set
/// during spawn time via SpawnRequestBuilder.WithProperty() or processed
/// type-safely in BarelyAliveSpawnStrategy.
/// 
/// Example:
/// <code>
/// var request = SpawnRequestBuilder
///     .For("Survivors.Mike")
///     .At(playerSpawn)
///     .WithProperty("Faction", "Police")
///     .WithProperty("ActionPoints", 4)
///     .Build();
/// </code>
/// </remarks>
public class SurvivorDescriptor : AgentDescriptor
{
    /// <summary>
    /// Faction the survivor belongs to (e.g., "Player", "Police", "Military").
    /// </summary>
    public string Faction { get; set; } = "Player";
    
    /// <summary>
    /// Number of action points the survivor starts with.
    /// </summary>
    public int ActionPoints { get; set; } = 3;
    
    public SurvivorDescriptor(string definitionId) : base(definitionId) { }
}
