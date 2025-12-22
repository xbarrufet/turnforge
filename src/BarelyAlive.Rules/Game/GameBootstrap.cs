using TurnForge.Engine.Core;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Registration;

namespace BarelyAlive.Rules.Game;

public static class GameBootstrap
{
    public static TurnForge.Engine.Core.TurnForge GameEngineBootstrap(TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null)
    {
        // Simple bootstrap for now. 
        // In a real scenario, this would configure DI, Strategies, etc.

        var context = new GameEngineContext(
            new BarelyAlive.Rules.Adapter.Repositories.InMemoryGameRepository(),
            new BarelyAlive.Rules.Core.Domain.Strategies.Spawn.ConfigurablePropSpawnStrategy(),
            new BarelyAlive.Rules.Core.Domain.Strategies.Spawn.ConfigurableAgentSpawnStrategy(logger),
            logger
        );

        return GameEngineFactory.Build(context);
    }
}
