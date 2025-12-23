using BarelyAlive.Rules.Adapter.Loaders;
using NUnit.Framework;
using TurnForge.Engine.Commands.Spawn.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Tests.Adapters.Loaders;

[TestFixture]
public class MissionLoaderTests
{
    [Test]
    public void ParseMissionString_ShouldParseCorrectly()
    {
        // Arrange
        var json = File.ReadAllText("Assets/all_mission01.json");

        // Act
        var result = MissionLoader.ParseMissionString(json);

        // Assert
        // Assert
        Assert.That(result.Item1, Is.InstanceOf<DiscreteSpatialDescriptor>());
        var spatial = result.Item1 as DiscreteSpatialDescriptor;
        Assert.That(spatial, Is.Not.Null);
        Assert.That(spatial!.Nodes.Count, Is.EqualTo(9));
        Assert.That(spatial.Connections.Count, Is.EqualTo(16));

        Assert.That(result.Item2.Count, Is.EqualTo(9)); // Zones
        Assert.That(result.Item2.Any(z => z.Id.Value == "d7de841d-64a5-48b3-9662-0fe757a8950e"), Is.True);

        Assert.That(result.Item3.Count, Is.EqualTo(5)); // Props (2 explicit + 3 generated from Zones)
        var spawnProp = result.Item3.FirstOrDefault(p => p.DefinitionId == "BarelyAlive.Spawn");
        Assert.That(spawnProp, Is.Not.Null);
        Assert.That(spawnProp!.Position, Is.Not.Null);
        Assert.That(spawnProp.Position!.Value.IsTile, Is.True, "Prop position should be discrete (TileId)");
        Assert.That(spawnProp.ExtraComponents!.Any(b => b.GetType().Name == "ZombieSpawnComponent"), Is.True);
    }
}
