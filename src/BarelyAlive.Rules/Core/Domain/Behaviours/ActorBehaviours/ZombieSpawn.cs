using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.ActorBehaviours;

[ActorTrait("ZombieSpawn")]
public sealed class ZombieSpawn : ActorTrait, TurnForge.Engine.Components.Interfaces.IGameEntityComponent
{
    // Marker behavior for Zombie Spawn points
}
