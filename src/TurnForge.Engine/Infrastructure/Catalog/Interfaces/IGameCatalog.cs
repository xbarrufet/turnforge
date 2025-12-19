using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Registration;

namespace TurnForge.Engine.Infrastructure.Catalog.Interfaces;

public interface IGameCatalog
{
    IDefinitionRegistry<PropTypeId, PropDefinition> Props { get; }
    IDefinitionRegistry<UnitTypeId, UnitDefinition> Units { get; }
    IDefinitionRegistry<NpcTypeId, NpcDefinition> Npcs { get; }
    
    void RegiterNpcDefinition(NpcTypeId typeId, NpcDefinition definition);
    void RegisterUnitDefinition(UnitTypeId typeId, UnitDefinition definition);
    void RegisterPropDefinition(PropTypeId typeId, PropDefinition definition);
    
    NpcDefinition GetNpcDefinition(NpcTypeId typeId);
    UnitDefinition GetUnitDefinition(UnitTypeId typeId);
    PropDefinition GetPropDefinition(PropTypeId typeId);
}