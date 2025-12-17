using TurnForge.Engine.Commands.Game.Definitions;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Rules.BarelyAlive.Actors;

public class ZombieSpawnActor:Prop
{
    public ZombieSpawnActor(ActorId id, Position position,
        IReadOnlyDictionary<string, ActorTraitDefinition> definitionTraits) : base(id, position, "ZombieSpawnPoint")
    {
        var orderStr = definitionTraits.TryGetValue("SpawnOrder", out var traitDefinition)
            ? traitDefinition.Attributes?["order"]
            : null;
        if (orderStr != null && int.TryParse(orderStr, out var order))
        {
            AddTrait(new ZombieSpawnOrderTrait(order));    
        }
        else
        {
            AddTrait(new ZombieSpawnOrderTrait(1));
        }
            
    }
}