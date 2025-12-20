using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Strategies.Spawn;

public sealed class BarelyAlivePropSpawnStrategy : IPropSpawnStrategy
{


    public IReadOnlyList<PropSpawnDecision> Decide(PropSpawnContext context)
    {
        List<PropSpawnDecision> decisions = new();
        foreach (var descriptor in context.PropsToSpawn)
        {
            decisions.Add(new PropSpawnDecision(
                descriptor.TypeId,
                descriptor.Position ?? Position.Empty,
                descriptor.ExtraBehaviours ?? Array.Empty<IActorBehaviour>()));
        }
        return decisions;
    }
}