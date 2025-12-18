using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Core.Behaviours.ActorBehaviours;
using BarelyAlive.Rules.Core.Behaviours.ZoneBehaviours;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Behaviours;

public static class BarelyAliveBehaviourFactory
{
    public static IActorBehaviour CreateActorBehaviour(BehaviourDto dto)
    {
        return dto.Type switch
        {
            ActorBehaviourNames.ZombieSpawn =>
                new ZombieSpawn(
                    dto.Data.GetProperty("order").GetInt32()
                ),
            _ => throw new NotSupportedException(
                $"Behaviour '{dto.Type}' not supported")
        };
    }
    
    public static IZoneBehaviour CreateZoneBehaviour(BehaviourDto dto)
    {
        return dto.Type switch
        {
            ZoneBehaviourNames.DarkZoneBehaviour =>
                new DarkZoneBehaviour(),
            ZoneBehaviourNames.IndoorZoneBehaviour =>
                new IndoorZoneBehaviour(),
            _ => throw new NotSupportedException(
                $"Behaviour '{dto.Type}' not supported")
        };
    }
}
