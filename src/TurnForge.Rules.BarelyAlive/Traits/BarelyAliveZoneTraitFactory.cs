using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Rules.BarelyAlive.Dto;
using TurnForge.Rules.BarelyAlive.Traits;

namespace TurnForge.Rules.BarelyAlive.Zones;

public static class BarelyAliveZoneTraitFactory
{
    public static IZoneTrait Create(ZoneTraitDto dto)
        => dto.Type switch
        {
            "Dark" => new TraitTypes.DarkZoneTrait(),
            "Indoor" => new TraitTypes.IndoorZoneTrait(),
            _ => throw new NotSupportedException(
                $"ZoneTrait '{dto.Type}' not supported")
        };
}
