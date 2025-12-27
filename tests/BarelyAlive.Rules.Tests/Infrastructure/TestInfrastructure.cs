using NUnit.Framework;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using BarelyAlive.Rules.Core.Domain.Entities;

namespace BarelyAlive.Rules.Tests.Infrastructure;

[TestFixture]
public class TestInfrastructure
{
    private TestBootstrap _bootstrap;

    [SetUp]
    public void Setup()
    {
        _bootstrap = TestBootstrap.CreateNewGame();
    }

    [Test]
    public void CreateNewGame_ShouldRegisterAllDefinitions()
    {
        var catalog = _bootstrap.GameCatalog;

        // Verify Survivors
        Assert.That(catalog.GetDefinition<SurvivorDefinition>(TestHelpers.MikeId), Is.Not.Null);
        Assert.That(catalog.GetDefinition<SurvivorDefinition>(TestHelpers.DougId), Is.Not.Null);

        // Verify Zombies
        Assert.That(catalog.GetDefinition<BaseGameEntityDefinition>(TestHelpers.ZRunnerId), Is.Not.Null);
        Assert.That(catalog.GetDefinition<BaseGameEntityDefinition>(TestHelpers.ZFatId), Is.Not.Null);
        
        // Verify Board Elements
        Assert.That(catalog.GetDefinition<BaseGameEntityDefinition>(TestHelpers.AreaId), Is.Not.Null);
        Assert.That(catalog.GetDefinition<BaseGameEntityDefinition>(TestHelpers.DoorId), Is.Not.Null);
    }

    [Test]
    public void CreateNewGame_ShouldInitializeAPIs()
    {
        Assert.That(_bootstrap.BarelyAliveApis, Is.Not.Null);
    }

    [Test]
    public void VerifyMission01Loading()
    {
        var descriptor = TestHelpers.GetMission01BoardDescriptor();
        Assert.That(descriptor, Is.Not.Null);
        Assert.That(descriptor.Zones.Count, Is.EqualTo(9));
    }
}