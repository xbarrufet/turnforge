using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities; // for GameState
using BarelyAlive.Rules.Tests.Infrastructure;
using BarelyAlive.Rules.Core.Domain.Entities;

namespace BarelyAlive.Rules.Tests.Infrastructure.Strategies;

public class TestAgentSpawnStrategy : ISpawnStrategy<AgentDescriptor>
{
    public IReadOnlyList<AgentDescriptor> Process(
        IReadOnlyList<AgentDescriptor> descriptors,
        GameState state)
    {
        // Find PlayerSpawn prop in the state
        // We assume Prods (Props) are already loaded when agents are spawned
        // Props are entities.
        
        var playerSpawn = state.GetProps()
            .FirstOrDefault(e => e.DefinitionId == "BarelyAlive.Spawn" || e.DefinitionId == TestHelpers.SpawnPlayerId);
            
        // Disambiguate? There are 2 spawns.
        // ZombieSpawn is at {0,0}. PartySpawn (Player) is at {2,0}.
        // If we strictly follow types, we should check components.
        // But components might be internal to Rule engine.
        // Let's assume if we find any BarelyAlive.Spawn, we check components logic if accessible.
        // Or just pick the one that IS NOT ZombieSpawn (if we can identify ZombieSpawn).
        // ZombieSpawn has ZombieSpawnComponent (from factory).
        
        // Just take the last one? Or Filter.
        // Let's robustly look for the one that implies Player.
        // If multiple exist, we pick the first one that fits "PlayerSpawn" criteria.
        // Helper: in Mission01, PlayerSpawn is the second one loaded? Or we look for specific tile ID?
        // PlayerSpawn is at "dd05ee1d..." (x=2, y=0).
        // We can match by position if needed, but strategy should be dynamic.
        
        // Let's try to match by exclusion of Zombie components if possible, or by specific tile if known.
        // Better: look for DefinitionID "BarelyAlive.Spawn".
        // The one with "ZombieSpawn" component is Zombie.
        // The other is Player.
        
        if (playerSpawn == null)
        {
             // Fallback attempt
             return descriptors;
        }
        
        // Refinement: Find the one that has PartySpawn data?
        // Let's just grab the one at {2,0} for test simplicity if components are hard to reach?
        // Actually, let's do: FirstOrDefault(p => p.DefinitionId == "BarelyAlive.Spawn" && p.Position.Equals(new Position(new TileId(Guid.Parse("dd05ee1d-7a6e-4996-b0bc-a9acb50fe3de")))));
        // That is hardcoding the strategy to Mission01 specific logic.
        // User asked for "Estrategia Agentes: La position de la entity es la position del prod detipo PlayerSpawn".
        
        // Let's scan for "BarelyAlive.Spawn".
        var spawns = state.GetProps().Where(p => p.DefinitionId == "BarelyAlive.Spawn").ToList();
        // Assume the "PartySpawn" one (Player) is the one we want.
        // How to distinguish? Check components for "ZombieSpawnComponent".
        
        playerSpawn = spawns.FirstOrDefault(p => !p.Components.Any(c => c.GetType().Name.Contains("ZombieSpawn")));
        
        if (playerSpawn == null) return descriptors;

        foreach (var descriptor in descriptors)
        {
            descriptor.Position = playerSpawn.Position;
        }

        return descriptors;
    }
}
