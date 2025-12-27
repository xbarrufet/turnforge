namespace TurnForge.Engine.Entities.Actors.Definitions;

public abstract class PropDefinition : AgentDefinition
{
    protected PropDefinition(string definitionId) : base(definitionId)
    {
    }

    public PropDefinition():base()
    {
    }
}