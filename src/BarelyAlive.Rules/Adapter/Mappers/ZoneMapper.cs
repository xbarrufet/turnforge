using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Core.Behaviours;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Adapter.Mappers;

public static class ZoneMapper
{
    public static Zone ToZone(ZoneDto dto)
    {
        var zoneId = new ZoneId(dto.Id);

        var bound = dto.Bound.Type switch
        {
            "TileSet" => new TileSetZoneBound(
                dto.Bound.Tiles.Select(t => t.ToPosition())
            ),
            _ => throw new NotSupportedException(
                $"ZoneBound '{dto.Bound.Type}'")
        };

        var Behaviours = dto.Behaviours
            .Select(BarelyAliveBehaviourFactory.CreateZoneBehaviour)
            .ToList();

        return new Zone(zoneId, bound, Behaviours);
    }
}

