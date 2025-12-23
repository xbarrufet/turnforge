using TurnForge.Engine.Core;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Registration;

namespace BarelyAlive.Rules.Game;

public static class GameBootstrap
{
    public static TurnForge.Engine.Core.TurnForge GameEngineBootstrap(TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null)
    {
        // Simple bootstrap for now. 
        // In a real scenario, this would configure DI, Strategies, etc.

        // Note: Strategies now need GenericActorFactory.
        // The factory is created and injected by GameEngineFactory from the catalog.
        // For now, pass null - GameEngineFactory will resolve dependencies
        
        var context = new GameEngineContext(
            new BarelyAlive.Rules.Adapter.Repositories.InMemoryGameRepository(),
            null, // PropStrategy - will be set by GameEngineFactory
            null, // AgentStrategy - will be set by GameEngineFactory
            logger
        );

        return GameEngineFactory.Build(context);
    }
}
