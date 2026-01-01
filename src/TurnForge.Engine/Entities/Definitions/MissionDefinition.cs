using System.Collections.Generic;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities.Definitions; // For BaseGameEntityDefinition

namespace TurnForge.Engine.Entities.Definitions;

/// <summary>
/// Definition of a game mission/match configuration.
/// Inherits from BaseGameEntityDefinition to be storable in GameCatalog.
/// </summary>
public sealed class MissionDefinition : BaseGameEntityDefinition
{
    public string Name { get; set; }
    public string BoardId { get; set; } // The board required for this mission
    public IReadOnlyDictionary<PlayerId, IBoardPosition> PlayerSpawnZones { get; set; }
    public IReadOnlyDictionary<string, IBoardPosition> NamedLocations { get; set; }
    public MissionObjective? Objective { get; set; }
    public List<SpawnRequest> EntitiesToSpawn { get; set; }
    public IReadOnlyList<ConnectionDescriptor> ConnectionRequests { get; set; }

    public MissionDefinition(string id) : base(id, "Mission")
    {
        PlayerSpawnZones = new Dictionary<PlayerId, IBoardPosition>();
        NamedLocations = new Dictionary<string, IBoardPosition>();
        ConnectionRequests = new List<ConnectionDescriptor>();
        EntitiesToSpawn = new List<SpawnRequest>();
        BoardId = string.Empty;
        Name = id;
    }
}

public class SpawnRequest
{
    public string EntityDefinitionId { get; set; }
    public string ControllerId { get; set; }
    public string SpawnLocationId { get; set; }
    public string? CustomId { get; set; }
    
    public SpawnRequest(string defId, string controller, string loc, string? customId = null)
    {
        EntityDefinitionId = defId;
        ControllerId = controller;
        SpawnLocationId = loc;
        CustomId = customId;
    }
}

/// <summary>
/// Base class for mission objectives.
/// </summary>
public abstract record MissionObjective(string Type);

/// <summary>
/// Eliminate a target number of enemies.
/// </summary>
public record EliminationObjective(int TargetKills) : MissionObjective("Elimination");

/// <summary>
/// Extract to a specific zone.
/// </summary>
public record ExtractionObjective(string ExitZoneName) : MissionObjective("Extraction");

/// <summary>
/// Control specific objective zones.
/// </summary>
public record ControlObjective(IReadOnlyList<string> ObjectiveZones) : MissionObjective("Control");
