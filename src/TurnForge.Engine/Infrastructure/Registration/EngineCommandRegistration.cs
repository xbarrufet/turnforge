// LEGACY FILE - Temporarily disabled during workflow refactor
// TODO: Reimplement with new workflow-based command system

/*
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;

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
        // TODO: Reimplement board and spawn commands using new workflow architecture
    }
}
*/

namespace TurnForge.Engine.Registration;

using TurnForge.Engine.Infrastructure;

/// <summary>
/// Placeholder for engine command registration.
/// Legacy command handlers have been removed during workflow refactor.
/// </summary>
public static class EngineCommandRegistration
{
    public static void Register(SimpleServiceProvider services)
    {
        // No-op: Command handlers will be reimplemented using workflow architecture
    }
}