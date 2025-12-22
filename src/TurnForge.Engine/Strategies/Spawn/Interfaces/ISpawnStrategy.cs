using TurnForge.Engine.Decisions.Spawn;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Strategies.Spawn.Interfaces;

/// <summary>
/// Strategy interface for spawn decisions.
/// The CommandHandler provides pre-populated descriptors from definitions.
/// Strategies only need to filter/modify them based on business rules.
/// </summary>
/// <typeparam name="TDescriptor">Type of descriptor this strategy processes</typeparam>
/// <remarks>
/// This interface provides default implementations for common behavior:
/// - Process(): By default, accepts all descriptors without changes
/// - ToDecisions(): By default, wraps all descriptors in SpawnDecisions
/// 
/// Custom strategies only need to override Process() to implement business logic like:
/// - Filtering descriptors based on game state validation
/// - Assigning spawn positions
/// - Modifying descriptor properties based on mission configuration
/// 
/// Example:
/// <code>
/// public class SurvivorSpawnStrategy : ISpawnStrategy&lt;AgentDescriptor&gt;
/// {
///     public IReadOnlyList&lt;AgentDescriptor&gt; Process(
///         IReadOnlyList&lt;AgentDescriptor&gt; descriptors,
///         GameState state)
///     {
///         return descriptors
///             .Where(d => state.GetAgents().Count &lt; 6) // Max 6 survivors
///             .Select(d => {
///                 d.Position = FindSpawnPoint(state); // Assign position
///                 return d;
///             })
///             .ToList();
///     }
/// }
/// </code>
/// </remarks>
public interface ISpawnStrategy<TDescriptor> 
    where TDescriptor : IGameEntityBuildDescriptor
{
    /// <summary>
    /// Process descriptors (already populated from definitions) and decide which to spawn.
    /// Override this to implement custom spawn logic (filtering, position assignment, etc).
    /// </summary>
    /// <param name="descriptors">Pre-populated descriptors from SpawnRequests</param>
    /// <param name="state">Current game state for context</param>
    /// <returns>Filtered/modified descriptors to spawn</returns>
    IReadOnlyList<TDescriptor> Process(
        IReadOnlyList<TDescriptor> descriptors,
        GameState state)
    {
        // Default implementation: Accept all descriptors without changes
        return descriptors;
    }
    
    /// <summary>
    /// Convert descriptors to spawn decisions.
    /// Usually doesn't need override - default implementation wraps descriptors.
    /// </summary>
    /// <param name="descriptors">Descriptors to convert to decisions</param>
    /// <returns>List of spawn decisions ready for applier</returns>
    IReadOnlyList<SpawnDecision<TDescriptor>> ToDecisions(
        IReadOnlyList<TDescriptor> descriptors)
    {
        return descriptors
            .Select(d => new SpawnDecision<TDescriptor>(d))
            .ToList();
    }
}
