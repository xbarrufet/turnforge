using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class Zone : GameEntity
{
    public static readonly Category ZoneDefaultCategory = new("ZoneCategory");

    // Direct properties (structural data)
    public ZoneId ZoneId { get; init; }
    public IZoneTopology ZoneTopology { get; init; }

    // Constructor for Builder (with all properties)
    public Zone(
        EntityId id,
        string definitionId,
        string name,
        Category category,
        ZoneId zoneId,
        IZoneTopology zoneTopology)
        : base(id, name: name, category: category, definitionId: definitionId)
    {
        ZoneId = zoneId;
        ZoneTopology = zoneTopology;
    }

    public Zone(EntityId id, string definitionId)
        : base(id, name: definitionId, category: ZoneDefaultCategory, definitionId: definitionId)
    {
        // ZoneTrait removed - ZoneId and ZoneTopology are direct properties
        // These will be set by the Builder from the Descriptor
    }

    public Zone(EntityId id, string definitionId, Category category)
        : base(id, name: definitionId, category: category, definitionId: definitionId)
    {
        // ZoneTrait removed - ZoneId and ZoneTopology are direct properties
        // These will be set by the Builder from the Descriptor
    }

    public Zone(EntityId id, string definitionId, string name, Category category)
        : base(id, name: name, category: category, definitionId: definitionId)
    {
        // ZoneTrait removed - ZoneId and ZoneTopology are direct properties
        // These will be set by the Builder from the Descriptor
    }

}
