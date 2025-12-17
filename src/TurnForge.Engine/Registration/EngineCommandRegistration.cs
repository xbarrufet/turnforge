using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.GameStart;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Registration;

/// <summary>
/// Registro centralizado de TODOS los CommandHandlers
/// que pertenecen al ENGINE (no a los juegos).
///
/// Define QUÉ capacidades ofrece el engine,
/// pero NO decide implementaciones concretas.
/// </summary>
public static class EngineCommandRegistration
{
    public static void Register(SimpleServiceProvider services)
    {
        // =====================================================
        // LoadGame
        // =====================================================

        services.Register<LoadGameHandler>(sp =>
            new LoadGameHandler(
                (IActorFactory)sp.GetService(typeof(IActorFactory))!,
                (IGameRepository)sp.GetService(typeof(IGameRepository))!,
                (IPropSpawnStrategy)sp.GetService(typeof(IPropSpawnStrategy))!,
                (IEffectSink)sp.GetService(typeof(IEffectSink))!
            )
        );
        services.Register<ICommandHandler<LoadGameCommand>>(sp =>
            (LoadGameHandler)sp.GetService(typeof(LoadGameHandler))!
        );

        // =====================================================
        // FUTURO (ejemplos):
        // - MoveActorHandler
        // - AttackHandler
        // - EndTurnHandler
        // - SaveGameHandler
        // =====================================================
        
        services.Register<GameStartCommandHandler>(sp =>
            new GameStartCommandHandler(
                (IGameRepository)sp.GetService(typeof(IGameRepository))!,
                (IUnitSpawnStrategy)sp.GetService(typeof(IUnitSpawnStrategy))!,
                (IActorFactory)sp.GetService(typeof(IActorFactory))!,
                (IEffectSink)sp.GetService(typeof(IPropSpawnStrategy))!
            )
        );
        services.Register<ICommandHandler<GameStartCommand>>(sp =>
            (GameStartCommandHandler)sp.GetService(typeof(GameStartCommandHandler))!
        );
    }
}