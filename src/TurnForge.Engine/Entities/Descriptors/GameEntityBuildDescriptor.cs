using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

/// <summary>
/// Base class for entity build descriptors.
/// Provides common properties needed for entity creation.
/// </summary>
public class GameEntityBuildDescriptor(string definitionId, string name) : IGameEntityBuildDescriptor
{
    public string DefinitionId { get; set; } = definitionId;
    public string Name { get; set; } = name;
    public List<IGameEntityComponent> ExtraComponents { get; init; } = new();

    public GameEntityBuildDescriptor(string definitionId, string name, IEnumerable<IGameEntityComponent>? extraComponents = null, IEnumerable<ITrait>? definitionTraitValues = null) : this(definitionId, name)
    {
        ExtraComponents = extraComponents?.ToList() ?? new List<IGameEntityComponent>();
        DefinitionTraitValues = definitionTraitValues?.ToList() ?? new List<ITrait>();
    }


    /// <summary>
    /// Initial values for traits defined in the Definition.
    /// These provide instance-specific values for traits that already exist in the template.
    /// Example: A spawn zone Definition has a ColorTrait, this provides the specific color value.
    /// </summary>
    public List<ITrait> DefinitionTraitValues { get; } = new();

    public bool TryGetTraitValue(Type traitType, out ITrait? trait)
    {
        trait = DefinitionTraitValues.FirstOrDefault(t =>
            t.GetType() == traitType || traitType.IsAssignableFrom(t.GetType()));
        return trait != null;
    }

    public bool TryGetExtraComponent(Type componentType, out IGameEntityComponent? component)
    {
        component = ExtraComponents.FirstOrDefault(c =>
            c.GetType() == componentType || componentType.IsAssignableFrom(c.GetType()));
        return component != null;
    }

    public void AddExtraComponent(IGameEntityComponent component)
    {
        ExtraComponents.Add(component);
    }

    public void AddDefinitionTraitValue(ITrait trait)
    {
        DefinitionTraitValues.Add(trait);
    }
}

