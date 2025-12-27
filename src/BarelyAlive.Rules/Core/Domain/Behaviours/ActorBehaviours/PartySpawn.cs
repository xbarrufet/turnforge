using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Actors.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.ActorBehaviours;

[ActorTrait("PartySpawn")]
public sealed class PartySpawn : ActorTrait, TurnForge.Engine.Components.Interfaces.IGameEntityComponent
{
    // No parameters needed for PartySpawn
}