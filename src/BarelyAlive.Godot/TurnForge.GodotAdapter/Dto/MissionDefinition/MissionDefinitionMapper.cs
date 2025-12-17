using System;
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities.Actors;

namespace TurnForge.GodotAdapter.Dto.MissionDefinition;


public class MissionDefinitionMapper
{

    public static SpatialDescriptor FromDto(DiscreteSpatialMissionDefinitionDto dto)
    {
        if (dto.SpatialDefinition.Nodes == null)
            throw new ArgumentNullException(nameof(dto), "SpatialDefinition or its Nodes cannot be null");

        var nodes = dto.SpatialDefinition.Nodes
            .Select(n => new Position(n.X, n.Y))
            .Distinct()
            .ToList();

        var connections = dto.SpatialDefinition.Connections == null
            ? new List<DiscreteConnectionDeacriptor>()
            : dto.SpatialDefinition.Connections
                .Select(c => new DiscreteConnectionDeacriptor(
                    new Position(c.From.X, c.From.Y),
                    new Position(c.To.X, c.To.Y)))
                .ToList();

        return new DiscreteSpatialDescriptor(
            Nodes: nodes,
            Connections: connections
        );
    }

    public static IReadOnlyList<ActorDefinition> FromDto(IReadOnlyList<ActorDefinitionDto> actors)
    {
        if (actors == null || actors.Count == 0)
            return Array.Empty<ActorDefinition>();

        var list = new List<ActorDefinition>(actors.Count);

        foreach (var a in actors)
        {
            // Parse ActorKind, fallback to Prop if unknown
            if (!Enum.TryParse<ActorKind>(a.ActorKind, ignoreCase: true, out var kind))
            {
                kind = ActorKind.Prop;
            }

            // Parse ActorId GUID, fallback to new GUID
            ActorId actorId;
            if (Guid.TryParse(a.ActorId, out var guid))
            {
                actorId = new ActorId(guid);
            }
            else
            {
                actorId = ActorId.New();
            }

            var position = new Position(a.InitialPosition.X, a.InitialPosition.Y);

            list.Add(new ActorDefinition(
                Kind: kind,
                ActorId: actorId,
                StartPosition: position,
                customType: string.Empty,
                Traits: null
            ));
        }

        return list;
    }
}