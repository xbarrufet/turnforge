using TurnForge.Rules.BarelyAlive.Loading;
using NUnit.Framework;

namespace TurnForge.Rules.BarelyAlive.Test.Loading;

[TestFixture]
public class MissionLoaderTests
{
    private string _testMissionPath;

    [SetUp]
    public void Setup()
    {
        // Path relative to execution directory, assuming assets are copied
        _testMissionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Assets", "mission01.json");
    }

    [Test]
    public void LoadFromFile_Mission01_ParsesCorrectly()
    {
        // Act
        var mission = MissionLoader.LoadFromFile(_testMissionPath);

        // Assert
        Assert.That(mission, Is.Not.Null);
        Assert.That(mission.MissionName, Is.EqualTo("mission01"));
        Assert.That(mission.Scale, Is.EqualTo("250x250"));
        Assert.That(mission.Areas, Is.Not.Empty);
        Assert.That(mission.Actors, Is.Not.Empty);

        var firstActor = mission.Actors[0];
        Assert.That(firstActor.ActorKind, Is.EqualTo("Prop"));
        Assert.That(firstActor.CustomType, Is.EqualTo("PartySpawnPoint"));
        
        var zombieActor = mission.Actors.FirstOrDefault(a => a.CustomType == "ZombieSpawnPoint");
        Assert.That(zombieActor, Is.Not.Null);
        Assert.That(zombieActor!.Behaviours, Is.Not.Empty);
        Assert.That(zombieActor.Behaviours[0].Type, Is.EqualTo("SpawnOrder"));
        Assert.That(zombieActor.Behaviours[0].Attributes[0].Name, Is.EqualTo("order"));
    }
}
