using TurnForge.Engine.APIs;
using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Infrastructure.Factories;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Infrastructure;

/// <summary>
/// Factory oficial del engine.
/// Ensambla TODA la infraestructura estándar del engine,
/// pero NO decide implementaciones concretas del juego.
/// </summary>
public static class GameEngineFactory
{
    public static Core.TurnForge Build(
        GameEngineContext gameEngineContext)
    {
        // 1️⃣ ServiceProvider del engine
        var services = new SimpleServiceProvider();

        // 1️⃣ Servicios internos del engine
        services.RegisterSingleton<IGameFactory>(new SimpleGameFactory());

        var gameCatalog = new InMemoryGameCatalog();
        services.RegisterSingleton<IGameCatalog>(gameCatalog);
        services.RegisterSingleton<IActorFactory>(
            new GenericActorFactory(
                services.Resolve<IGameCatalog>()
            ));


        // 2️⃣ Dependencias externas (decididas por el host/juego)
        services.RegisterSingleton(gameEngineContext.GameRepository);
        services.RegisterSingleton(gameEngineContext.PropSpawnStrategy);
        services.RegisterSingleton(gameEngineContext.AgentSpawnStrategy);

        // 3️⃣ Registro de comandos y handlers propios del engine
        EngineCommandRegistration.Register(services);

        // 4️⃣ Resolver de handlers (engine infra)
        var resolver =
            new ServiceProviderCommandHandlerResolver(services);



        //Infraestructura interna adicional
        var effectSink = new ObservableEffectSink();
        var gameLoop = new GameLoop();
        services.RegisterSingleton<IEffectSink>(effectSink);
        // 7️⃣ CommandBus (engine infra)
        var commandBus = new CommandBus(
            gameLoop,
            resolver
        );

        // 8️⃣ TurnForge (fachada pública)
        var runtime = new GameEngineRuntime(commandBus, effectSink, gameEngineContext.GameRepository);
        var catalogApi = new GameCatalogApi(gameCatalog);
        return new Core.TurnForge(runtime, catalogApi);
    }
}