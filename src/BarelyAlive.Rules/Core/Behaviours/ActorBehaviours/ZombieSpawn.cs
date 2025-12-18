using BarelyAlive.Rules.Core.Behaviours.Attributes;

namespace BarelyAlive.Rules.Core.Behaviours.ActorBehaviours;

/// <summary>
/// Behaviour que indica el orden de aparición en spawn de zombis.
/// </summary>

[ActorBehaviour("ZombieSpawn")]
public sealed class ZombieSpawn : TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour
{
    public int Order { get; }

    public ZombieSpawn( [BehaviourParam("order")] int order)
    {
        if (order < 1)
            throw new ArgumentException("Order must be >= 1", nameof(order));

        Order = order;
    }
}

