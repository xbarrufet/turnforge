using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Actors.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.ActorBehaviours;

[ActorTrait("ZombieSpawn")]
public sealed class ZombieSpawn : ActorTrait, TurnForge.Engine.Components.Interfaces.IGameEntityComponent
{
    // Marker behavior for Zombie Spawn points
}
