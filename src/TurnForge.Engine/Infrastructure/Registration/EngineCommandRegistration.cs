using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.GameStart;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Appliers;
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

        services.Register<InitializeGameHandler>(sp =>
            new InitializeGameHandler(
                (IActorFactory)sp.GetService(typeof(IActorFactory))!,
                (IGameFactory)sp.GetService(typeof(IGameFactory))!,
                (IGameRepository)sp.GetService(typeof(IGameRepository))!,
                (IPropSpawnStrategy)sp.GetService(typeof(IPropSpawnStrategy))!,
                (IEffectSink)sp.GetService(typeof(IEffectSink))!
            )
        );
        services.Register<ICommandHandler<InitializeGameCommand>>(sp =>
            (InitializeGameHandler)sp.GetService(typeof(InitializeGameHandler))!
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
                (IAgentSpawnStrategy)sp.GetService(typeof(IAgentSpawnStrategy))!,
                (IActorFactory)sp.GetService(typeof(IActorFactory))!,
                (IEffectSink)sp.GetService(typeof(IEffectSink))!
            )
        );
        services.Register<ICommandHandler<GameStartCommand>>(sp =>
            (GameStartCommandHandler)sp.GetService(typeof(GameStartCommandHandler))!
        );
    }
}