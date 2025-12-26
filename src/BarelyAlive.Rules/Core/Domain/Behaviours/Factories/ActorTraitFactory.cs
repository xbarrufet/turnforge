using System.Reflection;
using System.Text.Json;
using BarelyAlive.Rules.Core.Domain.Behaviours.ActorBehaviours;
using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Entities.Actors.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.Factories;

public static class ActorTraitFactory
{
    private static readonly Dictionary<string, Type> _traitTypes = LoadTraits();

    private static Dictionary<string, Type> LoadTraits()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                typeof(IActorTrait).IsAssignableFrom(t) &&
                !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attr = t.GetCustomAttribute<ActorTraitAttribute>()
            })
            .Where(x => x.Attr != null)
            .ToDictionary(
                x => x.Attr!.Type,
                x => x.Type
            );
    }

    public static bool IsRegistered(string type) => _traitTypes.ContainsKey(type);

    public static IActorTrait Create(string type, Dictionary<string, JsonElement> data)
    {
        if (!_traitTypes.TryGetValue(type, out var traitType))
            throw new NotSupportedException($"Trait '{type}' is not registered.");

        var ctor = traitType.GetConstructors().Single();
        var args = ctor.GetParameters()
            .Select(p => ResolveParameter(p, data))
            .ToArray();

        return (IActorTrait)Activator.CreateInstance(traitType, args)!;
    }

    private static object ResolveParameter(
        ParameterInfo parameter,
        Dictionary<string, JsonElement> data)
    {
        var attr = parameter.GetCustomAttribute<TraitParamAttribute>();
        if (attr == null)
            throw new InvalidOperationException(
                $"Parameter '{parameter.Name}' missing TraitParamAttribute.");

        if (!data.TryGetValue(attr.JsonName, out var jsonProp))
            throw new InvalidOperationException(
                $"Missing json property '{attr.JsonName}'.");

        return jsonProp.Deserialize(parameter.ParameterType)!;
    }
}
