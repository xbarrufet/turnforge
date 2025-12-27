using System;
using TurnForge.Engine.Definitions;

namespace TurnForge.Engine.Core.Attributes;

/// <summary>
/// Specifies which Definition type this Entity is created from.
/// Applied to Entity classes to establish bidirectional Definition↔Entity mapping.
/// </summary>
/// <remarks>
/// This creates a compile-time verified link:
/// - Entity declares its Definition type via this attribute
/// - EntityTypeRegistry builds Definition→Entity mapping at startup
/// 
/// Enables compile-time safety: if Definition type doesn't exist, compilation fails.
/// 
/// Example:
/// <code>
/// [DefinitionType(typeof(SurvivorDefinition))]
/// [DescriptorType(typeof(SurvivorDescriptor))]
/// public class Survivor : Agent
/// {
///     // ... entity implementation
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DefinitionTypeAttribute : Attribute
{
    /// <summary>
    /// Gets the Definition type associated with this Entity.
    /// </summary>
    public Type DefinitionType { get; }
    
    /// <summary>
    /// Creates a new DefinitionType attribute.
    /// </summary>
    /// <param name="definitionType">The Definition type. Must inherit from BaseGameEntityDefinition.</param>
    /// <exception cref="ArgumentException">Thrown if definitionType doesn't inherit from BaseGameEntityDefinition.</exception>
    public DefinitionTypeAttribute(Type definitionType)
    {
        if (!typeof(BaseGameEntityDefinition).IsAssignableFrom(definitionType))
        {
            throw new ArgumentException(
                $"Type {definitionType.Name} must inherit from BaseGameEntityDefinition",
                nameof(definitionType));
        }
        
        DefinitionType = definitionType;
    }
}
