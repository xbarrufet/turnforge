using System.Reflection;
using BarelyAlive.Rules.Core.Behaviours.Attributes;
using NUnit.Framework;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;

namespace BarelyAlive.Rules.Tests.Architecture;

[TestFixture]
public class BehaviourConventionTests
{
    private Assembly _rulesAssembly = null!;

    [SetUp]
    public void Setup()
    {
        // Get the assembly containing rules/behaviours
        _rulesAssembly = typeof(BarelyAlive.Rules.Game.BarelyAliveGame).Assembly;
    }

    [Test]
    public void AllActorBehaviours_ShouldInheritFrom_ActorBehaviour_BaseClass()
    {
        // Find all types with [ActorBehaviour] attribute
        var actorBehaviours = _rulesAssembly.GetTypes()
            .Where(t => t.GetCustomAttribute<ActorBehaviourAttribute>() != null)
            .ToList();

        Assert.That(actorBehaviours, Is.Not.Empty, "No [ActorBehaviour] types found to test");

        foreach (var type in actorBehaviours)
        {
            Assert.That(typeof(ActorBehaviour).IsAssignableFrom(type),
                $"Type '{type.Name}' has [ActorBehaviour] but does not inherit from ActorBehaviour base class.");
        }
    }

    [Test]
    public void AllZoneBehaviours_ShouldInheritFrom_ZoneBehaviour_BaseClass()
    {
        // Find all types with [ZoneBehaviour] attribute
        var zoneBehaviours = _rulesAssembly.GetTypes()
            .Where(t => t.GetCustomAttribute<ZoneBehaviourAttribute>() != null)
            .ToList();

        Assert.That(zoneBehaviours, Is.Not.Empty, "No [ZoneBehaviour] types found to test");

        foreach (var type in zoneBehaviours)
        {
            Assert.That(typeof(ZoneBehaviour).IsAssignableFrom(type),
                $"Type '{type.Name}' has [ZoneBehaviour] but does not inherit from ZoneBehaviour base class.");
        }
    }
}
