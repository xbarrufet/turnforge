using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Board;

public sealed class Connection : GameEntity
{
    public static Category ConnectionCategory = new("ConnectionCategory");

    // Direct properties (structural data)
    public ZoneId From { get; init; }
    public ZoneId To { get; init; }
    public IZoneConnectionPosition ConnectionPosition { get; init; }

    // Constructor for Builder (with all properties)
    public Connection(
        EntityId id,
        string definitionId,
        string name,
        Category category,
        ZoneId from,
        ZoneId to,
        IZoneConnectionPosition connectionPosition)
        : base(id, name, category, definitionId)
    {
        From = from;
        To = to;
        ConnectionPosition = connectionPosition;
    }

    public Connection(EntityId id, string definitionId, string name, Category category)
        : base(id, name, category, definitionId)
    {
        // ConnectionTrait removed - From, To, ConnectionPosition are direct properties
        // These will be set by the Builder from the Descriptor
    }
}