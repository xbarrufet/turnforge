using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Repositories;
using TurnForge.Engine.Repositories.InMemory;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Tests.helpers;

using TurnForge.Engine.Tests.Helpers;

namespace TurnForge.Engine.Tests.Bootstrap;

internal static class EngineTestBootstrapper
{
    public static (GameEngine Engine, IGameRepository Repository) Boot()
    {
        
        // 2️⃣ Repository
        // 2️⃣ Repository
        var repository = new InMemoryGameRepository();
        var actorFactory = new TestActionFactory();
        
        // Mock registries
        var propDefs = new TestDefinitionRegistry<TurnForge.Engine.Entities.Actors.Definitions.PropTypeId, TurnForge.Engine.Entities.Actors.Definitions.PropDefinition>();
        var unitDefs = new TestDefinitionRegistry<TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId, TurnForge.Engine.Entities.Actors.Definitions.UnitDefinition>();
        var npcDefs = new TestDefinitionRegistry<TurnForge.Engine.Entities.Actors.Definitions.NpcTypeId, TurnForge.Engine.Entities.Actors.Definitions.NpcDefinition>();
        
        // Register test definitions
        unitDefs.Register(new TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId("Survivor1"), new TurnForge.Engine.Entities.Actors.Definitions.UnitDefinition(new TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId("Survivor1"), 100, new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()));
        unitDefs.Register(new TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId("Survivor2"), new TurnForge.Engine.Entities.Actors.Definitions.UnitDefinition(new TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId("Survivor2"), 100, new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()));

        var gameContext = new GameEngineContext(
            repository,
            propDefs,
            unitDefs,
            npcDefs,
            new TestPropSpawnStrategy(),
            new TestUnitSpawnStrategy());
        var engine = GameEngineFactory.Build(gameContext);    
        return (engine, repository);
    }
}