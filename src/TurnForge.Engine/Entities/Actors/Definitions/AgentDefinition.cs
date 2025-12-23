namespace TurnForge.Engine.Entities.Actors.Definitions;

public abstract class AgentDefinition: BaseGameEntityDefinition
{
    protected AgentDefinition(string definitionId, string name, string category) : base(definitionId, name, category) {

    }

   public AgentDefinition():base()
    {
    }
    
}