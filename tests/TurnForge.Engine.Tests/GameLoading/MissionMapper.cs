using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Tests.GameLoading.Dto;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.GameLoading;

public sealed class MissionMapper
{
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
            .Select(c => new DiscreteConnectionDeacriptor(
                From: GetAreaPosition(dto, c.AreaFromId),
                To: GetAreaPosition(dto, c.AreaToId)
            ))
            .ToList();

        var spatial = new DiscreteSpatialDescriptor(
            Nodes: nodes,
            Connections: connections
        );

        // 3️⃣ Props desde actores con customType
        var props = dto.Actors
            .Where(a => a.ActorKind == "Prop")
            .Select(a => new PropDescriptor(
                Id: ActorId.New(),
                CustomType: a.CustomType ?? string.Empty,
                Traits: MapActorTraits(a),
                Position: GetAreaPosition(dto, a.Position)
            ))
            .ToList();

        return new LoadGameCommand(
            spatial: spatial,
            props: props
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

    private static IReadOnlyDictionary<string, IReadOnlyList<ActorTraitDefinition>> MapActorTraits(ActorDto actor)
    {
        var traits = new Dictionary<string, IReadOnlyList<ActorTraitDefinition>>();

        foreach (var trait in actor.Traits)
        {
            var attributes = trait.Attributes
                .ToDictionary(attr => attr.Name, attr => attr.Value);

            traits[trait.Type] = new List<ActorTraitDefinition>
            {
                new(Name: trait.Type, Attributes: attributes)
            };
        }

        return traits;
    }
}