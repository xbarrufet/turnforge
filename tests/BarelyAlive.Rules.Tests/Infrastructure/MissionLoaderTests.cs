using NUnit.Framework;
using BarelyAlive.Rules.Tests.Infrastructure;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities.Descriptors;

namespace BarelyAlive.Rules.Tests.Infrastructure;

[TestFixture]
public class MissionLoaderTests
{
    [Test]
    public void LoadMission01_ShouldParseCorrectly()
    {
        var (spatialDesc, zones, props, agents) = BarelyAlive.Rules.Adapter.Loaders.MissionLoader.ParseMissionString(TestHelpers.Mission01Json);
        
        Assert.That(spatialDesc, Is.Not.Null);
        var discreteSpatial = spatialDesc as DiscreteSpatialDescriptor;
        Assert.That(discreteSpatial, Is.Not.Null);
        
        // Original JSON had 9 nodes
        Assert.That(discreteSpatial.Nodes.Count, Is.EqualTo(9));
        
        // Connections: 16
        Assert.That(discreteSpatial.Connections.Count, Is.EqualTo(16));
        
        // Zones: 9
        Assert.That(zones.Count, Is.EqualTo(9));
        
        // Props verification:
        // 9 Areas (one per zone)
        // Explicit props in JSON: 2 Spawns, 1 Door (explicit).
        // Auto-generated doors: ?
        
        var areaProps = props.Where(p => p.DefinitionId == "Area").ToList();
        Assert.That(areaProps.Count, Is.EqualTo(9), "Should have 1 Area prop per Zone");
        
        var explicitDoorInJson = "55f54395-6e94-4d1a-9694-824050f4a867";
        var doorProps = props.Where(p => p.DefinitionId == "Door").ToList();
        Assert.That(doorProps, Is.Not.Empty, "Should have at least the explicit door");
        Assert.That(doorProps.Any(d => d.Position.HasValue && d.Position.Value.IsConnection && d.Position.Value.ConnectionId.ToString() == explicitDoorInJson), "Explicit door missing");
        
        // Check for auto-generated doors?
        // User requested NO auto-generation. Doors are hardcoded in JSON.
        // Current JSON has 1 explicit door, 2 spawns, 9 zones (areas).
        // Total = 12.
        var spawnProps = props.Where(p => p.DefinitionId == "Spawn.Zombie" || p.DefinitionId == "Spawn.Player").ToList();
        Assert.That(spawnProps.Count, Is.EqualTo(2), "Should have 2 spawns");
        Assert.That(spawnProps.All(s => s.Position.HasValue && !s.Position.Value.IsConnection), "Spawns should be on Tiles (not connections)");
        
        Assert.That(props.Count, Is.EqualTo(12), "Should match exact number of props (Areas + Explicit in JSON)");
    }
}
