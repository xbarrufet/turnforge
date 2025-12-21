using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Strategies.Spawn;

namespace TurnForge.Engine.Entities.Appliers;

public sealed class PropApplier(IGameEntityFactory<Prop> factory) : IBuildApplier<PropSpawnDecision, Prop>
{
    public ApplierResponse Apply(PropSpawnDecision decision, GameState state)
    {
        var prop = factory.Build(decision.Descriptor);
        return new ApplierResponse(state.WithProp(prop), [new PropSpawnedEffect(prop.Id, prop.Definition.TypeId, decision.Position)]);
    }
}
