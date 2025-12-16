using TurnForge.Rules.BarelyAlive.Actors;

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
        // 1️⃣ Crear ServiceProvider
        var services = new SimpleServiceProvider();

        // 2️⃣ Registrar repositorios
        services.RegisterSingleton<IGameRepository>(
            new InMemoryGameRepository()
        );

        // 3️⃣ Registrar factories del juego
        services.RegisterSingleton<IActorFactory>(
            new BarelyAliveActorFactory()
        );

        // 4️⃣ Registrar handlers
        services.Register<LoadGameHandler>(sp =>
            new LoadGameHandler(
                (IActorFactory)sp.GetService(typeof(IActorFactory))!,
                (IGameRepository)sp.GetService(typeof(IGameRepository))!
            )
        );

        services.Register<ICommandHandler<LoadGameCommand>>(sp =>
            (LoadGameHandler)sp.GetService(typeof(LoadGameHandler))!
        );

        // 5️⃣ Crear resolver
        var resolver =
            new ServiceProviderCommandHandlerResolver(services);

        // 6️⃣ Crear EventBus
        var eventBus = new EventBus();

        // 7️⃣ Crear GameLoop
        var gameLoop = new GameLoop();

        // 8️⃣ Crear CommandBus
        var commandBus = new CommandBus(
            gameLoop,
            resolver,
            eventBus
        );

        // 9️⃣ Crear GameEngine
        return new GameEngine(commandBus);
    }
}
