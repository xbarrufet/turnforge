using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Repositories;
using TurnForge.Engine.Repositories.InMemory;
using TurnForge.Engine.Repositories.Interfaces;

namespace TurnForge.Engine.Tests.Bootstrap;

internal static class EngineTestBootstrapper
{
    public static (GameEngine Engine, IGameRepository Repository) Boot()
    {
    

        // 2️⃣ Repository
        var repository = new InMemoryGameRepository();
        var actorFactory = new TestActorFactory();
        var engine = GameEngineFactory.Build(repository, actorFactory);    
        return (engine, repository);
    }
}