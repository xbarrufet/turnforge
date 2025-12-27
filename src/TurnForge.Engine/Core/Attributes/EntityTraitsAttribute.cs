using TurnForge.Engine.Traits.Interfaces;

namespace TurnForge.Engine.Core.Attributes;

/// <summary>
/// Declares the Traits that a specific EntityDefinition implies.
/// e.g. [EntityTraits(typeof(PositionTrait), typeof(HealthTrait))]
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class EntityTraitsAttribute : Attribute
{
    public Type[] TraitTypes { get; }

    public EntityTraitsAttribute(params Type[] traitTypes)
    {
        // Validació opcional: assegurar que implementen IBaseTrait
        TraitTypes = traitTypes;
    }
}