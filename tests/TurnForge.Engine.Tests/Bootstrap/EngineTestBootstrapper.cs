using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Persistence;
using TurnForge.Engine.Repositories;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Tests.Helpers;
using TurnForge.Engine.Components;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Bootstrap;

internal static class EngineTestBootstrapper
{
    public static (TurnForge.Engine.Core.TurnForge Engine, IGameRepository Repository) Boot()
    {
        // 2️⃣ Repository
        var repository = new InMemoryGameRepository();
       

        // Local variables were unused and relying on deleted types.
        // If repository setup is needed, it goes here.

        /*var gameContext = new GameEngineContext(
            repository,
            new TestPropSpawnStrategy(),
            new TestAgentSpawnStrategy());
        var engine = GameEngineFactory.Build(gameContext);*/
        //return (null, repository);
        return (null,null);
    }
}