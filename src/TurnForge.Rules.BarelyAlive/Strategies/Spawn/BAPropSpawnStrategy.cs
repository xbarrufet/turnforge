using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Rules.BarelyAlive.Actors;

namespace TurnForge.Rules.BarelyAlive.Strategies.Spawn;

public sealed class BAPropSpawnStrategy:IPropSpawnStrategy
{ 
    

    public IReadOnlyList<PropSpawnDecision> Decide(PropSpawnContext context)
    {
        List<PropSpawnDecision> decisions = new();
        foreach (var descriptor in context.PropsToSpawn)
        {
            decisions.Add(new PropSpawnDecision(
                descriptor.TypeId,
                descriptor.Position ?? Position.Zero,
                descriptor.ExtraBehaviours ?? Array.Empty<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()));
        }
        return decisions;
    }
}