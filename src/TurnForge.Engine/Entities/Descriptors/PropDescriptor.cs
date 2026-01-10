using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions.BasicDefinitions;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

public class PropDescriptor : GameEntityBuildDescriptor
{
    // Direct property for Prop-specific data
    public IBoardPositionId StartPosition { get; init; }

    public PropDescriptor(
        string name,
        IBoardPositionId startPosition,
        string definitionId = BasicPropDefinition.DefinitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? definitionTraitValues = null)
        : base(definitionId, name, extraComponents, definitionTraitValues)
    {
        // MovementComponent removed - StartPosition is a direct property
        StartPosition = startPosition;
    }

    public PropDescriptor(
        IBoardPositionId startPosition,
        string definitionId = BasicPropDefinition.DefinitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? definitionTraitValues = null)
        : base(definitionId, definitionId, extraComponents, definitionTraitValues)
    {
        // MovementComponent removed - StartPosition is a direct property
        StartPosition = startPosition;
    }

    public PropDescriptor(
        string name,
        string definitionId = BasicPropDefinition.DefinitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? definitionTraitValues = null)
        : base(definitionId, name, extraComponents, definitionTraitValues)
    {
        // MovementComponent removed - StartPosition is a direct property
        StartPosition = IBoardPositionId.Limbo;
    }

    public PropDescriptor(
        string definitionId = BasicPropDefinition.DefinitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? definitionTraitValues = null)
        : base(definitionId, definitionId, extraComponents, definitionTraitValues)
    {
        // MovementComponent removed - StartPosition is a direct property
        StartPosition = IBoardPositionId.Limbo;
    }
}
