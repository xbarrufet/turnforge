using BarelyAlive.Rules.Core.Behaviours.ActorBehaviours;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Domain.Strategies.Spawn;

/// <summary>
/// Modern strategy that determines spawn positions for agents.
/// Returns modified descriptors (with positions assigned) instead of creating entities.
/// Entity creation is delegated to the Applier.
/// </summary>
public class ConfigurableAgentSpawnStrategy(
    TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null) : ISpawnStrategy<AgentDescriptor>
{
    private readonly TurnForge.Engine.Core.Interfaces.IGameLogger? _logger = logger;

    /// <summary>
    /// Process descriptors by assigning spawn positions based on game state.
    /// This preserves the original positioning logic while adapting to descriptor-based pattern.
    /// </summary>
    public IReadOnlyList<AgentDescriptor> Process(
        IReadOnlyList<AgentDescriptor> descriptors,
        GameState state)
    {
        var processed = new List<AgentDescriptor>();
        
        foreach (var descriptor in descriptors)
        {
            // Determine spawn position based on category
            var spawnPosition = DetermineSpawnLocation(descriptor.DefinitionID, state);
            
            if (spawnPosition == Position.Empty)
            {
                _logger?.LogWarning($"[SpawnStrategy] No spawn position found for {descriptor.DefinitionID}.");
            }
            else
            {
                _logger?.Log($"[SpawnStrategy] Assigned position {spawnPosition} to {descriptor.DefinitionID}");
            }
            
            // Update descriptor with determined position
            SetDescriptorPosition(descriptor, spawnPosition);
            
            processed.Add(descriptor);
        }
        
        return processed;
    }
    
    /// <summary>
    /// Helper to set position on descriptor (using optional Position property).
    /// </summary>
    private void SetDescriptorPosition(AgentDescriptor descriptor, Position position)
    {
        // Set Position property if available (added to GameEntityBuildDescriptor)
        descriptor.Position = position;
    }
    
    // ToDecisions() inherited from ISpawnStrategy<T> (default implementation)
    // It wraps descriptors in SpawnDecision<AgentDescriptor>
    
    /// <summary>
    /// Determines spawn location based on agent category and props with spawn behaviors.
    /// Preserved from legacy implementation - critical game logic.
    /// </summary>
    private Position DetermineSpawnLocation(string category, GameState state)
    {
        var props = state.GetProps().ToList();
        _logger?.Log($"[SpawnStrategy] Analyzing {props.Count} props for category '{category}'...");
        
        if (category == "Survivor")
        {
            var spawnPoint = props.FirstOrDefault(p => p.HasBehavior<PartySpawn>());

            if (spawnPoint == null) 
            {
                 _logger?.LogWarning($"[SpawnStrategy] No prop with PartySpawn behavior found.");
                 return Position.Empty;
            }
            _logger?.Log($"[SpawnStrategy] Found spawn point at {spawnPoint.PositionComponent?.CurrentPosition}");
            return spawnPoint.PositionComponent?.CurrentPosition ?? Position.Empty;
        }
        else if (category == "Zombie")
        {
             var spawnPoint = state.GetProps()
                .FirstOrDefault(p => p.HasBehavior<ZombieSpawn>());

            if (spawnPoint == null) return Position.Empty;
            return spawnPoint.PositionComponent?.CurrentPosition ?? Position.Empty;
        }

        return Position.Empty;
    }
}
