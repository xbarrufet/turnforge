using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Rules.BarelyAlive.Traits;

namespace TurnForge.Rules.BarelyAlive.Actors.Definitions;

public static class BarelyAliveDefinitions
{
    public static readonly UnitDefinition Survivor =
        new(
            TypeId: BarelyAliveTypes.Survivor,
            MaxHealth: 5,
            MaxBaseMovement: 3,
            MaxActionPoints: 2,
            Behaviours: Array.Empty<IActorBehaviour>()
        );

    public static readonly NpcDefinition Zombie =
        new(
            TypeId: BarelyAliveTypes.Zombie,
            MaxHealth: 3,
            MaxBaseMovement: 1,
            MaxActionPoints: 2,
            Behaviours: new IActorBehaviour[]
            {
                new TraitTypes.FastTrait(1)
            }
        );

    public static PropDefinition ZombieSpawn(int order) =>
        new(
            TypeId: BarelyAliveTypes.ZombieSpawn,
            MaxBaseMovement: 0,
            MaxActionPoints: 0,
            Behaviours: new IActorBehaviour[]
            {
                new TraitTypes.SpawnOrderTrait(order)
            },
            MaxHealth: 1
        );

    public static readonly PropDefinition Door =
        new(
            TypeId: BarelyAliveTypes.Door,
            MaxBaseMovement: 0,
            MaxActionPoints: 0,
            Behaviours: Array.Empty<IActorBehaviour>(),
            MaxHealth: 2
        );
}
