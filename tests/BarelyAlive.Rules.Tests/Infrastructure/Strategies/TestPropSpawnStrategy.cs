using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities; // for GameState
using BarelyAlive.Rules.Tests.Infrastructure;
using System.Linq;

namespace BarelyAlive.Rules.Tests.Infrastructure.Strategies;

public class TestPropSpawnStrategy : ISpawnStrategy<PropDescriptor>
{
    public IReadOnlyList<PropDescriptor> Process(
        IReadOnlyList<PropDescriptor> descriptors,
        GameState state)
    {
        // 1. Find ZombieSpawn position
        // It might be in the current descriptors list (if being spawned now) or in the state (if already spawned)
        // Usually Props are bulk spawned.
        
        // 1. Find ZombieSpawn position
        // "BarelyAlive.Spawn" + ZombieSpawnComponent
        
        Position? zombieSpawnPos = null;
        
        // Look in state first (Props)
        var zombieProp = state.GetProps().FirstOrDefault(p => 
            (p.DefinitionId == "BarelyAlive.Spawn" || p.DefinitionId == TestHelpers.SpawnZombieId) && 
            p.Components.Any(c => c.GetType().Name.Contains("ZombieSpawn"))); // Identifies ZombieSpawn
            
        zombieSpawnPos = zombieProp?.Position;
        
        if (zombieSpawnPos == null)
        {
            // Check in descriptors (if being spawned now - unlikely for Props as they batch spawn, but if split)
            // Descriptors don't have components instantiated easily visible like entities?
            // Actually PropDescriptor has ExtraComponents list.
            var zDesc = descriptors.FirstOrDefault(d => 
                (d.DefinitionID == "BarelyAlive.Spawn" || d.DefinitionID == TestHelpers.SpawnZombieId) &&
                d.ExtraComponents.Any(c => c.GetType().Name.Contains("ZombieSpawn")));
                
            zombieSpawnPos = zDesc?.Position;
        }
        
        if (zombieSpawnPos == null)
        {
            // Fallback or just ignore modification if reference point is missing
            return descriptors;
        }

        // 2. Assign position if missing
        foreach (var descriptor in descriptors)
        {
            if (descriptor.Position == null)
            {
                descriptor.Position = zombieSpawnPos;
            }
        }

        return descriptors;
    }
}
