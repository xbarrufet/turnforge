using System.Collections.Generic;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

/// <summary>
/// Immutable mission data containing spawn zones, objectives, and named locations.
/// </summary>
public record MissionData(
    string MissionId,
    string Name,
    IReadOnlyDictionary<PlayerId, IBoardPosition> PlayerSpawnZones,
    IReadOnlyDictionary<string, IBoardPosition> NamedLocations,  // "ExitZone", "ObjectiveA", etc.
    MissionObjective? Objective
);

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
