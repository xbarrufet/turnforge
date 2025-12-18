using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Rules.BarelyAlive.Dto;

namespace TurnForge.Rules.BarelyAlive.Traits;

public static class BarelyAliveTraitFactory
{
    public static IActorBehaviour Create(TraitDto dto)
    {
        return dto.Type switch
        {
            "SpawnOrder" =>
                new TraitTypes.SpawnOrderTrait(
                    int.Parse(dto.Attributes.First(a => a.Name == "order").Value)
                ),
            "Fast" =>
                new TraitTypes.FastTrait(
                    int.Parse(dto.Attributes.First(a => a.Name == "movement").Value)
                ),

            _ => throw new NotSupportedException(
                $"Trait '{dto.Type}' not supported")
        };
    }
}
