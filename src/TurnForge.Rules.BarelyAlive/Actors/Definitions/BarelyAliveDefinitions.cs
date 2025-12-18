using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Rules.BarelyAlive.Traits;

namespace TurnForge.Rules.BarelyAlive.Actors.Definitions;

public static class BarelyAliveDefinitions
{
    public static readonly UnitDefinition Survivor =
        new(
            TypeId: BarelyAliveTypes.Survivor,
            InitialMaxHealth: 5,
            Behaviours: Array.Empty<IActorBehaviour>()
        );

    public static readonly NpcDefinition Zombie =
        new(
            TypeId: BarelyAliveTypes.Zombie,
            MaxHealth: 3,
            BaseMovement: 1,
            Behaviours: new IActorBehaviour[]
            {
                new TraitTypes.FastTrait(1)
            }
        );

    public static PropDefinition ZombieSpawn(int order) =>
        new(
            TypeId: BarelyAliveTypes.ZombieSpawn,
            MaxHealth: null,
            Behaviours: new IActorBehaviour[]
            {
                new TraitTypes.SpawnOrderTrait(order)
            }
        );

    public static readonly PropDefinition Door =
        new(
            TypeId: BarelyAliveTypes.Door,
            MaxHealth: 2,
            Behaviours: Array.Empty<IActorBehaviour>()
        );
}
