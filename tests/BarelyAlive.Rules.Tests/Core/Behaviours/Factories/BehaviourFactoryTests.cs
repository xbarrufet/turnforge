using System.Reflection;
using System.Text.Json;
using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using BarelyAlive.Rules.Core.Domain.Behaviours.Factories;
using TurnForge.Engine.Entities.Actors.Interfaces;

namespace BarelyAlive.Rules.Tests.Core.Behaviours.Factories;

[TestFixture]
public class BehaviourFactoryTests
{
    private Assembly _rulesAssembly = null!;

    [SetUp]
    public void Setup()
    {
        _rulesAssembly = typeof(BarelyAlive.Rules.Game.BarelyAliveGame).Assembly;
    }

    [Test]
    public void AllRegistered_ActorBehaviours_CanBeInstantiated()
    {
        var actorBehaviours = _rulesAssembly.GetTypes()
            .Where(t => t.GetCustomAttribute<ActorBehaviourAttribute>() != null);

        foreach (var type in actorBehaviours)
        {
            var attr = type.GetCustomAttribute<ActorBehaviourAttribute>()!;
            var registeredType = attr.Type;

            // Generate dummy parameters
            var data = GenerateDummyParameters(type);

            // Act
            Assert.DoesNotThrow(() =>
            {
                var instance = ActorBehaviourFactory.Create(registeredType, data);
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance, Is.InstanceOf(type));
            }, $"Failed to instantiate ActorBehaviour '{registeredType}'");
        }
    }

    [Test]
    public void AllRegistered_ZoneBehaviours_CanBeInstantiated()
    {
        var zoneBehaviours = _rulesAssembly.GetTypes()
            .Where(t => t.GetCustomAttribute<ZoneBehaviourAttribute>() != null);

        foreach (var type in zoneBehaviours)
        {
            var attr = type.GetCustomAttribute<ZoneBehaviourAttribute>()!;
            var registeredType = attr.Type;

            // Generate dummy parameters
            var data = GenerateDummyParameters(type);

            // Act
            Assert.DoesNotThrow(() =>
            {
                var instance = ZoneBehaviourFactory.Create(registeredType, data);
                Assert.That(instance, Is.Not.Null);
                Assert.That(instance, Is.InstanceOf(type));
            }, $"Failed to instantiate ZoneBehaviour '{registeredType}'");
        }
    }

    private Dictionary<string, JsonElement> GenerateDummyParameters(Type type)
    {
        var dict = new Dictionary<string, JsonElement>();
        var ctor = type.GetConstructors().Single();

        foreach (var param in ctor.GetParameters())
        {
            var paramAttr = param.GetCustomAttribute<BehaviourParamAttribute>();
            if (paramAttr == null) continue; // Should fail elsewhere if missing, but defensive here

            object? dummyValue = null;
            if (param.ParameterType == typeof(int)) dummyValue = 1;
            else if (param.ParameterType == typeof(string)) dummyValue = "dummy";
            else if (param.ParameterType == typeof(bool)) dummyValue = false;

            // We need to serialize then deserialize to get a JsonElement
            var json = JsonSerializer.Serialize(dummyValue);
            var element = JsonSerializer.Deserialize<JsonElement>(json);

            dict[paramAttr.JsonName] = element;
        }

        return dict;
    }
}
