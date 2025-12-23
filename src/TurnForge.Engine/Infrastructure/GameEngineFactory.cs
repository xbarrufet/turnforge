using TurnForge.Engine.APIs;
using TurnForge.Engine.Appliers.Spawn;
using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Appliers.Entity;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Entities.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Infrastructure.Factories;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
using TurnForge.Engine.Core.Orchestrator;
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
        services.RegisterSingleton<IBoardFactory>(new BoardFactory());

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

        // 7️⃣ CommandBus (engine infra)
        // Command validation now handled by FSM
        var commandBus = new CommandBus(resolver);

        // 7.5 Orchestrator & Appliers
        var orchestrator = new TurnForgeOrchestrator();

        // Resolve factories for appliers
        var actorFactory = services.Resolve<IActorFactory>();
        if (actorFactory is not GenericActorFactory genericActorFactory)
        {
            throw new InvalidOperationException($"Expected implementation {nameof(GenericActorFactory)} for IActorFactory");
        }
        var boardFactory = services.Resolve<IBoardFactory>();

        // Register New Spawn Appliers (using new spawn pipeline)
        orchestrator.RegisterApplier(new TurnForge.Engine.Appliers.Board.InitializeBoardApplier());
        orchestrator.RegisterApplier(new AgentSpawnApplier(genericActorFactory));
        orchestrator.RegisterApplier(new PropSpawnApplier(genericActorFactory));

        // 8️⃣ TurnForge (fachada pública)
        var runtime = new GameEngineRuntime(
            commandBus, 
            gameEngineContext.GameRepository, 
            orchestrator, 
            gameEngineContext.Logger,
            boardFactory); // Inject BoardFactory
        var catalogApi = new GameCatalogApi(gameCatalog);
        return new Core.TurnForge(runtime, catalogApi);
    }
}