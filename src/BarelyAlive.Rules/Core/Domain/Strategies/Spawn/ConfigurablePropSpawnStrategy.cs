using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Strategies.Spawn;

public class ConfigurablePropSpawnStrategy : IPropSpawnStrategy
{
    public IReadOnlyList<PropSpawnDecision> Decide(PropSpawnContext context)
    {
        // Simple default: Allow all spawning for now
        var decisions = new List<PropSpawnDecision>();
        foreach (var request in context.PropsToSpawn)
        {
            decisions.Add(new PropSpawnDecision(request.TypeId, request.Position ?? TurnForge.Engine.ValueObjects.Position.Empty, request.ExtraBehaviours));
        }
        return decisions;
    }
}
