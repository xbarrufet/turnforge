using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.Definitions.BasicDefinitions;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Traits;

namespace TurnForge.Engine.Entities.Descriptors;

public class ZoneDescriptor : GameEntityBuildDescriptor
{
    public ZoneDescriptor(
        string name,
        ZoneId zoneId,
        IZoneTopology zoneTopology,
        string definitionId = BasicZoneDefinition.DefinitionId,
        IEnumerable<ITrait>? definitionTraitValues = null)
        : base(definitionId, name, null, definitionTraitValues)
    {
        // ZoneTrait removed - ZoneId and ZoneTopology are direct properties
        ZoneId = zoneId;
        ZoneTopology = zoneTopology;
    }

    public ZoneDescriptor(
        ZoneId zoneId,
        IZoneTopology zoneTopology,
        string definitionId = BasicZoneDefinition.DefinitionId,
        IEnumerable<ITrait>? definitionTraitValues = null)
        : base(definitionId, definitionId, null, definitionTraitValues)
    {
        // ZoneTrait removed - ZoneId and ZoneTopology are direct properties
        ZoneId = zoneId;
        ZoneTopology = zoneTopology;
    }

    public ZoneId ZoneId { get; init; }
    public IZoneTopology ZoneTopology { get; init; }

}
