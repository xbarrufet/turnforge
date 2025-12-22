using BarelyAlive.Rules.Core.Behaviours.ActorBehaviours;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Domain.Strategies.Spawn;

/// <summary>
/// Strategy that determines spawn positions for agents and creates the entities.
/// Uses GenericActorFactory to create Agent entities with proper positioning.
/// </summary>
public class ConfigurableAgentSpawnStrategy(
    GenericActorFactory factory,
    TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null) : IAgentSpawnStrategy
{
    private readonly TurnForge.Engine.Core.Interfaces.IGameLogger? _logger = logger;

    private Position DetermineSpawnLocation(string category, AgentSpawnContext ctx)
    {
        var props = ctx.GameState.GetProps().ToList();
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
             var spawnPoint = ctx.GameState.GetProps()
                .FirstOrDefault(p => p.HasBehavior<ZombieSpawn>());

            if (spawnPoint == null) return Position.Empty;
            return spawnPoint.PositionComponent?.CurrentPosition ?? Position.Empty;
        }

        return Position.Empty;
    }

    public IReadOnlyList<AgentSpawnDecision> Decide(AgentSpawnContext ctx)
    {
        var decisions = new List<AgentSpawnDecision>();
        
        foreach (var descriptor in ctx.AgentsToSpawn)
        {
            var spawnPosition = DetermineSpawnLocation(descriptor.DefinitionID, ctx);
            
            if (spawnPosition == Position.Empty)
            {
                _logger?.LogWarning($"[SpawnStrategy] No spawn position found for {descriptor.DefinitionID}.");
            }
            
            // TODO: Update descriptor with position before building
            // For now, create agent and manually set position
            var agent = factory.BuildAgent(descriptor);
            
            // Set the determined spawn position
            if (spawnPosition != Position.Empty)
            {
                agent.PositionComponent.CurrentPosition = spawnPosition;
            }
            
            // Wrap in decision
            decisions.Add(new AgentSpawnDecision(agent));
        }
        
        return decisions;
    }
}
