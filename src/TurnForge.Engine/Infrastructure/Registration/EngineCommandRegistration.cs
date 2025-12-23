using TurnForge.Engine.Commands.Board;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Appliers.Entity;
using TurnForge.Engine.Entities.Board.Interfaces;
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
        // Board Initialization
        // =====================================================
        
        services.Register<InitializeBoardCommandHandler>(sp =>
            new InitializeBoardCommandHandler(
                (IBoardFactory)sp.GetService(typeof(IBoardFactory))!,
                (IGameRepository)sp.GetService(typeof(IGameRepository))!
            )
        );
        services.Register<ICommandHandler<InitializeBoardCommand>>(sp =>
            (InitializeBoardCommandHandler)sp.GetService(typeof(InitializeBoardCommandHandler))!
        );

    }
}