using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action;

/// <summary>
/// In-memory workflow registry.
/// Stores workflow factories that create new instances on each execution.
/// </summary>
public sealed class ActionRegistry : IActionRegistry
{
    private readonly Dictionary<string, Func<IAction>> _factories = new();
    
    public void Register(ActionId id, Func<IAction> factory)
    {
        _factories[id.Value] = factory;
    }
    
    public IAction? GetAction(ActionId id)
    {
        if (_factories.TryGetValue(id.Value, out var factory))
        {
            return factory();
        }
        return null;
    }
    
    public bool IsRegistered(ActionId id)
    {
        return _factories.ContainsKey(id.Value);
    }
}
