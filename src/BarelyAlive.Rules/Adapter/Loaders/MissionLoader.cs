using System.Linq;
using System.Text.Json;
using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Adapter.Mappers;
using BarelyAlive.Rules.Core.Behaviours;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Descriptors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;
using ArgumentException = System.ArgumentException;
using Array = System.Array;
using InvalidOperationException = System.InvalidOperationException;

namespace BarelyAlive.Rules.Adapter.Loaders;

/// <summary>
/// Cargador de misiones desde archivos JSON.
/// Responsable de deserializar y validar datos de misión.
/// </summary>
public sealed class MissionLoader
{
    //loads from a JSON string and creates the needed objects to call the InitializeGameCommand
    public static (SpatialDescriptor, IReadOnlyList<ZoneDescriptor>, IReadOnlyList<PropDescriptor>, IReadOnlyList<AgentDescriptor>) ParseMissionString(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var missionDto = JsonSerializer.Deserialize<MissionDto>(json, options)
                         ?? throw new ArgumentException("Failed to deserialize mission JSON");

        // 1. Build Coordinate Map (Vector -> TileId)
        var coordinateMap = new Dictionary<Vector, TileId>();

        // Collect nodes
        foreach (var nodeDto in missionDto.Spatial.Nodes)
        {
            var vec = nodeDto.ToPosition();
            if (!coordinateMap.ContainsKey(vec))
            {
                coordinateMap[vec] = TileId.New();
            }
        }

        // 2. Map Spatial
        var spatialDescriptor = MapSpatial(missionDto.Spatial, coordinateMap);

        // 3. Map Zones & Props
        var zoneDescriptors = missionDto.Zones.Select(z => MapZone(z, coordinateMap)).ToArray();
        var propDescriptors = missionDto.Props.Select(p => MapProp(p, coordinateMap)).ToArray();
        var agentDescriptors = Array.Empty<AgentDescriptor>();

        return (spatialDescriptor, zoneDescriptors, propDescriptors, agentDescriptors);
    }

    private static SpatialDescriptor MapSpatial(SpatialDto dto, Dictionary<Vector, TileId> map)
    {
        if (dto.Type != "DiscreteGraph")
            throw new NotSupportedException($"Spatial type '{dto.Type}' not supported");

        var nodes = dto.Nodes
            .Select(n => map[n.ToPosition()])
            .ToList();

        var connections = new List<DiscreteConnectionDeacriptor>();
        foreach (var c in dto.Connections)
        {
            if (map.TryGetValue(c.From.ToPosition(), out var fromId) &&
                map.TryGetValue(c.To.ToPosition(), out var toId))
            {
                connections.Add(new DiscreteConnectionDeacriptor(fromId, toId));
            }
        }

        return new DiscreteSpatialDescriptor(nodes, connections);
    }

    private static ZoneDescriptor MapZone(ZoneDto dto, Dictionary<Vector, TileId> map)
    {
        var zoneId = new ZoneId(dto.Id);

        IZoneBound bound = dto.Bound.Type switch
        {
            "TileSet" => new TileSetZoneBound(
                dto.Bound.Tiles
                   .Select(t => t.ToPosition())
                   .Where(map.ContainsKey) // Ensure we only include valid mapped tiles
                   .Select(v => new Position(map[v]))
            ),
            _ => throw new NotSupportedException($"ZoneBound type '{dto.Bound.Type}' not supported")
        };

        var behaviours = dto.Behaviours
            .Select(BarelyAliveBehaviourFactory.CreateZoneBehaviour)
            .ToList();

        return new ZoneDescriptor(zoneId, bound, behaviours);
    }

    private static PropDescriptor MapProp(PropDto dto, Dictionary<Vector, TileId> map)
    {
        var typeId = new PropTypeId(dto.TypeId);

        Position? position = null;
        if (dto.Position != null)
        {
            var vec = dto.Position.ToPosition();
            if (map.TryGetValue(vec, out var tileId))
            {
                position = new Position(tileId);
            }
        }

        var behaviours = dto.Behaviours
            .Select(BarelyAliveBehaviourFactory.CreateActorBehaviour)
            .ToList();

        return new PropDescriptor(typeId, position, behaviours);
    }
}
