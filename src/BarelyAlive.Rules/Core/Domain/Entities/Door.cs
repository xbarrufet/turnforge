using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Domain.Entities;

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
}

[EntityType(typeof(Door))]
public class DoorDescriptor(string definitionId, Color color) : PropDescriptor(definitionId)
{
    [MapToComponent(typeof(ColorComponent), nameof(Color))]
    public Color Color { get; set; } = color;
}

[EntityType(typeof(Door))]
public class DoorDefinition(string definitionId, string name, string category) : PropDefinition(definitionId, name, category)
{}