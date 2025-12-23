using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

/// <summary>
/// Base class for entity build descriptors.
/// Provides common properties needed for entity creation.
/// </summary>
public class GameEntityBuildDescriptor(string definitionId) : IGameEntityBuildDescriptor
{
    public string DefinitionID { get; set; } = definitionId;
    public List<IGameEntityComponent> ExtraComponents { get; init; } = new();
    public Position? Position { get; set; } = null;
}
