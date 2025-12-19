using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Persistence;
using TurnForge.Engine.Repositories;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Tests.helpers;
using TurnForge.Engine.Tests.Helpers;
using TurnForge.Engine.Tests.Strategies;

namespace TurnForge.Engine.Tests.Bootstrap;

internal static class EngineTestBootstrapper
{
    public static (TurnForge.Engine.Core.TurnForge Engine, IGameRepository Repository) Boot()
    {

        // 2️⃣ Repository
        // 2️⃣ Repository
        var repository = new InMemoryGameRepository();
        var actorFactory = new TestActionFactory();

        // Mock registries
        // Mock registries
        var propDefs = new TestDefinitionRegistry<TurnForge.Engine.Entities.Actors.Definitions.PropTypeId, TurnForge.Engine.Entities.Actors.Definitions.PropDefinition>();
        var agentDefs = new TestDefinitionRegistry<TurnForge.Engine.Entities.Actors.Definitions.AgentTypeId, TurnForge.Engine.Entities.Actors.Definitions.AgentDefinition>();

        // Register test definitions
        agentDefs.Register(new TurnForge.Engine.Entities.Actors.Definitions.AgentTypeId("Survivor1"), new TurnForge.Engine.Entities.Actors.Definitions.AgentDefinition(new TurnForge.Engine.Entities.Actors.Definitions.AgentTypeId("Survivor1"), 100, 3, 2, new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()));
        agentDefs.Register(new TurnForge.Engine.Entities.Actors.Definitions.AgentTypeId("Survivor2"), new TurnForge.Engine.Entities.Actors.Definitions.AgentDefinition(new TurnForge.Engine.Entities.Actors.Definitions.AgentTypeId("Survivor2"), 100, 3, 2, new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()));

        var gameContext = new GameEngineContext(
            repository,
            new TestPropSpawnStrategy(),
            new TestAgentSpawnStrategy());
        var engine = GameEngineFactory.Build(gameContext);
        return (engine, repository);
    }
}