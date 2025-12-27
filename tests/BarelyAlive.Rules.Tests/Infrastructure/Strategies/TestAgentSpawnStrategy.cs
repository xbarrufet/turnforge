using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions; // for GameState
using TurnForge.Engine.ValueObjects; // for Position
using BarelyAlive.Rules.Tests.Infrastructure;
using BarelyAlive.Rules.Core.Domain.Entities;
using System.Linq;

namespace BarelyAlive.Rules.Tests.Infrastructure.Strategies;

public class TestAgentSpawnStrategy : ISpawnStrategy<AgentDescriptor>
{
    public IReadOnlyList<AgentDescriptor> Process(
        IReadOnlyList<AgentDescriptor> descriptors,
        GameState state)
    {
        // Find PlayerSpawn prop in the state
        // Props are entities.
        
        var playerSpawn = state.GetProps()
            .FirstOrDefault(e => e.DefinitionId == "Spawn.Player" || e.DefinitionId == TestHelpers.SpawnPlayerId);
            
        // Refinement: Find the one that implies Player.
        // Let's scan for "BarelyAlive.Spawn".
        var spawns = state.GetProps().Where(p => p.DefinitionId == "Spawn.Player" || p.DefinitionId == TestHelpers.SpawnPlayerId).ToList();
        
        // Assume the one WITHOUT ZombieSpawnComponent is PlayerSpawn
        playerSpawn = spawns.FirstOrDefault(p => !p.Components.Any(c => c.GetType().Name.Contains("ZombieSpawn")));
        
        if (playerSpawn == null)
        {
             System.Console.WriteLine("[TestAgentSpawnStrategy] PlayerSpawn NOT found! Returning descriptors unmodified.");
             return descriptors;
        }

        System.Console.WriteLine($"[TestAgentSpawnStrategy] PlayerSpawn FOUND at {playerSpawn.PositionComponent.CurrentPosition}. Updating {descriptors.Count} descriptors.");

        foreach (var descriptor in descriptors)
        {
            // Update Position via Traits
            var positionTrait = descriptor.RequestedTraits.OfType<TurnForge.Engine.Traits.Standard.PositionTrait>().FirstOrDefault();
            if (positionTrait != null)
            {
                 // Replace existing
                 descriptor.RequestedTraits.Remove(positionTrait);
            }
            descriptor.RequestedTraits.Add(new TurnForge.Engine.Traits.Standard.PositionTrait(playerSpawn.PositionComponent.CurrentPosition));
        }

        return descriptors;
    }
}
