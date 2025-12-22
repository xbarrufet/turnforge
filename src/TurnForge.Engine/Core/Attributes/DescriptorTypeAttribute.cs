using System;

namespace TurnForge.Engine.Core.Attributes;

/// <summary>
/// Attribute to specify which descriptor type should be created for a given Definition.
/// Similar to [EntityType] but for descriptors.
/// </summary>
/// <remarks>
/// This allows definitions to specify custom descriptor types that extend the base descriptor
/// with additional properties or behavior.
/// 
/// Example:
/// <code>
/// [DescriptorType(typeof(SurvivorDescriptor))]
/// [EntityType(typeof(Survivor))]
/// public class SurvivorDefinition : GameEntityDefinition
/// {
///     // ...
/// }
/// </code>
/// 
/// When spawning from this definition, the system will create a SurvivorDescriptor
/// instead of the default AgentDescriptor.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DescriptorTypeAttribute : Attribute
{
    /// <summary>
    /// The concrete descriptor type to instantiate for this definition
    /// </summary>
    public Type DescriptorType { get; }
    
    /// <summary>
    /// Creates a new DescriptorType attribute
    /// </summary>
    /// <param name="descriptorType">Type of descriptor to create (must implement IGameEntityDescriptor)</param>
    public DescriptorTypeAttribute(Type descriptorType)
    {
        if (descriptorType == null)
            throw new ArgumentNullException(nameof(descriptorType));
        
        // Note: Cannot check interface implementation at attribute construction time
        // due to generic constraints. Validation happens at runtime in DescriptorBuilder.
        DescriptorType = descriptorType;
    }
}
