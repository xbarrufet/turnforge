using TurnForge.Engine.Events;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Strategies.Spawn;

namespace TurnForge.Engine.Appliers.Entity;

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
            [new PropSpawnedEvent(
                prop.Id, 
                prop.DefinitionId, 
                prop.PositionComponent.CurrentPosition)]);
    }
}
