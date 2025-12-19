using BarelyAlive.Rules.Registration;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Registration;

namespace TurnForge.Engine.Infrastructure.Catalog;

internal sealed class InMemoryGameCatalog : IGameCatalog
{
    public IDefinitionRegistry<PropTypeId, PropDefinition> Props { get; }
    public IDefinitionRegistry<UnitTypeId, UnitDefinition> Units { get; }
    public IDefinitionRegistry<NpcTypeId, NpcDefinition> Npcs { get; }
    public void RegiterNpcDefinition(NpcTypeId typeId, NpcDefinition definition)
    {
        Npcs.Register(typeId, definition);
    }

    public void RegisterUnitDefinition(UnitTypeId typeId, UnitDefinition definition)
    {
        Units.Register(typeId, definition);
    }

    public void RegisterPropDefinition(PropTypeId typeId, PropDefinition definition)
    {
        Props.Register(typeId, definition);
    }

    public NpcDefinition GetNpcDefinition(NpcTypeId typeId)
    {
        return Npcs.Get(typeId);
    }

    public UnitDefinition GetUnitDefinition(UnitTypeId typeId)
    {
        return Units.Get(typeId);
    }

    public PropDefinition GetPropDefinition(PropTypeId typeId)
    {
        return Props.Get(typeId);
    }

    public InMemoryGameCatalog()
    {
        Props = new InMemoryDefinitionRegistry<PropTypeId, PropDefinition>();
        Units = new InMemoryDefinitionRegistry<UnitTypeId, UnitDefinition>();
        Npcs  = new InMemoryDefinitionRegistry<NpcTypeId, NpcDefinition>();
    }
    
}