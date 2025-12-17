using TurnForge.Rules.BarelyAlive.Actors;
using TurnForge.Rules.BarelyAlive.Strategies.Spawn;
using TurnForge.Engine.Infrastructure;

namespace TurnForge.Rules.BarelyAlive.Bootstrapper;


using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Repositories.InMemory;
using TurnForge.Engine.Repositories.Interfaces;


public static class BarelyAliveEngineBootstrapper
{
    public static GameEngine Boot()
    {
        GameEngineContext context = new GameEngineContext(
            new BarelyAliveActorFactory(),
            new InMemoryGameRepository(),
            new BAPropSpawnStrategy());
        return GameEngineFactory.Build(context);
    }
}
