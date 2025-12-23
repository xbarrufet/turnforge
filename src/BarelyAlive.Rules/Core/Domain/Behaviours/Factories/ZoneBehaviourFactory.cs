using System.Reflection;
using System.Text.Json;
using BarelyAlive.Rules.Core.Domain.Behaviours.ZoneBehaviours;
using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.Factories;

public static class ZoneBehaviourFactory
{
    private static readonly Dictionary<string, Type> _behaviourTypes = LoadBehaviours();

    private static Dictionary<string, Type> LoadBehaviours()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                typeof(IZoneBehaviour).IsAssignableFrom(t) &&
                !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attr = t.GetCustomAttribute<ZoneBehaviourAttribute>()
            })
            .Where(x => x.Attr != null)
            .ToDictionary(
                x => x.Attr!.Type,
                x => x.Type
            );
    }

    public static bool IsRegistered(string type) => _behaviourTypes.ContainsKey(type);

    public static IZoneBehaviour Create(string type, Dictionary<string, JsonElement> data)
    {
        if (!_behaviourTypes.TryGetValue(type, out var behaviourType))
            throw new NotSupportedException($"ZoneBehaviour '{type}' is not registered.");

        var ctor = behaviourType.GetConstructors().Single();
        var args = ctor.GetParameters()
            .Select(p => ResolveParameter(p, data))
            .ToArray();

        return (IZoneBehaviour)Activator.CreateInstance(behaviourType, args)!;
    }

    private static object ResolveParameter(
        ParameterInfo parameter,
        Dictionary<string, JsonElement> data)
    {
        var attr = parameter.GetCustomAttribute<BehaviourParamAttribute>();
        if (attr == null)
            throw new InvalidOperationException(
                $"Parameter '{parameter.Name}' missing BehaviourParamAttribute.");

        if (!data.TryGetValue(attr.JsonName, out var jsonProp))
            throw new InvalidOperationException(
                $"Missing json property '{attr.JsonName}'.");

        return jsonProp.Deserialize(parameter.ParameterType)!;
    }
}
