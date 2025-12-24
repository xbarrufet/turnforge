using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Spawn;

/// <summary>
/// Request to spawn entities with configurable properties.
/// This is the input to the spawn system from external commands.
/// </summary>
/// <remarks>
/// SpawnRequests are converted to Descriptors by the CommandHandler's preprocessor,
/// which automatically applies definition data and property overrides.
/// 
/// Example:
/// <code>
/// new SpawnRequest(
///     DefinitionId: "Survivors.Mike",
///     Count: 3,
///     Position: new Position(spawnTile),
///     PropertyOverrides: new() { ["Faction"] = "Police" }
/// )
/// </code>
/// </remarks>
public sealed record SpawnRequest(
    string DefinitionId,  // Required: entity definition ID to spawn
    int Count = 1,       // Optional: number of entities to spawn (batch spawn)
    Position Position = default!,  // Optional: spawn position (Position.Empty = strategy decides)
    Dictionary<string, object>? PropertyOverrides = null,  // Optional: override specific descriptor properties
    IEnumerable<IGameEntityComponent>? ExtraComponents = null // Optional: extra components/behaviors
);
