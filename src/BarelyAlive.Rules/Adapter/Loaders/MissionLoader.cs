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

        // 1. Build Maps
        var idToTileMap = new Dictionary<string, TileId>();
        var vectorToTileMap = new Dictionary<Vector, TileId>();

        // Collect nodes
        foreach (var nodeDto in missionDto.Spatial.Nodes)
        {
             if (Guid.TryParse(nodeDto.Id, out var checklistGuid))
             {
                 var tileId = new TileId(checklistGuid);
                 idToTileMap[nodeDto.Id] = tileId;
                 var vec = new Vector(nodeDto.X, nodeDto.Y);
                 vectorToTileMap[vec] = tileId;
             }
        }

        // 2. Map Spatial (Including Zones now)
        var (spatialDescriptor, zoneDescriptors) = MapSpatialAndZones(missionDto.Spatial, idToTileMap);

        // Map explicit props (Spawns, Doors, Areas)
        // Pass both maps to MapProp to handle different positioning types
        var propRequests = missionDto.Props.Select(p => MapProp(p, idToTileMap, vectorToTileMap)).ToList();
        
        var agentDescriptors = missionDto.Agents.Select(a => MapAgent(a, vectorToTileMap)).ToArray();

        return (spatialDescriptor, zoneDescriptors.ToArray(), propRequests.ToArray(), agentDescriptors);
    }

    private static SpawnRequest MapAgent(AgentDto dto, Dictionary<Vector, TileId> map)
    {
        var category = dto.Category;
        var agentName = dto.AgentName;

        Position position = Position.Empty;
        if (dto.Position != null)
        {
            var posDto = (PositionDto)dto.Position;
            var vec = posDto.ToPosition(); // Ensure ToPosition uses just X,Y
            if (map.TryGetValue(vec, out var tileId))
            {
                position = new Position(tileId);
            }
        }
        // Agents in mission must have a position
        if (position == Position.Empty) throw new ArgumentException($"Agent {agentName} must have a valid position in the mission.");

        var behaviours = dto.Behaviours
            .Select(BarelyAliveBehaviourFactory.CreateActorBehaviour)
            .Cast<IGameEntityComponent>() 
            .ToList();

        // Create SpawnRequest instead of Descriptor
        return new SpawnRequest(
            DefinitionId: agentName,
            Position: position,
            ExtraComponents: behaviours
        );
    }
    

    private static (SpatialDescriptor, List<ZoneDescriptor>) MapSpatialAndZones(SpatialDto dto, Dictionary<string, TileId> idMap)
    {
        if (dto.Type != "DiscreteGraph")
            throw new NotSupportedException($"Spatial type '{dto.Type}' not supported");

        var nodes = dto.Nodes
            .Where(n => idMap.ContainsKey(n.Id))
            .Select(n => idMap[n.Id])
            .ToList();

        var connections = new List<DiscreteConnectionDescriptor>();
        foreach (var c in dto.Connections)
        {
            if (idMap.TryGetValue(c.From, out var fromId) &&
                idMap.TryGetValue(c.To, out var toId))
            {
                // Parse Connection ID
                var connectionId = !string.IsNullOrEmpty(c.Id) && Guid.TryParse(c.Id, out var cid) 
                    ? new ConnectionId(cid) 
                    : ConnectionId.New();

                connections.Add(new DiscreteConnectionDescriptor(connectionId, fromId, toId));
            }
        }
        
        // Map Zones from Spatial
        var zoneDescriptors = new List<ZoneDescriptor>();
        foreach(var z in dto.Zones)
        {
             var zoneId = new ZoneId(z.Id);
             IZoneBound bound = z.Bound.Type switch
             {
                 "TileSet" => new TileSetZoneBound(
                     z.Bound.Tiles
                        .Where(idMap.ContainsKey) 
                        .Select(id => new Position(idMap[id]))
                 ),
                 _ => throw new NotSupportedException($"ZoneBound type '{z.Bound.Type}' not supported")
             };
             zoneDescriptors.Add(new ZoneDescriptor(zoneId, bound));
        }

        return (new DiscreteSpatialDescriptor(nodes, connections), zoneDescriptors);
    }
    
    // MapZone method removed as it is now integrated into spatial mapping and no longer generates props.

    private static SpawnRequest MapProp(PropDto dto, Dictionary<string, TileId> idMap, Dictionary<Vector, TileId> vectorMap)
    {
        var definitionId = dto.TypeId;

        Position position = Position.Empty;
        if (dto.Position != null)
        {
            if (dto.Position is JsonElement elem)
            {
                if (elem.ValueKind == JsonValueKind.String)
                {
                    var idStr = elem.GetString();
                    if (!string.IsNullOrEmpty(idStr))
                    {
                        if (idMap.TryGetValue(idStr, out var tileId))
                        {
                            position = new Position(tileId);
                        }
                        else if (Guid.TryParse(idStr, out var guid))
                        {
                            position = new Position(new ConnectionId(guid));
                        }
                    }
                }
                else if (elem.ValueKind == JsonValueKind.Object)
                {
                    var posDto = JsonSerializer.Deserialize<PositionDto>(elem.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (posDto != null)
                    {
                        var vec = new Vector(posDto.X, posDto.Y);
                        if (vectorMap.TryGetValue(vec, out var tileId))
                        {
                            position = new Position(tileId);
                        }
                    }
                }
                else if (elem.ValueKind == JsonValueKind.Array)
                {
                    // Array of IDs (strings) for Area
                    var ids = new List<string>();
                    foreach (var item in elem.EnumerateArray())
                    {
                        if(item.ValueKind == JsonValueKind.String) ids.Add(item.GetString() ?? "");
                    }
                    
                    var tiles = ids.Where(idMap.ContainsKey).Select(id => idMap[id]).ToArray();
                    if (tiles.Any())
                    {
                        position = new Position(tiles);
                    }
                }
            }
        }

        var actorComponents = new List<IGameEntityComponent>();
        var zoneBehaviours = new List<IZoneBehaviour>();

        foreach (var bDto in dto.Behaviours)
        {
            // Special handling for Zombies/Spawns handled in BarelyAliveBehaviourFactory
            // Usually BarelyAliveBehaviourFactory.CreateActorBehaviour handles "ZombieSpawn"
            // We can check if it's that OR registered in ActorFactory.
            if (bDto.Type == "ZombieSpawn" || 
                BarelyAlive.Rules.Core.Domain.Behaviours.Factories.ActorBehaviourFactory.IsRegistered(bDto.Type))
            {
                var component = BarelyAliveBehaviourFactory.CreateActorBehaviour(bDto);
                if (component is IGameEntityComponent c) actorComponents.Add(c);
            }
            else if (BarelyAlive.Rules.Core.Domain.Behaviours.Factories.ZoneBehaviourFactory.IsRegistered(bDto.Type))
            {
                // It's a zone behavior (Indoor, Dark)
                var zb = BarelyAliveBehaviourFactory.CreateZoneBehaviour(bDto);
                zoneBehaviours.Add(zb);
            }
            else
            {
                throw new NotSupportedException($"Behaviour '{bDto.Type}' is not registered as Actor or Zone behaviour.");
            }
        }
        
        if (zoneBehaviours.Any())
        {
            actorComponents.Add(new BarelyAlive.Rules.Core.Domain.Components.ZoneEffectComponent(zoneBehaviours));
        }

        return new SpawnRequest(
            DefinitionId: definitionId,
            Position: position,
            ExtraComponents: actorComponents
        );
    }
}
