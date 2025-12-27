using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities; 
using TurnForge.Engine.ValueObjects;
using BarelyAlive.Rules.Tests.Infrastructure;
using System.Linq;

namespace BarelyAlive.Rules.Tests.Infrastructure.Strategies;

public class TestPropSpawnStrategy : ISpawnStrategy<PropDescriptor>
{
    public IReadOnlyList<PropDescriptor> Process(
        IReadOnlyList<PropDescriptor> descriptors,
        GameState state)
    {
        // 1. Find ZombieSpawn position (using new ID)
        Position zombieSpawnPos = Position.Empty;
        
        var zombieProp = state.GetProps().FirstOrDefault(p => 
            p.DefinitionId == TestHelpers.SpawnZombieId); 
            
        if (zombieProp != null)
        {
             zombieSpawnPos = zombieProp.PositionComponent.CurrentPosition;
        }
        
        if (zombieSpawnPos == Position.Empty)
        {
            // Check in descriptors being spawned
            var zDesc = descriptors.FirstOrDefault(d => d.DefinitionId == TestHelpers.SpawnZombieId);
            var zTrait = zDesc?.RequestedTraits.OfType<TurnForge.Engine.Traits.Standard.PositionTrait>().FirstOrDefault();
            if(zTrait != null)
            {
                zombieSpawnPos = zTrait.InitialPosition;
            }
        }
        
        if (zombieSpawnPos == Position.Empty)
        {
            return descriptors;
        }

        // 2. Assign position to those missing it (e.g. if logic requires)
        foreach (var descriptor in descriptors)
        {
            var hasPos = descriptor.RequestedTraits.OfType<TurnForge.Engine.Traits.Standard.PositionTrait>().Any();
            if (!hasPos)
            {
                descriptor.RequestedTraits.Add(new TurnForge.Engine.Traits.Standard.PositionTrait(zombieSpawnPos));
            }
        }

        return descriptors;
    }
}
