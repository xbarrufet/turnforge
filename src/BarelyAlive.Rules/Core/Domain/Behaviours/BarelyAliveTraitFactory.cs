using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Core.Domain.Behaviours.ActorBehaviours;
using BarelyAlive.Rules.Core.Domain.Behaviours.Factories;
using BarelyAlive.Rules.Core.Domain.Behaviours.ZoneBehaviours;
using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Definitions.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours;

public static class BarelyAliveTraitFactory
{
    public static TurnForge.Engine.Components.Interfaces.IGameEntityComponent CreateActorTrait(TraitDto dto)
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

        // Keep using ActorTraitFactory for legacy/other traits
        // Cast to IGameEntityComponent (assuming they implement it or we wrap them)
        var trait = ActorTraitFactory.Create(dto.Type, dto.ExtensionData);
        if (trait is TurnForge.Engine.Components.Interfaces.IGameEntityComponent componentTrait)
        {
            return componentTrait;
        }
        
        throw new InvalidCastException($"Trait {trait.GetType().Name} does not implement IGameEntityComponent");
    }

    public static IZoneTrait CreateZoneTrait(TraitDto dto)
    {
        return ZoneTraitFactory.Create(dto.Type, dto.ExtensionData);
    }
}

