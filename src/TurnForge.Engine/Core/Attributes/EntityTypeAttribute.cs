using System;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Core.Attributes;

/// <summary>
/// Attribute to specify which concrete entity type should be instantiated
/// for a given Definition or Descriptor.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class EntityTypeAttribute : Attribute
{
    public Type EntityType { get; }
    
    public EntityTypeAttribute(Type entityType)
    {
        if (!typeof(GameEntity).IsAssignableFrom(entityType))
        {
            throw new ArgumentException(
                $"Type {entityType.Name} must inherit from GameEntity", 
                nameof(entityType));
        }
        
        EntityType = entityType;
    }
}