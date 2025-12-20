using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
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

        // =====================================================
        // Game Initialization
        // =====================================================

        services.Register<InitGameCommandHandler>(sp =>
            new InitGameCommandHandler(
                (IActorFactory)sp.GetService(typeof(IActorFactory))!,
                (IGameFactory)sp.GetService(typeof(IGameFactory))!,
                (IGameRepository)sp.GetService(typeof(IGameRepository))!,
                (IPropSpawnStrategy)sp.GetService(typeof(IPropSpawnStrategy))!,
                (IAgentSpawnStrategy)sp.GetService(typeof(IAgentSpawnStrategy))!,
                (IEffectSink)sp.GetService(typeof(IEffectSink))!
            )
        );
        services.Register<ICommandHandler<InitGameCommand>>(sp =>
            (InitGameCommandHandler)sp.GetService(typeof(InitGameCommandHandler))!
        );
    }
}