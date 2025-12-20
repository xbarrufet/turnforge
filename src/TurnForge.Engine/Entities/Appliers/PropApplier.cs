using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Events;
using TurnForge.Engine.Strategies.Spawn;

namespace TurnForge.Engine.Entities.Appliers;

public sealed class PropApplier(IGameEntityFactory<Prop> factory, IEffectSink effectsSink) : IBuildApplier<PropSpawnDecision, Prop>
{
    public GameState Apply(PropSpawnDecision decision, GameState state)
    {
        var prop = factory.Build(decision.Descriptor);
        effectsSink.Emit(new PropSpawnedEffect(prop.Id, prop.Definition.TypeId, decision.Position));
        return state.WithProp(prop);
    }
}
