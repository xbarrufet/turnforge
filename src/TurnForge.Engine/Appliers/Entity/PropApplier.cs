using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Strategies.Spawn;

namespace TurnForge.Engine.Entities.Appliers;

/// <summary>
/// Applier that adds a pre-created Prop entity to the GameState.
/// The Strategy is responsible for creating the entity via Factory.
/// </summary>
public sealed class PropApplier : ISpawnApplier<PropSpawnDecision, Prop>
{
    public ApplierResponse Apply(PropSpawnDecision decision, GameState state)
    {
        var prop = decision.Entity;
        
        return new ApplierResponse(
            state.WithProp(prop), 
            [new PropSpawnedEffect(
                prop.Id, 
                prop.DefinitionId, 
                prop.PositionComponent.CurrentPosition)]);
    }
}
