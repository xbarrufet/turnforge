using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.Services;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Builders;

/// <summary>
/// Type-safe builder for Zone entities.
/// Optimized for performance and compile-time safety.
/// </summary>
public sealed class ZoneBuilder
{
    private readonly ComponentInitializationService _traitService;

    public ZoneBuilder(ComponentInitializationService traitService)
    {
        _traitService = traitService;
    }

    /// <summary>
    /// Builds a Zone entity from descriptor and definition.
    /// </summary>
    public Zone Build(ZoneDescriptor descriptor, ZoneDefinition definition)
    {
        // Build Zone with ZoneId and ZoneTopology from descriptor
        var zone = new Zone(
            EntityId.New(),
            descriptor.DefinitionId,
            descriptor.Name,
            definition.Category,
            descriptor.ZoneId,
            descriptor.ZoneTopology
        );

        // 2. Initialize traits from definition
        foreach (var trait in definition.Traits)
        {
            zone.AddTrait((dynamic)trait);
        }

        // 3. Apply trait overrides from descriptor if any
        var overrides = descriptor.DefinitionTraitValues;
        if (overrides != null)
        {
            foreach (var trait in overrides)
            {
                var traitType = trait.GetType();
                var removeMethod = typeof(GameEntity).GetMethod("RemoveTrait");
                removeMethod?.MakeGenericMethod(traitType)?.Invoke(zone, null);
                zone.AddTrait((dynamic)trait);
            }
        }

        // 4. Initialize components from traits
        _traitService.InitializeComponents(zone);

        // 5. Add extra components from descriptor if any
        var components = descriptor.ExtraComponents;
        if (components != null)
        {
            foreach (var component in components)
            {
                zone.AddComponent((dynamic)component);
            }
        }

        return zone;
    }
}

