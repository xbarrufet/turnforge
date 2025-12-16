using TurnForge.Engine.Commands.Game.Definitions;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Rules.BarelyAlive.Actors;

public class BarelyAliveActorFactory:IActorFactory
{
    public Actor CreateActor(ActorDefinition def)
    {
        return def.Kind switch
        {
            ActorKind.Unit =>
                new Unit(def.ActorId, def.StartPosition),

            ActorKind.Hostile =>
                new Hostile(def.ActorId, def.StartPosition),
            ActorKind.Prop =>
                new Prop(def.ActorId, def.StartPosition),
                
            _ => throw new NotSupportedException()
        };
    }
}