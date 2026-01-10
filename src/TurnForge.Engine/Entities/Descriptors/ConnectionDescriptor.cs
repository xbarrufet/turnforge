using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.Definitions.BasicDefinitions;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Traits;

namespace TurnForge.Engine.Entities.Descriptors;

public class ConnectionDescriptor : GameEntityBuildDescriptor
{
    public ConnectionDescriptor(
        string name,
        ZoneId from,
        ZoneId to,
        IZoneConnectionPosition connectionPosition,
        string definitionId = BasicConnectionDefinition.DefinitionId,

        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? definitionTraitValues = null)
        : base(definitionId, name, extraComponents, definitionTraitValues)
    {
        // ConnectionTrait removed - From, To, ConnectionPosition are direct properties
        From = from;
        To = to;
        ConnectionPosition = connectionPosition;
    }

    public ConnectionDescriptor(
        ZoneId from,
        ZoneId to,
        IZoneConnectionPosition connectionPosition,
        string definitionId = BasicConnectionDefinition.DefinitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? definitionTraitValues = null)
        : base(definitionId, definitionId, extraComponents, definitionTraitValues)
    {
        // ConnectionTrait removed - From, To, ConnectionPosition are direct properties
        From = from;
        To = to;
        ConnectionPosition = connectionPosition;
    }

    public ZoneId From { get; init; }
    public ZoneId To { get; init; }
    public IZoneConnectionPosition ConnectionPosition { get; init; }
}