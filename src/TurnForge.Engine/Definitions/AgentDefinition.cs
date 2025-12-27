using TurnForge.Engine.Traits.Standard;

namespace TurnForge.Engine.Definitions;

public abstract class AgentDefinition: ActorDefinition
{
    protected AgentDefinition(string definitionId, string category) : base(definitionId, category) {
    
        AddTrait(new ActionPointsTrait());
    }

   
    
}