using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Core.Behaviours.ActorBehaviours;
using BarelyAlive.Rules.Core.Behaviours.Factories;
using BarelyAlive.Rules.Core.Behaviours.ZoneBehaviours;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Behaviours;

public static class BarelyAliveBehaviourFactory
{
    public static IActorBehaviour CreateActorBehaviour(BehaviourDto dto)
    {
        return ActorBehaviourFactory.Create(dto.Type, dto.ExtensionData);
    }

    public static IZoneBehaviour CreateZoneBehaviour(BehaviourDto dto)
    {
        return ZoneBehaviourFactory.Create(dto.Type, dto.ExtensionData);
    }
}

