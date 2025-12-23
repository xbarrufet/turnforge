namespace TurnForge.Engine.Entities.Actors.Definitions;

public abstract class PropDefinition : AgentDefinition
{
    protected PropDefinition(string definitionId, string name, string category) : base(definitionId, name, category)
    {
    }

    public PropDefinition():base()
    {
    }
}