using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;

namespace TurnForge.Engine.Decisions.Spawn;

/// <summary>
/// Decision containing a fully configured descriptor ready for entity creation.
/// Generic to allow type-safe descriptors per strategy.
/// </summary>
/// <typeparam name="TDescriptor">Type of descriptor (AgentDescriptor, PropDescriptor, etc.)</typeparam>
/// <remarks>
/// SpawnDecisions are created by strategies from processed descriptors.
/// The Applier receives these decisions and uses GenericActorFactory to create the actual entities.
/// 
/// Example:
/// <code>
/// var decision = new SpawnDecision&lt;AgentDescriptor&gt;(
///     new AgentDescriptor("Survivors.Mike") 
///     {
///         Position = spawnPoint
///     }
/// );
/// </code>
/// </remarks>
public sealed record SpawnDecision<TDescriptor>(
    TDescriptor Descriptor
) : IDecision
    where TDescriptor : IGameEntityBuildDescriptor
{
    /// <summary>
    /// When this decision should be executed
    /// </summary>
    public DecisionTiming Timing { get; init; } = DecisionTiming.Immediate;
    
    /// <summary>
    /// Origin of this decision (command ID, system, etc.)
    /// </summary>
    public string OriginId { get; init; } = "System";
}
