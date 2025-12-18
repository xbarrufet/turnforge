using System.Reflection;
using System.Text.Json;
using BarelyAlive.Rules.Core.Behaviours.ActorBehaviours;
using BarelyAlive.Rules.Core.Behaviours.Attributes;
using TurnForge.Engine.Entities.Actors.Interfaces;

namespace BarelyAlive.Rules.Core.Behaviours.Factories;

public static class ActorBehaviourFactory
{
    private static readonly Dictionary<string, Type> _behaviourTypes = LoadBehaviours();

    private static Dictionary<string, Type> LoadBehaviours()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                typeof(IActorBehaviour).IsAssignableFrom(t) &&
                !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attr = t.GetCustomAttribute<ActorBehaviourAttribute>()
            })
            .Where(x => x.Attr != null)
            .ToDictionary(
                x => x.Attr!.Type,
                x => x.Type
            );
    }

    public static IActorBehaviour Create(string type, JsonElement data)
    {
        if (!_behaviourTypes.TryGetValue(type, out var behaviourType))
            throw new NotSupportedException($"Behaviour '{type}' is not registered.");

        var ctor = behaviourType.GetConstructors().Single();
        var args = ctor.GetParameters()
            .Select(p => ResolveParameter(p, data))
            .ToArray();

        return (IActorBehaviour)Activator.CreateInstance(behaviourType, args)!;
    }

    private static object ResolveParameter(
        ParameterInfo parameter,
        JsonElement data)
    {
        var attr = parameter.GetCustomAttribute<BehaviourParamAttribute>();
        if (attr == null)
            throw new InvalidOperationException(
                $"Parameter '{parameter.Name}' missing BehaviourParamAttribute.");

        if (!data.TryGetProperty(attr.JsonName, out var jsonProp))
            throw new InvalidOperationException(
                $"Missing json property '{attr.JsonName}'.");

        return jsonProp.Deserialize(parameter.ParameterType)!;
    }
}
