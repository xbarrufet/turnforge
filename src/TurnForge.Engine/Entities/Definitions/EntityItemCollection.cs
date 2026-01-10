using TurnForge.Engine.Entities.TraitsComponents.Interfaces;

namespace TurnForge.Engine.Entities.Definitions;

/// <summary>
/// Generic collection manager for entity items (traits or components).
/// Handles storage, required tracking, and validation.
/// </summary>
/// <typeparam name="TItem">Base type for items (ITrait or IGameEntityComponent)</typeparam>
internal class EntityItemCollection<TItem>
{
    private readonly Dictionary<Type, List<TItem>> _itemsByType = new();
    private readonly HashSet<Type> _requiredTypes = new();

    /// <summary>
    /// Adds an item to the collection, optionally marking it as required.
    /// </summary>
    public void Add(Type type, TItem item, bool isRequired = false)
    {
        if (isRequired)
            _requiredTypes.Add(type);

        if (!_itemsByType.TryGetValue(type, out var list))
        {
            list = new List<TItem>();
            _itemsByType[type] = list;
        }
        list.Add(item);
    }

    /// <summary>
    /// Adds an item with stacking validation.
    /// If stackAllowed is false and items of this type exist, replaces them.
    /// </summary>
    public void AddWithStackValidation(Type type, TItem item, bool stackAllowed, bool isRequired = false)
    {
        if (isRequired)
            _requiredTypes.Add(type);

        if (!stackAllowed && _itemsByType.ContainsKey(type))
        {
            // Replace existing items
            _itemsByType[type] = new List<TItem> { item };
        }
        else
        {
            // Add or stack
            if (!_itemsByType.TryGetValue(type, out var list))
            {
                list = new List<TItem>();
                _itemsByType[type] = list;
            }
            list.Add(item);
        }
    }

    /// <summary>
    /// Removes all items of the specified type.
    /// Throws if the type is marked as required.
    /// </summary>
    public void Remove(Type type)
    {
        if (_requiredTypes.Contains(type))
            throw new InvalidOperationException($"Cannot remove required item of type {type.Name}");
        _itemsByType.Remove(type);
    }

    /// <summary>
    /// Gets all items of the specified type (required and non-required).
    /// Supports inheritance: if T is ITrait, returns all traits (VitalityTrait, MovableTrait, etc.)
    /// </summary>
    public IEnumerable<T> GetAll<T>() where T : TItem
    {
        var requestedType = typeof(T);

        // Find all types in the dictionary that are assignable to T
        var matchingTypes = _itemsByType.Keys
            .Where(storedType => requestedType.IsAssignableFrom(storedType))
            .ToList();

        if (!matchingTypes.Any())
            return Enumerable.Empty<T>();

        // Get all items of matching types
        return matchingTypes
            .SelectMany(type => _itemsByType[type])
            .Cast<T>();
    }

    /// <summary>
    /// Gets only items of the specified type that are marked as required.
    /// Supports inheritance: if T is ITrait, returns all required traits (VitalityTrait, MovableTrait, etc.)
    /// </summary>
    public IEnumerable<T> GetAllRequired<T>() where T : TItem
    {
        var requestedType = typeof(T);

        // Find all required types that are assignable to T
        var matchingRequiredTypes = _requiredTypes
            .Where(reqType => requestedType.IsAssignableFrom(reqType))
            .ToList();

        if (!matchingRequiredTypes.Any())
            return Enumerable.Empty<T>();

        // Get all items of matching required types
        return matchingRequiredTypes
            .SelectMany(reqType => _itemsByType.TryGetValue(reqType, out var list) ? list : Enumerable.Empty<TItem>())
            .Cast<T>();
    }

    /// <summary>
    /// Gets the first item of the specified type, or null.
    /// </summary>
    public T? GetFirst<T>() where T : TItem
        => GetAll<T>().FirstOrDefault();

    /// <summary>
    /// Checks if any items of the specified type exist.
    /// </summary>
    public bool Has<T>() where T : TItem
        => _itemsByType.ContainsKey(typeof(T));

    /// <summary>
    /// Gets the count of items of the specified type.
    /// </summary>
    public int Count<T>() where T : TItem
        => _itemsByType.TryGetValue(typeof(T), out var list) ? list.Count : 0;

    /// <summary>
    /// Checks if a type is marked as required.
    /// </summary>
    public bool IsRequired(Type type)
        => _requiredTypes.Contains(type);

    /// <summary>
    /// Gets all types that are marked as required.
    /// </summary>
    public IEnumerable<Type> GetAllRequiredTypes()
        => _requiredTypes;

    /// <summary>
    /// Gets all items across all types.
    /// </summary>
    public IEnumerable<TItem> GetAllItems()
        => _itemsByType.Values.SelectMany(x => x);
}
