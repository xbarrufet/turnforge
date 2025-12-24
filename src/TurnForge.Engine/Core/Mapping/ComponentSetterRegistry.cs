using System.Reflection;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;

namespace TurnForge.Engine.Core.Mapping;

/// <summary>
/// Registry responsible for discovering and caching settable properties on Components.
/// Scans assemblies for IComponent implementations and builds a fast lookup map.
/// </summary>
public static class ComponentSetterRegistry
{
    // Cache: ComponentType -> Dictionary<PropertyName, PropertyInfo>
    private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _componentPropertyCache = new();
    private static readonly HashSet<Assembly> _scannedAssemblies = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Forces initialization of the registry by scanning the provided assemblies.
    /// If no assemblies are provided, it scans the assembly containing IGameEntityComponent.
    /// </summary>
    public static void Initialize(params Assembly[] assembliesToScan)
    {
        var targetAssemblies = assembliesToScan.Length > 0 
            ? assembliesToScan 
            : new[] { typeof(IGameEntityComponent).Assembly };

        lock (_lock)
        {
            foreach (var assembly in targetAssemblies)
            {
                if (_scannedAssemblies.Contains(assembly)) continue;
                
                ScanAssembly(assembly);
                _scannedAssemblies.Add(assembly);
            }
        }
    }

    /// <summary>
    /// Retrieves the cached property map for a specific component type.
    /// Returns an empty dictionary if the component hasn't been scanned or has no mappable properties.
    /// </summary>
    public static IReadOnlyDictionary<string, PropertyInfo> GetSettableProperties(Type componentType)
    {
        // Ensure the assembly of this component is scanned
        Initialize(componentType.Assembly);

        if (_componentPropertyCache.TryGetValue(componentType, out var map))
        {
            return map;
        }

        return new Dictionary<string, PropertyInfo>();
    }
    
    /// <summary>
    /// Scans a single assembly for IGameEntityComponent implementations.
    /// </summary>
    private static void ScanAssembly(Assembly assembly)
    {
        var componentTypes = assembly.GetTypes()
            .Where(t => typeof(IGameEntityComponent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in componentTypes)
        {
            RegisterComponent(type);
        }
    }

    /// <summary>
    /// Analyzes a component type and caches its eligible properties.
    /// Eligible = Public, Has Setter, Not marked with [DoNotMap].
    /// </summary>
    private static void RegisterComponent(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => 
                p.CanWrite && 
                p.GetSetMethod(false) != null && // Must have public setter
                p.GetCustomAttribute<DoNotMapAttribute>() == null
            );

        var propertyMap = properties.ToDictionary(p => p.Name, p => p);

        if (propertyMap.Count > 0)
        {
            _componentPropertyCache[type] = propertyMap;
        }
    }
}
