using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Appliers;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Infrastructure;

/// <summary>
/// Factory oficial del engine.
/// Ensambla TODA la infraestructura estándar del engine,
/// pero NO decide implementaciones concretas del juego.
/// </summary>
public static class GameEngineFactory
{
    public static GameEngine Build(
        GameEngineContext gameEngineContext)
    {
        // 1️⃣ ServiceProvider del engine
        var services = new SimpleServiceProvider();

        // 1️⃣ Servicios internos del engine
        services.RegisterSingleton<IGameFactory>(new SimpleGameFactory());
        
        // 2️⃣ Dependencias externas (decididas por el host/juego)
        services.RegisterSingleton<IGameRepository>(gameEngineContext.GameRepository);
        
        services.RegisterSingleton<IDefinitionRegistry<PropTypeId, PropDefinition>>(gameEngineContext.PropDefinitions);
        services.RegisterSingleton<IDefinitionRegistry<UnitTypeId, UnitDefinition>>(gameEngineContext.UnitDefinitions);
        services.RegisterSingleton<IDefinitionRegistry<NpcTypeId, NpcDefinition>>(gameEngineContext.NpcDefinitions);
        services.RegisterSingleton<IActorFactory>(
            new GenericActorFactory(
                services.Resolve<IDefinitionRegistry<PropTypeId, PropDefinition>>(),
                services.Resolve<IDefinitionRegistry<UnitTypeId, UnitDefinition>>(),
                services.Resolve<IDefinitionRegistry<NpcTypeId, NpcDefinition>>()
            )
        );



        
        services.RegisterSingleton<IPropSpawnStrategy>(gameEngineContext.PropSpawnStrategy);
        services.RegisterSingleton<IUnitSpawnStrategy>(gameEngineContext.UnitSpawnStrategy);
        services.RegisterSingleton<IEffectSink>(new ObservableEffectSink());
        
        // 3️⃣ Registro de comandos y handlers propios del engine
        EngineCommandRegistration.Register(services);

        // 4️⃣ Resolver de handlers (engine infra)
        var resolver =
            new ServiceProviderCommandHandlerResolver(services);

        // 5️⃣ EventBus (engine infra)
        var effectSink = new ObservableEffectSink();

        // 6️⃣ GameLoop (engine infra)
        var gameLoop = new GameLoop();

        // 7️⃣ CommandBus (engine infra)
        var commandBus = new CommandBus(
            gameLoop,
            resolver,
            effectSink
        );

        // 8️⃣ GameEngine (fachada pública)
        return new GameEngine(commandBus);
    }
}