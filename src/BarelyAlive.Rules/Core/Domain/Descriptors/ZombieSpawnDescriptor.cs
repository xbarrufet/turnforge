using TurnForge.Engine.Entities.Actors.Descriptors;

namespace BarelyAlive.Rules.Core.Domain.Descriptors;

/// <summary>
/// Descriptor for ZombieSpawn prop entities with spawn order configuration.
/// </summary>
/// <remarks>
/// Extends PropDescriptor with ZombieSpawn-specific properties that control
/// the spawn order of zombie waves.
/// 
/// Example:
/// <code>
/// var request = SpawnRequestBuilder
///     .For("Spawn.Zombie")
///     .At(position)
///     .WithProperty("Order", 99)  // High priority spawn
///     .Build();
/// </code>
/// 
/// The Order property will be mapped to the ZombieSpawnComponent during entity creation.
/// </remarks>
public class ZombieSpawnDescriptor : PropDescriptor
{
    /// <summary>
    /// Spawn order for zombie waves (lower values spawn first).
    /// </summary>
    public int Order { get; set; } = 1;
    
    public ZombieSpawnDescriptor(string definitionId) : base(definitionId) { }
}
