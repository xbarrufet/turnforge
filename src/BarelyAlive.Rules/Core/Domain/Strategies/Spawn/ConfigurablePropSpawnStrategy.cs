using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Strategies.Spawn;

/// <summary>
/// Strategy that creates Prop entities from descriptors and wraps them in spawn decisions.
/// Uses GenericActorFactory to create the actual entities.
/// </summary>
public class ConfigurablePropSpawnStrategy(GenericActorFactory factory) : IPropSpawnStrategy
{
    public IReadOnlyList<PropSpawnDecision> Decide(PropSpawnContext context)
    {
        var decisions = new List<PropSpawnDecision>();
        
        foreach (var descriptor in context.PropsToSpawn)
        {
            // Create the entity using the factory
            var prop = factory.BuildProp(descriptor);
            
            // Wrap in decision
            decisions.Add(new PropSpawnDecision(prop));
        }
        
        return decisions;
    }
}
