using System.Collections.Generic;
using System.Linq;
using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Core.Domain.Behaviours.Factories;
using TurnForge.Engine.Definitions.Actors.Interfaces;

namespace BarelyAlive.Rules.Adapter.Mappers;

public static class ActorTraitMapper
{
    public static IReadOnlyList<IActorTrait> ToTraits(
        IReadOnlyList<TraitDto> dtos)
    {
        return dtos
            .Select(Map)
            .ToList();
    }

    public static IActorTrait Map(TraitDto dto)
    {
        return ActorTraitFactory.Create(dto.Type, dto.ExtensionData);
    }
}