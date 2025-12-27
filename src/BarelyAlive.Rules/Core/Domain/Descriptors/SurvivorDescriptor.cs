using TurnForge.Engine.Definitions.Actors.Descriptors;

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
    // Properties removed in favor of Traits (TeamTrait, ActionPointsTrait)
    // - Faction -> TeamTrait
    // - ActionPoints -> ActionPointsTrait
    
    public SurvivorDescriptor(string definitionId) : base(definitionId) { }
}
