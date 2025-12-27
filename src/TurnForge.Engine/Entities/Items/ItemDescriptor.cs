using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Entities.Items;

/// <summary>
/// Descriptor for spawning Item instances.
/// </summary>
/// <remarks>
/// Unlike AgentDescriptor, this does NOT have a Position property
/// since Items cannot exist on the board directly.
/// </remarks>
public class ItemDescriptor : IGameEntityBuildDescriptor
{
    /// <summary>
    /// Definition ID to look up in the catalog.
    /// </summary>
    /// <summary>
    /// Definition ID to look up in the catalog.
    /// </summary>
    public string DefinitionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Extra components to add (implements interface requirement).
    /// </summary>
    public List<IGameEntityComponent> ExtraComponents { get; init; } = new();

    /// <summary>
    /// Required by IGameEntityBuildDescriptor interface.
    /// Items generally don't use trait overrides yet, but must support the interface.
    /// </summary>
    public List<TurnForge.Engine.Traits.Interfaces.IBaseTrait> RequestedTraits { get; } = new();
    
    /// <summary>
    /// Optional override for item name.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Optional attribute overrides.
    /// </summary>
    public Dictionary<string, object>? AttributeOverrides { get; set; }
    
    public ItemDescriptor() { }
    
    public ItemDescriptor(string definitionId)
    {
        DefinitionId = definitionId;
    }
    
    /// <summary>
    /// Fluent builder for setting name.
    /// </summary>
    public ItemDescriptor WithName(string name)
    {
        Name = name;
        return this;
    }
    
    /// <summary>
    /// Fluent builder for adding attribute override.
    /// </summary>
    public ItemDescriptor WithAttribute(string key, object value)
    {
        AttributeOverrides ??= new();
        AttributeOverrides[key] = value;
        return this;
    }
}
