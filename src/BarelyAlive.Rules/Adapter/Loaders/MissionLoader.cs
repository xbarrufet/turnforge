using System.Linq;
using System.Text.Json;
using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Adapter.Mappers;
using BarelyAlive.Rules.Core.Behaviours;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
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
    public static (SpatialDescriptor, ZoneDescriptor[], PropDescriptor[]) ParseMissionString(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var missionDto = JsonSerializer.Deserialize<MissionDto>(json, options)
                         ?? throw new ArgumentException("Failed to deserialize mission JSON");

        var spatialDescriptor = MapSpatial(missionDto.Spatial);
        var zoneDescriptors = missionDto.Zones.Select(MapZone).ToArray();
        var propDescriptors = missionDto.Props.Select(MapProp).ToArray();

        return (spatialDescriptor, zoneDescriptors, propDescriptors);
    }

    private static SpatialDescriptor MapSpatial(SpatialDto dto)
    {
        if (dto.Type != "DiscreteGraph")
            throw new NotSupportedException($"Spatial type '{dto.Type}' not supported");

        var nodes = dto.Nodes.Select(n => n.ToPosition()).ToList();

        var connections = dto.Connections.Select(c => new DiscreteConnectionDeacriptor(
            c.From.ToPosition(),
            c.To.ToPosition()
        )).ToList();

        return new DiscreteSpatialDescriptor(nodes, connections);
    }

    private static ZoneDescriptor MapZone(ZoneDto dto)
    {
        var zoneId = new ZoneId(dto.Id);

        IZoneBound bound = dto.Bound.Type switch
        {
            "TileSet" => new TileSetZoneBound(dto.Bound.Tiles.Select(t => t.ToPosition())),
            _ => throw new NotSupportedException($"ZoneBound type '{dto.Bound.Type}' not supported")
        };

        var behaviours = dto.Behaviours
            .Select(BarelyAliveBehaviourFactory.CreateZoneBehaviour)
            .ToList();

        return new ZoneDescriptor(zoneId, bound, behaviours);
    }

    private static PropDescriptor MapProp(PropDto dto)
    {

        var typeId = new PropTypeId(dto.TypeId);
        var position = dto.Position?.ToPosition();

        var behaviours = dto.Behaviours
            .Select(BarelyAliveBehaviourFactory.CreateActorBehaviour)
            .ToList();

        return new PropDescriptor(typeId, position, behaviours);
    }
}
