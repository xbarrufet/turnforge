using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities; // for GameState
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
            .FirstOrDefault(e => e.DefinitionId == "BarelyAlive.Spawn" || e.DefinitionId == TestHelpers.SpawnPlayerId);
            
        // Refinement: Find the one that implies Player.
        // Let's scan for "BarelyAlive.Spawn".
        var spawns = state.GetProps().Where(p => p.DefinitionId == "BarelyAlive.Spawn").ToList();
        
        // Assume the one WITHOUT ZombieSpawnComponent is PlayerSpawn
        playerSpawn = spawns.FirstOrDefault(p => !p.Components.Any(c => c.GetType().Name.Contains("ZombieSpawn")));
        
        if (playerSpawn == null)
        {
             return descriptors;
        }

        foreach (var descriptor in descriptors)
        {
            descriptor.Position = playerSpawn.PositionComponent.CurrentPosition;
        }

        return descriptors;
    }
}
