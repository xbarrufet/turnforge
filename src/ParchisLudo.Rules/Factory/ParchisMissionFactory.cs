using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Entities;
using TurnForge.Engine.Entities.Definitions; // for MissionDefinition

using Parchis.Rules.Board; // For ParchisBoard enum and constants
using TurnForge.Engine.Entities.Board.Interfaces; // For IBoardPosition interface

namespace Parchis.Rules.Factory;

public static class ParchisMissionFactory
{
    public static MissionDefinition CreateStandardMission()
    {
        throw new System.NotImplementedException("MissionData needs PlayerIds. Use CreateMissionForPlayers(...)");
    }

    public static MissionDefinition CreateMissionForPlayers(Dictionary<PlayerId, ParchisBoard.PlayerColor> playerColors)
    {
        var mission = new MissionDefinition("parchis_standard")
        {
             Name = "Standard Parchis Match"
        };
        
        var spawnZones = new Dictionary<PlayerId, IBoardPosition>();

        foreach (var kvp in playerColors)
        {
            var playerId = kvp.Key;
            var color = kvp.Value;
            
            // "spawn_red", "spawn_blue", etc.
            var tileIdStr = $"spawn_{color.ToString().ToLower()}";
            spawnZones.Add(playerId, new TilePosition(new TileId(tileIdStr)));
        }
        mission.PlayerSpawnZones = spawnZones;

        // 2. NamedLocations
        var locations = new Dictionary<string, IBoardPosition>();
        locations.Add("Center", new TilePosition(new TileId("center")));
        mission.NamedLocations = locations;

        // 3. Objective: Reach Center (Use ExtractionObjective as proxy)
        mission.Objective = new ExtractionObjective("Center");

        // 4. Connection descriptors for directional movement
        mission.ConnectionRequests = CreateConnectionDescriptors().ToList();

        return mission;
    }
    
    /// <summary>
    /// Creates connection descriptors for the Parchis board.
    /// - Forward connections for main track
    /// - Finish entry connections restricted by color
    /// </summary>
    public static IEnumerable<ConnectionDescriptor> CreateConnectionDescriptors()
    {
        // Main track: forward connections (1 → 2 → 3 → ... → 68 → 1)
        for (int i = 1; i < ParchisBoard.MainCircuitSize; i++)
        {
            yield return ConnectionDescriptor.Forward($"track_{i}", $"track_{i + 1}");
        }
        // Close the circuit
        yield return ConnectionDescriptor.Forward($"track_{ParchisBoard.MainCircuitSize}", "track_1");
        
        // Spawn to track entry (forward)
        yield return ConnectionDescriptor.Forward("spawn_red", ParchisBoard.RedEntry);
        yield return ConnectionDescriptor.Forward("spawn_blue", ParchisBoard.BlueEntry);
        yield return ConnectionDescriptor.Forward("spawn_green", ParchisBoard.GreenEntry);
        yield return ConnectionDescriptor.Forward("spawn_yellow", ParchisBoard.YellowEntry);
        
        // Finish entry connections (color-restricted)
        yield return ConnectionDescriptor.FinishEntry($"track_{ParchisBoard.RedFinishEntry}", "red_finish_1", "red");
        yield return ConnectionDescriptor.FinishEntry($"track_{ParchisBoard.BlueFinishEntry}", "blue_finish_1", "blue");
        yield return ConnectionDescriptor.FinishEntry($"track_{ParchisBoard.GreenFinishEntry}", "green_finish_1", "green");
        yield return ConnectionDescriptor.FinishEntry($"track_{ParchisBoard.YellowFinishEntry}", "yellow_finish_1", "yellow");
        
        // Finish lane forward connections (per color)
        foreach (var color in new[] { "red", "blue", "green", "yellow" })
        {
            for (int i = 1; i < ParchisBoard.FinishLaneSize; i++)
            {
                yield return new ConnectionDescriptor(
                    new TileId($"{color}_finish_{i}"),
                    new TileId($"{color}_finish_{i + 1}"),
                    "forward",
                    color  // Only same color can use
                );
            }
            
            // Last finish tile to center
            yield return new ConnectionDescriptor(
                new TileId($"{color}_finish_{ParchisBoard.FinishLaneSize}"),
                new TileId("center"),
                "finish_complete",
                color
            );
        }
    }
}

