using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Spawn;

/// <summary>
/// Fluent builder for creating SpawnRequests with improved developer experience.
/// Provides IntelliSense-driven API while producing standard SpawnRequest objects.
/// </summary>
/// <remarks>
/// This builder is purely a convenience layer - it produces the same SpawnRequest
/// that the engine already expects. No internal pipeline changes needed.
/// 
/// Usage:
/// <code>
/// var boss = SpawnRequestBuilder
///     .For("Enemies.DragonBoss")
///     .At(bossSpawnPoint)
///     .WithProperty("Health", 1000)
///     .WithProperty("PhaseCount", 3)
///     .WithComponent(new BossAIComponent())
///     .Build();
/// </code>
/// </remarks>
public sealed class SpawnRequestBuilder
{
    private string _definitionId = string.Empty;
    private int _count = 1;
    private Position _position = Position.Empty;
    private Dictionary<string, object> _overrides = new();
    private List<IGameEntityComponent> _components = new();

    /// <summary>
    /// Private constructor - use For() to create instances.
    /// </summary>
    private SpawnRequestBuilder() { }

    /// <summary>
    /// Creates a new builder for the specified definition.
    /// </summary>
    /// <param name="definitionId">Entity definition ID (e.g., "Survivors.Mike", "Enemies.Zombie")</param>
    /// <returns>Builder instance for method chaining</returns>
    public static SpawnRequestBuilder For(string definitionId)
    {
        if (string.IsNullOrWhiteSpace(definitionId))
        {
            throw new ArgumentException("Definition ID cannot be null or empty", nameof(definitionId));
        }

        return new SpawnRequestBuilder { _definitionId = definitionId };
    }

    /// <summary>
    /// Sets the spawn position.
    /// </summary>
    /// <param name="position">World position where entity should spawn</param>
    /// <returns>Builder instance for method chaining</returns>
    public SpawnRequestBuilder At(Position position)
    {
        _position = position;
        return this;
    }

    /// <summary>
    /// Sets the number of entities to spawn (batch spawn).
    /// </summary>
    /// <param name="count">Number of entities to spawn (must be > 0)</param>
    /// <returns>Builder instance for method chaining</returns>
    public SpawnRequestBuilder WithCount(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than 0", nameof(count));
        }

        _count = count;
        return this;
    }

    /// <summary>
    /// Adds a property override that will be applied to the descriptor.
    /// </summary>
    /// <param name="key">Property name (must match descriptor property name)</param>
    /// <param name="value">Property value</param>
    /// <returns>Builder instance for method chaining</returns>
    /// <remarks>
    /// The property will be mapped to the descriptor via reflection in DescriptorBuilder.
    /// If the descriptor doesn't have a matching property, it will be silently ignored.
    /// </remarks>
    public SpawnRequestBuilder WithProperty(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Property key cannot be null or empty", nameof(key));
        }

        _overrides[key] = value;
        return this;
    }

    /// <summary>
    /// Type-safe property override with generic constraint.
    /// </summary>
    /// <typeparam name="T">Type of the property value</typeparam>
    /// <param name="key">Property name</param>
    /// <param name="value">Property value</param>
    /// <returns>Builder instance for method chaining</returns>
    public SpawnRequestBuilder WithProperty<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Property key cannot be null or empty", nameof(key));
        }

        _overrides[key] = value!;
        return this;
    }

    /// <summary>
    /// Adds an extra component to be attached to the spawned entity.
    /// </summary>
    /// <typeparam name="TComponent">Type of component (must implement IGameEntityComponent)</typeparam>
    /// <param name="component">Component instance</param>
    /// <returns>Builder instance for method chaining</returns>
    /// <remarks>
    /// Extra components are added in addition to components created from the definition.
    /// They bypass the normal descriptor-to-component mapping.
    /// </remarks>
    public SpawnRequestBuilder WithComponent<TComponent>(TComponent component)
        where TComponent : IGameEntityComponent
    {
        if (component == null)
        {
            throw new ArgumentNullException(nameof(component));
        }

        _components.Add(component);
        return this;
    }

    /// <summary>
    /// Adds multiple extra components at once.
    /// </summary>
    /// <param name="components">Array of components to add</param>
    /// <returns>Builder instance for method chaining</returns>
    public SpawnRequestBuilder WithComponents(params IGameEntityComponent[] components)
    {
        if (components == null || components.Length == 0)
        {
            return this;
        }

        foreach (var component in components)
        {
            if (component != null)
            {
                _components.Add(component);
            }
        }

        return this;
    }

    /// <summary>
    /// Builds the final SpawnRequest.
    /// </summary>
    /// <returns>Immutable SpawnRequest ready for command execution</returns>
    public SpawnRequest Build()
    {
        if (string.IsNullOrWhiteSpace(_definitionId))
        {
            throw new InvalidOperationException("Definition ID must be set before building");
        }

        return new SpawnRequest(
            _definitionId,
            _count,
            _position, // Non-nullable, defaults to Position.Empty
            _overrides.Count > 0 ? _overrides : null,
            _components.Count > 0 ? _components : null
        );
    }

    /// <summary>
    /// Implicit conversion to SpawnRequest for convenience.
    /// Allows using the builder directly where SpawnRequest is expected.
    /// </summary>
    /// <param name="builder">Builder instance</param>
    public static implicit operator SpawnRequest(SpawnRequestBuilder builder)
    {
        return builder.Build();
    }
}
