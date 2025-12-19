using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Services.Interfaces;

public interface IGameCatalogApi
{
    void RegiterNpcDefinition(NpcTypeId typeId, NpcDefinition definition);
    void RegisterUnitDefinition(UnitTypeId typeId, UnitDefinition definition);
    void RegisterPropDefinition(PropTypeId typeId, PropDefinition definition);
    
    NpcDefinition GetNpcDefinition(NpcTypeId typeId);
    UnitDefinition GetUnitDefinition(UnitTypeId typeId);
    PropDefinition GetPropDefinition(PropTypeId typeId);

}