using System.Collections.Generic;
using System.Collections.Immutable;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Values;

namespace TurnForge.Engine.Components;

/// <summary>
/// Component that provides arbitrary named stats (Attributes) to an entity.
/// Data-driven alternative/complement to specific component fields.
/// </summary>
public class AttributeComponent : IGameEntityComponent
{
    private readonly ImmutableDictionary<string, AttributeValue> _attributes;

    public AttributeComponent(IDictionary<string, AttributeValue> attributes)
    {
        _attributes = attributes.ToImmutableDictionary();
    }
    
    private AttributeComponent(ImmutableDictionary<string, AttributeValue> attributes)
    {
        _attributes = attributes;
    }

    public AttributeValue? Get(string name) => _attributes.TryGetValue(name, out var val) ? val : null;
    
    /// <summary>
    /// Returns a new instance of AttributeComponent with the updated value.
    /// Preserves immutability of the component state.
    /// </summary>
    public AttributeComponent Set(string name, AttributeValue value)
    {
        return new AttributeComponent(_attributes.SetItem(name, value));
    }
    
    public IReadOnlyDictionary<string, AttributeValue> GetAll() => _attributes;
}
