using System.Reflection;
using TurnForge.Engine.Core; 
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Extensions;

public static class GameEngineExtensions
{
    private static Dictionary<string, object>? ToDictionary(IActionParameters? parameters)
    {
        if (parameters == null) return null;
        
        var dict = new Dictionary<string, object>();
        var properties = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        foreach (var prop in properties)
        {
            // We use the property name as key.
            // This relies on the convention that parameter object properties match the expected context keys.
            var value = prop.GetValue(parameters);
            if (value != null) // Avoid nulls to not clutter dictionary, unless keys explicitly require null?
            {
               dict[prop.Name] = value!;
            }
        }
        return dict;
    }

    /// <summary>
    /// Extension for IGameEngine interface
    /// </summary>
    public static ActionTransaction ExecuteAction(this IGameEngine engine, ActionId actionId, IActionParameters parameters)
    {
        return engine.ExecuteAction(actionId, ToDictionary(parameters));
    }
    
    /// <summary>
    /// Extension for the TurnForge Facade class
    /// </summary>
    public static ActionTransaction ExecuteAction(this TurnForge.Engine.Core.TurnForge facade, ActionId actionId, IActionParameters parameters)
    {
        // Facade ExecuteAction takes care of injecting dependencies like Catalog
        return facade.ExecuteAction(actionId, ToDictionary(parameters));
    }
}
