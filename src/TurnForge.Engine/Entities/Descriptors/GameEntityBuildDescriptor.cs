using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

/// <summary>
/// Base class for entity build descriptors.
/// Provides common properties needed for entity creation.
/// </summary>
public class GameEntityBuildDescriptor(string definitionId) : IGameEntityBuildDescriptor
{
    public string DefinitionId { get; set; } = definitionId;
    public List<IGameEntityComponent> ExtraComponents { get; init; } = new();

    /// <summary>
    /// Traits requested for this spawn override. 
    /// These will be added to the entity, potentially overriding definition traits.
    /// </summary>
    public List<TurnForge.Engine.Traits.Interfaces.IBaseTrait> RequestedTraits { get; } = new();
}

