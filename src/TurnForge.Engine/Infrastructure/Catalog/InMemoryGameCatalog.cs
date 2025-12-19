using BarelyAlive.Rules.Registration;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Registration;

namespace TurnForge.Engine.Infrastructure.Catalog;

internal sealed class InMemoryGameCatalog : IGameCatalog
{
    public IDefinitionRegistry<PropTypeId, PropDefinition> Props { get; }
    public IDefinitionRegistry<AgentTypeId, AgentDefinition> Agents { get; }

    public void RegisterAgentDefinition(AgentTypeId typeId, AgentDefinition definition)
    {
        Agents.Register(typeId, definition);
    }

    public void RegisterPropDefinition(PropTypeId typeId, PropDefinition definition)
    {
        Props.Register(typeId, definition);
    }

    public AgentDefinition GetAgentDefinition(AgentTypeId typeId)
    {
        return Agents.Get(typeId);
    }

    public PropDefinition GetPropDefinition(PropTypeId typeId)
    {
        return Props.Get(typeId);
    }

    public InMemoryGameCatalog()
    {
        Props = new InMemoryDefinitionRegistry<PropTypeId, PropDefinition>();
        Agents = new InMemoryDefinitionRegistry<AgentTypeId, AgentDefinition>();
    }

}