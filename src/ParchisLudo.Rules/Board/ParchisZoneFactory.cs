using static ParchisLudo.Rules.Board.ParchisBoard;

namespace ParchisLudo.Rules.Board;

/// <summary>
/// Factory to create Parchis zones with traits.
/// Zones represent special board locations with gameplay significance.
/// </summary>
public static class ParchisZoneFactory
{
    /*
    /// <summary>
    /// Creates all Parchis zones including spawn zones, safe zones, and center.
    /// </summary>
    public static List<ZoneDeployment> CreateZones()
    {
        var zones = new List<ZoneDeployment>();

        // 1. Spawn Zones (4) - traits: Color, Spawn
        zones.Add(CreateSpawnZone(PlayerColor.Red, ParchisBoard.RedSpawn));
        zones.Add(CreateSpawnZone(PlayerColor.Blue, ParchisBoard.BlueSpawn));
        zones.Add(CreateSpawnZone(PlayerColor.Green, ParchisBoard.GreenSpawn));
        zones.Add(CreateSpawnZone(PlayerColor.Yellow, ParchisBoard.YellowSpawn));

        // 2. Entry Cells (4) - trait: Safe
        zones.Add(CreateSafeZone("red_entry", RedEntry));
        zones.Add(CreateSafeZone("blue_entry", BlueEntry));
        zones.Add(CreateSafeZone("green_entry", GreenEntry));
        zones.Add(CreateSafeZone("yellow_entry", YellowEntry));

        // 3. Safety Zones (8) - trait: Safe
        var safePositions = new[]
        {
            "track_12", "track_17", "track_29", "track_34",
            "track_46", "track_51", "track_63", "track_68"
        };
        foreach (var pos in safePositions)
        {
            zones.Add(CreateSafeZone($"safe_{pos}", pos));
        }

        // 4. Center Zone
        zones.Add(CreateCenterZone());

        return zones;
    }

    private static ZoneDeployment CreateSpawnZone(PlayerColor color, string tileId)
    {
        var descriptor = new ZoneDescriptor(
            SpawZoneDefinition.DefId,
            extraComponents: null,
            definitionTraitValues: new ITrait[]
            {
                new ColorTrait(color)  // Provide color value for this specific spawn zone
            }
        );
        return new ZoneDeployment(descriptor, new TilePosition(new TileId(tileId)));
    }

    private static ZoneDeployment CreateSafeZone(string id, string tileId)
    {
        var descriptor = new ZoneDescriptor(
           SafetyZoneDefinition.DefId,
            extraComponents: null
        );
        return new ZoneDeployment(descriptor, new TilePosition(new TileId(tileId)));
    }

    private static ZoneDeployment CreateCenterZone()
    {
        var descriptor = new ZoneDescriptor(
            CenterZoneDefinition.DefId,
            extraComponents: null
        );
        return new ZoneDeployment(descriptor, new TilePosition(new TileId(ParchisBoard.Center)));
    }*/
}
