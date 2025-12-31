using TurnForge.Engine.Traits.Standard;

namespace TurnForge.Engine.Entities.Definitions.Actors;

public abstract class ActorDefinition: BaseGameEntityDefinition
{
    protected ActorDefinition(string definitionId, string category) : base(definitionId, category) {
        AddTrait(new VitalityTrait());
        AddTrait(new PositionTrait());
        AddTrait(new TeamTrait());
    }


    
}   