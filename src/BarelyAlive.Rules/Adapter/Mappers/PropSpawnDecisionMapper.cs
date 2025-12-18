using BarelyAlive.Rules.Adapter.Dto;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Adapter.Mappers;

public static class PropSpawnDecisionMapper
{
    public static PropSpawnDecision ToDecision(PropDto dto)
    {
        return new PropSpawnDecision(
            typeId: new PropTypeId(dto.TypeId),
            position: new Position(dto.Position.X, dto.Position.Y),
            behaviours: ActorBehaviourMapper.ToBehaviours(dto.Behaviours)
        );
    }
}