using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Core.Domain.Behaviours.ActorBehaviours;
using BarelyAlive.Rules.Core.Domain.Behaviours.Factories;
using BarelyAlive.Rules.Core.Domain.Behaviours.ZoneBehaviours;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours;

public static class BarelyAliveBehaviourFactory
{
    public static TurnForge.Engine.Components.Interfaces.IGameEntityComponent CreateActorBehaviour(BehaviourDto dto)
    {
        if (dto.Type == "ZombieSpawn")
        {
            var component = new BarelyAlive.Rules.Core.Domain.Entities.ZombieSpawnComponent();
            if (dto.ExtensionData.TryGetValue("order", out var orderJson))
            {
                component.Order = System.Text.Json.JsonSerializer.Deserialize<int>(orderJson);
            }
            return component;
        }

        // Keep using ActorBehaviourFactory for legacy/other behaviors
        // Cast to IGameEntityComponent (assuming they implement it or we wrap them)
        var behaviour = ActorBehaviourFactory.Create(dto.Type, dto.ExtensionData);
        if (behaviour is TurnForge.Engine.Components.Interfaces.IGameEntityComponent componentBehaviour)
        {
            return componentBehaviour;
        }
        
        throw new InvalidCastException($"Behaviour {behaviour.GetType().Name} does not implement IGameEntityComponent");
    }

    public static IZoneBehaviour CreateZoneBehaviour(BehaviourDto dto)
    {
        return ZoneBehaviourFactory.Create(dto.Type, dto.ExtensionData);
    }
}

