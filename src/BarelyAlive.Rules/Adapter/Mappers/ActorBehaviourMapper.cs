using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Core.Behaviours.Factories;
using TurnForge.Engine.Entities.Actors.Interfaces;

namespace BarelyAlive.Rules.Adapter.Mappers;

public static class ActorBehaviourMapper
{
    public static IReadOnlyList<IActorBehaviour> ToBehaviours(
        IReadOnlyList<BehaviourDto> dtos)
    {
        return dtos
            .Select(Map)
            .ToList();
    }

    public static IActorBehaviour Map(BehaviourDto dto)
    {
        return ActorBehaviourFactory.Create(dto.Type, dto.ExtensionData);
    }
}