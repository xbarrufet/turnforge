using TurnForge.Engine.Commands.Game.Definitions;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;

internal sealed class TestActorFactory : IActorFactory
{
    public Actor CreateActor(ActorDefinition def)
    {
        return def.Kind switch
        {
            ActorKind.Prop =>
                new Prop(def.ActorId, def.StartPosition),

            _ =>
                throw new NotSupportedException(
                    "Only Prop actors supported in engine tests")
        };
    }
}