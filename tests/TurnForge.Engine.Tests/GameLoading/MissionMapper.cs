using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Game.Definitions;
using TurnForge.Engine.Tests.GameLoading.Dto;
using TurnForge.Engine.ValueObjects;
using TurnForge.Rules.BarelyAlive;

namespace TurnForge.Engine.Tests.GameLoading;

public sealed class MissionMapper
{
    public MissionMetadata GetMetadata(MissionDto dto)
    {
        return new MissionMetadata(
            missionName: dto.MissionName,
            mapSize: Size.Parse(dto.MapSize)
        );
    }

    public LoadGameCommand Map(MissionDto dto)
    {
        if (dto.Areas.Count == 0)
            throw new InvalidOperationException(
                "Mission must contain at least one area");

        // 1️⃣ Cada Area => un nodo discreto
        var nodes = dto.Areas
            .Select(a => new Position(a.X, a.Y))
            .Distinct()
            .ToList();

        // 2️⃣ Conexiones espaciales puras
        var connections = dto.Connections
            .Select(c => new DiscreteConnectionDefinition(
                From: GetAreaPosition(dto, c.AreaFromId),
                To: GetAreaPosition(dto, c.AreaToId)
            ))
            .ToList();

        var spatial = new DiscreteSpatialDefinition(
            Nodes: nodes,
            Connections: connections
        );

        return new LoadGameCommand(
            spatial: spatial,
            actors: Array.Empty<ActorDefinition>()
        );
    }

    // -------------------------------------------------

    private static Position GetAreaPosition(
        MissionDto mission,
        string areaId)
    {
        var area = mission.Areas.First(a => a.Id == areaId);
        return new Position(area.X, area.Y);
    }
}