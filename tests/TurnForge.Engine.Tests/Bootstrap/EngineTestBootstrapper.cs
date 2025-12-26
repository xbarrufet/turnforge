using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
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
       

        // Mock registries
        var propDefs = new TestDefinitionRegistry<string, TestPropDefinition>();
        var agentDefs = new TestDefinitionRegistry<string, TestAgentDefinition>();

        // Register test definitions
        agentDefs.Register("Survivor1",
                new TestAgentDefinition
                {
                    DefinitionId = "Survivor1",
                    AgentName = "Survivor1",
                    Category = "Test",
                    PositionComponent = new BasePositionComponent(Position.Empty),
                    MaxHealth = 100,
                     MaxMovement = 3,
                     Traits = new List<IActorTrait>()
                 });

        agentDefs.Register("Survivor2",
             new TestAgentDefinition
             {
                 DefinitionId = "Survivor2",
                 AgentName = "Survivor2",
                 Category = "Test",
                 PositionComponent = new BasePositionComponent(Position.Empty),
                 MaxHealth = 100,
                 MaxMovement = 3,
                 Traits = new List<IActorTrait>()
             });

        /*var gameContext = new GameEngineContext(
            repository,
            new TestPropSpawnStrategy(),
            new TestAgentSpawnStrategy());
        var engine = GameEngineFactory.Build(gameContext);*/
        //return (null, repository);
        return (null,null);
    }
}