namespace TurnForge.Engine.Entities.Actors.Definitions;

public abstract class AgentDefinition: BaseGameEntityDefinition
{
    protected AgentDefinition(string definitionId) : base(definitionId) {
    }

   public AgentDefinition():base()
    {
    }
    
}