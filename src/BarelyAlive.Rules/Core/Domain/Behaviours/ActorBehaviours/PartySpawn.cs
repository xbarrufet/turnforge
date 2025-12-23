using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.ActorBehaviours;

[ActorBehaviour("PartySpawn")]
public sealed class PartySpawn : ActorBehaviour, TurnForge.Engine.Components.Interfaces.IGameEntityComponent
{
    // No parameters needed for PartySpawn
}