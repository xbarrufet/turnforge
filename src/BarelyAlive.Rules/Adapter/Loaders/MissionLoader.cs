using System.Linq;
using System.Text.Json;
using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Adapter.Mappers;
using BarelyAlive.Rules.Core.Domain.Behaviours;
using TurnForge.Engine.Commands.Spawn.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
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
    public static (SpatialDescriptor, IReadOnlyList<ZoneDescriptor>, IReadOnlyList<SpawnRequest>, IReadOnlyList<SpawnRequest>) ParseMissionString(string json)
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
        var zoneDescriptors = new List<ZoneDescriptor>();
        var generatedProps = new List<SpawnRequest>();

        foreach (var zone in missionDto.Zones)
        {
            var (descriptor, requests) = MapZone(zone, coordinateMap);
            zoneDescriptors.Add(descriptor);
            generatedProps.AddRange(requests);
        }

        var propRequests = missionDto.Props.Select(p => MapProp(p, coordinateMap)).ToList();
        propRequests.AddRange(generatedProps);
        
        var agentDescriptors = missionDto.Agents.Select(a => MapAgent(a, coordinateMap)).ToArray();

        return (spatialDescriptor, zoneDescriptors.ToArray(), propRequests.ToArray(), agentDescriptors);
    }

    private static SpawnRequest MapAgent(AgentDto dto, Dictionary<Vector, TileId> map)
    {
        var category = dto.Category;
        var agentName = dto.AgentName;

        Position? position = null;
        if (dto.Position != null)
        {
            var posDto = (PositionDto)dto.Position;
            var vec = posDto.ToPosition();
            if (map.TryGetValue(vec, out var tileId))
            {
                position = new Position(tileId);
            }
        }
        // Agents in mission must have a position
        if (position == null) throw new ArgumentException($"Agent {agentName} must have a valid position in the mission.");

        // Behaviors are mapped from DTO if present (assuming factory creates base components)
        var behaviours = dto.Behaviours
            .Select(BarelyAliveBehaviourFactory.CreateActorBehaviour)
            .Cast<IGameEntityComponent>() // Cast to common interface
            .ToList();

        // Create SpawnRequest instead of Descriptor
        return new SpawnRequest(
            DefinitionId: agentName, // AgentName is DefinitionId in this context
            Position: position,
            ExtraComponents: behaviours
        );
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

    private static (ZoneDescriptor, IEnumerable<SpawnRequest>) MapZone(ZoneDto dto, Dictionary<Vector, TileId> map)
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

        var requests = new List<SpawnRequest>();
        if (behaviours.Any())
        {
            var tiles = dto.Bound.Tiles
                .Select(t => t.ToPosition())
                .Where(map.ContainsKey) // Ensure we only include valid mapped tiles
                .Select(v => map[v])
                .ToArray();

            var component = new BarelyAlive.Rules.Core.Domain.Components.ZoneEffectComponent(behaviours);
            requests.Add(new SpawnRequest(
                DefinitionId: "BarelyAlive.ZoneEffect",
                Position: new Position(tiles),
                ExtraComponents: new List<IGameEntityComponent> { component }
            ));
        }

        return (new ZoneDescriptor(zoneId, bound, new List<IZoneBehaviour>()), requests);
    }

    private static SpawnRequest MapProp(PropDto dto, Dictionary<Vector, TileId> map)
    {
        var definitionId = dto.TypeId;

        Position? position = null;
        if (dto.Position != null)
        {
            var vector = new TurnForge.Engine.ValueObjects.Vector(dto.Position.X, dto.Position.Y);
            if (map.TryGetValue(vector, out var tileId))
            {
                position = new TurnForge.Engine.ValueObjects.Position(tileId);
            }
        }

        // Behaviors are mapped from DTO if present
        // Using CreateActorBehaviour as props share similar behavior structure in this context
        var behaviours = dto.Behaviours
            .Select(b => 
            {
                var obj = BarelyAliveBehaviourFactory.CreateActorBehaviour(b);
                if (obj is IGameEntityComponent c) return c;
                throw new InvalidCastException($"Behaviour {obj.GetType().Name} does not implement IGameEntityComponent");
            })
            .ToList();

        // Create SpawnRequest for Prop
        return new SpawnRequest(
            DefinitionId: definitionId,
            Position: position,
            ExtraComponents: behaviours
        );
    }
}
