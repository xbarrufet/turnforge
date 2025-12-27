using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Strategies.Spawn;

/// <summary>
/// Modern strategy for prop spawning.
/// Uses default implementation - no filtering or modification needed.
/// Process() accepts all descriptors as-is.
/// ToDecisions() wraps them in SpawnDecision<PropDescriptor>.
/// </summary>
public class ConfigurablePropSpawnStrategy : ISpawnStrategy<PropDescriptor>
{
    // All methods inherited from ISpawnStrategy<PropDescriptor> with default implementations:
    // - Process(descriptors, state) returns descriptors unchanged (accept all)
    // - ToDecisions(descriptors) wraps descriptors in SpawnDecision<PropDescriptor>
    
    // No custom logic needed - props spawn exactly as requested in mission files
}
