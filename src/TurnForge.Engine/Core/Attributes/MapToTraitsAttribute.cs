using System;

namespace TurnForge.Engine.Core.Attributes;

/// <summary>
/// Attribute to map a property containing a collection of ITrait to the entity's TraitContainerComponent.
/// This attribute handles the special case of copying traits to the component's trait list.
/// </summary>
/// <remarks>
/// Unlike MapToComponent which works with simple properties, this attribute:
/// - Always maps to ITraitContainerComponent
/// - Copies the trait collection (not by reference)
/// - Uses AddTrait() method to maintain component integrity
/// 
/// Example:
/// <code>
/// [MapToTraits]
/// public IReadOnlyList&lt;ITrait&gt; Traits { get; set; }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class MapToTraitsAttribute : Attribute
{
    // No parameters needed - always maps to ITraitContainerComponent.Traits
}
