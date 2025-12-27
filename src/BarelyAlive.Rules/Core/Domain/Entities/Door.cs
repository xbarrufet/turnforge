using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Domain.Entities;


[DefinitionType(typeof(DoorDefinition))]
[DescriptorType(typeof(DoorDescriptor))]
public class Door : Prop
{
    public Door(EntityId id, string definitionId, string name, string category) : base(id, definitionId, name, category)
    {
        AddComponent(new ColorComponent());
    }
}

public enum Color
{
    Red,
    Green,
    Blue
}

public class ColorComponent : IGameEntityComponent
{
    public Color Color { get; set; }

    public ColorComponent() { }

    public ColorComponent(BarelyAlive.Rules.Core.Domain.Traits.ColorTrait trait)
    {
        Color = trait.Color;
    }
}


/// <summary>
/// Descriptor for Door props with color customization.
/// </summary>
/// <remarks>
/// Provides two constructors:
/// - Single-parameter (definitionId): For DescriptorBuilder reflection-based instantiation
/// - Two-parameter (definitionId, color): For explicit color specification in tests/code
/// </remarks>
public class DoorDescriptor : PropDescriptor
{
    public Color Color { get; set; }
    
    /// <summary>
    /// Default constructor for DescriptorBuilder (reflection-based instantiation).
    /// Color will be set to default value or via PropertyOverrides.
    /// </summary>
    public DoorDescriptor(string definitionId) : base(definitionId)
    {
        Color = Color.Red; // Default color
    }
    
    /// <summary>
    /// Explicit constructor for tests and direct usage with color specification.
    /// </summary>
    public DoorDescriptor(string definitionId, Color color) : base(definitionId)
    {
        Color = color;
    }
}


public class DoorDefinition(string definitionId) : PropDefinition(definitionId)
{}