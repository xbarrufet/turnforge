using System;

namespace TurnForge.Engine.Core.Attributes;

/// <summary>
/// Attribute to map a property containing a collection of IBehaviour to the entity's BehaviourComponent.
/// This attribute handles the special case of copying behaviours to the component's behaviour list.
/// </summary>
/// <remarks>
/// Unlike MapToComponent which works with simple properties, this attribute:
/// - Always maps to IBehaviourComponent
/// - Copies the behaviour collection (not by reference)
/// - Uses AddBehaviour() method to maintain component integrity
/// 
/// Example:
/// <code>
/// [MapToBehaviours]
/// public IReadOnlyList&lt;IBehaviour&gt; Behaviours { get; set; }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class MapToBehavioursAttribute : Attribute
{
    // No parameters needed - always maps to IBehaviourComponent.Behaviours
}
