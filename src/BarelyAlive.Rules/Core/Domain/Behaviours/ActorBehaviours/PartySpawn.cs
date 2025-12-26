using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.ActorBehaviours;

[ActorTrait("PartySpawn")]
public sealed class PartySpawn : ActorTrait, TurnForge.Engine.Components.Interfaces.IGameEntityComponent
{
    // No parameters needed for PartySpawn
}