using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Descriptors;

namespace TurnForge.Engine.Core.Registries;

/// <summary>
/// Registry for Definition↔Entity and Entity→Descriptor type mappings.
/// Built at startup by scanning assemblies for [DefinitionType] and [DescriptorType] attributes.
/// </summary>
/// <remarks>
/// This registry eliminates the need for redundant attributes on Definition classes.
/// Instead, all type relationships are declared on Entity classes:
/// 
/// <code>
/// [DefinitionType(typeof(SurvivorDefinition))]  // Entity → Definition
/// [DescriptorType(typeof(SurvivorDescriptor))]  // Entity → Descriptor
/// public class Survivor : Agent { }
/// </code>
/// 
/// The registry builds inverse mappings at startup:
/// - SurvivorDefinition → Survivor (for factory)
/// - Survivor → SurvivorDescriptor (for spawn strategy)
/// </remarks>
public static class EntityTypeRegistry
{
    private static readonly Dictionary<Type, Type> _definitionToEntity = new();
    private static readonly Dictionary<Type, Type> _entityToDefinition = new();
    private static bool _initialized = false;
    private static readonly object _lock = new();

    /// <summary>
    /// Initializes the registry by scanning all loaded assemblies.
    /// Called by GameEngineFactory.Build() at engine startup.
    /// </summary>
    /// <remarks>
    /// RATIONALE: Centralized Initialization Pattern
    /// 
    /// This method is called once during engine initialization in GameEngineFactory.Build().
    /// This provides several architectural benefits:
    /// 
    /// 1. **Explicit Flow**: Clear initialization order - registry setup happens before
    ///    any factories or builders need it.
    /// 
    /// 2. **Performance**: Single assembly scan at startup instead of lazy initialization
    ///    with lock overhead on every factory/builder instantiation.
    /// 
    /// 3. **Separation of Concerns**: Domain components (factories, builders) don't need
    ///    to manage global initialization - infrastructure layer handles it.
    /// 
    /// 4. **Testability**: Tests can rely on GameEngineFactory setup, or call Initialize()
    ///    explicitly in test setup for isolated unit tests.
    /// 
    /// Thread-safe and idempotent - multiple calls will only initialize once.
    /// </remarks>
    public static void Initialize()
    {
        lock (_lock)
        {
            if (_initialized) return;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    RegisterAssembly(assembly);
                }
                catch (ReflectionTypeLoadException)
                {
                    // Skip assemblies that can't be loaded (e.g., system assemblies)
                    continue;
                }
            }

            _initialized = true;
        }
    }

    /// <summary>
    /// Registers all entities with [DefinitionType] attribute from an assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for entity types.</param>
    public static void RegisterAssembly(Assembly assembly)
    {
        var entityTypes = assembly.GetTypes()
            .Where(t => typeof(GameEntity).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var entityType in entityTypes)
        {
            var attr = entityType.GetCustomAttribute<DefinitionTypeAttribute>();
            if (attr != null)
            {
                Register(attr.DefinitionType, entityType);
            }
        }
    }

    /// <summary>
    /// Manually registers a Definition↔Entity mapping.
    /// </summary>
    /// <param name="definitionType">The Definition type.</param>
    /// <param name="entityType">The Entity type.</param>
    /// <exception cref="ArgumentException">Thrown if types don't meet inheritance requirements.</exception>
    public static void Register(Type definitionType, Type entityType)
    {
        if (!typeof(BaseGameEntityDefinition).IsAssignableFrom(definitionType))
        {
            throw new ArgumentException(
                $"Type {definitionType.Name} must inherit from BaseGameEntityDefinition",
                nameof(definitionType));
        }

        if (!typeof(GameEntity).IsAssignableFrom(entityType))
        {
            throw new ArgumentException(
                $"Type {entityType.Name} must inherit from GameEntity",
                nameof(entityType));
        }

        _definitionToEntity[definitionType] = entityType;
        _entityToDefinition[entityType] = definitionType;
    }

    /// <summary>
    /// Gets the Entity type associated with a Definition type.
    /// </summary>
    /// <param name="definitionType">The Definition type to lookup.</param>
    /// <returns>The Entity type, or null if no mapping exists (will use default entity type).</returns>
    public static Type? GetEntityType(Type definitionType)
    {
        EnsureInitialized();
        return _definitionToEntity.TryGetValue(definitionType, out var entityType) 
            ? entityType 
            : null;
    }

    /// <summary>
    /// Gets the Definition type associated with an Entity type.
    /// </summary>
    /// <param name="entityType">The Entity type to lookup.</param>
    /// <returns>The Definition type, or null if no mapping exists.</returns>
    public static Type? GetDefinitionType(Type entityType)
    {
        EnsureInitialized();
        return _entityToDefinition.TryGetValue(entityType, out var definitionType) 
            ? definitionType 
            : null;
    }

    /// <summary>
    /// Gets the Descriptor type from an Entity (if specified via [DescriptorType]).
    /// </summary>
    /// <param name="entityType">The Entity type to check for descriptor attribute.</param>
    /// <returns>The Descriptor type, or null if no [DescriptorType] attribute exists.</returns>
    public static Type? GetDescriptorType(Type entityType)
    {
        var attr = entityType.GetCustomAttribute<DescriptorTypeAttribute>();
        return attr?.DescriptorType;
    }

    /// <summary>
    /// Gets complete type chain for a Definition: Definition → Entity → Descriptor.
    /// </summary>
    /// <param name="definitionType">The Definition type.</param>
    /// <returns>Tuple of (EntityType, DescriptorType) or nulls if not found.</returns>
    public static (Type? EntityType, Type? DescriptorType) GetTypeChain(Type definitionType)
    {
        var entityType = GetEntityType(definitionType);
        var descriptorType = entityType != null ? GetDescriptorType(entityType) : null;
        return (entityType, descriptorType);
    }

    /// <summary>
    /// Checks if a Definition type has a custom Entity registered.
    /// </summary>
    public static bool HasCustomEntity(Type definitionType)
    {
        EnsureInitialized();
        return _definitionToEntity.ContainsKey(definitionType);
    }

    /// <summary>
    /// Gets all registered Definition→Entity mappings (for debugging/diagnostics).
    /// </summary>
    public static IReadOnlyDictionary<Type, Type> GetAllMappings()
    {
        EnsureInitialized();
        return _definitionToEntity;
    }

    private static void EnsureInitialized()
    {
        if (!_initialized)
        {
            Initialize();
        }
    }

    /// <summary>
    /// Clears the registry (for testing only).
    /// </summary>
    internal static void Clear()
    {
        lock (_lock)
        {
            _definitionToEntity.Clear();
            _entityToDefinition.Clear();
            _initialized = false;
        }
    }
}
