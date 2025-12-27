using System.Reflection;
using System.Text.Json;
using BarelyAlive.Rules.Core.Domain.Behaviours.ZoneBehaviours;
using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Definitions.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.Factories;

public static class ZoneTraitFactory
{
    private static readonly Dictionary<string, Type> _traitTypes = LoadTraits();

    private static Dictionary<string, Type> LoadTraits()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                typeof(IZoneTrait).IsAssignableFrom(t) &&
                !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attr = t.GetCustomAttribute<ZoneTraitAttribute>()
            })
            .Where(x => x.Attr != null)
            .ToDictionary(
                x => x.Attr!.Type,
                x => x.Type
            );
    }

    public static bool IsRegistered(string type) => _traitTypes.ContainsKey(type);

    public static IZoneTrait Create(string type, Dictionary<string, JsonElement> data)
    {
        if (!_traitTypes.TryGetValue(type, out var traitType))
            throw new NotSupportedException($"ZoneTrait '{type}' is not registered.");

        var ctor = traitType.GetConstructors().Single();
        var args = ctor.GetParameters()
            .Select(p => ResolveParameter(p, data))
            .ToArray();

        return (IZoneTrait)Activator.CreateInstance(traitType, args)!;
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
