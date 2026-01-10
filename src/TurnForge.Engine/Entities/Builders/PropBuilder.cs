using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.TraitsComponents.Components;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.Services;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Builders;

/// <summary>
/// Type-safe builder for Prop entities and their subclasses.
/// Supports custom Prop subclasses (e.g., Chest, Door, Trap) while maintaining type safety.
/// Optimized for performance and compile-time safety.
/// 
/// Example usage:
/// <code>
/// // For base Prop
/// var prop = builder.Build(descriptor, definition);
/// 
/// // For custom Chest : Prop
/// var chest = builder.Build&lt;Chest&gt;(descriptor, definition);
/// </code>
/// </summary>
public sealed class PropBuilder
{
    private readonly ComponentInitializationService _traitService;

    public PropBuilder(ComponentInitializationService traitService)
    {
        _traitService = traitService;
    }

    /// <summary>
    /// Builds a base Prop entity from descriptor and definition.
    /// </summary>
    public Prop Build(PropDescriptor descriptor, PropDefinition definition)
    {
        // Build Prop with StartPosition from descriptor
        var prop = new Prop(
            EntityId.New(),
            descriptor.DefinitionId,
            descriptor.Name,
            definition.Category,
            descriptor.StartPosition
        );

        // 2. Initialize traits from definition
        foreach (var trait in definition.Traits)
        {
            prop.AddTrait((dynamic)trait);
        }

        // 3. Apply trait overrides from descriptor if any
        var overrides = descriptor.DefinitionTraitValues;
        if (overrides != null)
        {
            foreach (var trait in overrides)
            {
                var traitType = trait.GetType();
                var removeMethod = typeof(GameEntity).GetMethod("RemoveTrait");
                removeMethod?.MakeGenericMethod(traitType)?.Invoke(prop, null);
                prop.AddTrait((dynamic)trait);
            }
        }

        // 4. Initialize components from traits
        _traitService.InitializeComponents(prop);

        // 5. Add extra components from descriptor if any
        var components = descriptor.ExtraComponents;
        if (components != null)
        {
            foreach (var component in components)
            {
                prop.AddComponent((dynamic)component);
            }
        }

        return prop;
    }
}
