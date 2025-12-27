using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Decisions.Spawn;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Strategies.Spawn;

/// <summary>
/// Base class for spawn strategies using pattern matching for type dispatch.
/// </summary>
/// <remarks>
/// ARCHITECTURE: Pattern Matching (Zero Reflection)
/// 
/// This base class provides a simple template method pattern where subclasses
/// override ProcessDescriptor() to implement type-specific spawn logic using
/// C# pattern matching (switch expressions).
/// 
/// BENEFITS:
/// - ✅ Zero reflection (10-50x faster than reflection)
/// - ✅ Compile-time type safety
/// - ✅ Better debuggability and IntelliSense support
/// - ✅ Explicit and maintainable
/// 
/// USAGE PATTERN:
/// <code>
/// public class MySpawnStrategy : BaseSpawnStrategy
/// {
///     protected override AgentDescriptor ProcessDescriptor(
///         AgentDescriptor descriptor,
///         GameState state)
///     {
///         // Pattern matching: compile-time type dispatch
///         return descriptor switch
///         {
///             SurvivorDescriptor s => ProcessSurvivor(s, state),
///             ZombieDescriptor z => ProcessZombie(z, state),
///             _ => descriptor // default: accept as-is
///         };
///     }
///     
///     private SurvivorDescriptor ProcessSurvivor(
///         SurvivorDescriptor descriptor,
///         GameState state)
///     {
///         descriptor.Position = FindPlayerSpawn(state);
///         return descriptor;
///     }
/// }
/// </code>
/// </remarks>
public abstract class BaseSpawnStrategy : ISpawnStrategy<AgentDescriptor>
{
    /// <summary>
    /// Entry point called by CommandHandler.
    /// Iterates descriptors and delegates to virtual ProcessDescriptor().
    /// </summary>
    public virtual IReadOnlyList<AgentDescriptor> Process(
        IReadOnlyList<AgentDescriptor> descriptors,
        GameState state)
    {
        if (descriptors.Count == 0) return descriptors;

        var processed = new List<AgentDescriptor>(descriptors.Count);
        
        foreach (var descriptor in descriptors)
        {
            // Virtual call - subclass handles type dispatch via pattern matching
            var result = ProcessDescriptor(descriptor, state);
            processed.Add(result);
        }
        
        return processed;
    }

    /// <summary>
    /// Process a single descriptor with type-specific logic.
    /// Override and use pattern matching to dispatch to type-specific handlers.
    /// </summary>
    /// <param name="descriptor">Descriptor to process (may be specific subtype)</param>
    /// <param name="state">Current game state for context</param>
    /// <returns>Processed descriptor (modified in-place or returned)</returns>
    /// <remarks>
    /// DEFAULT BEHAVIOR: Accept descriptor as-is without modification.
    /// 
    /// RECOMMENDED PATTERN:
    /// Override and use switch expression for type dispatch:
    /// <code>
    /// return descriptor switch
    /// {
    ///     SpecificDescriptor s => HandleSpecific(s, state),
    ///     AnotherDescriptor a => HandleAnother(a, state),
    ///     _ => descriptor // fallback
    /// };
    /// </code>
    /// </remarks>
    protected virtual AgentDescriptor ProcessDescriptor(
        AgentDescriptor descriptor,
        GameState state)
    {
        // Default implementation: accept descriptor without changes
        return descriptor;
    }

    /// <summary>
    /// Converts processed descriptors to spawn decisions.
    /// Override for custom decision creation logic.
    /// </summary>
    public virtual IReadOnlyList<SpawnDecision<AgentDescriptor>> ToDecisions(
        IReadOnlyList<AgentDescriptor> descriptors)
    {
        return descriptors
            .Select(d => new SpawnDecision<AgentDescriptor>(d))
            .ToList();
    }
}

