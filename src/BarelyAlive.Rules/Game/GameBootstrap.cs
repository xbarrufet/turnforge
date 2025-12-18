using BarelyAlive.Rules.Core.Strategies.Spawn;
using BarelyAlive.Rules.Registration;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Repositories.InMemory;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Registration;

namespace BarelyAlive.Rules.Game;

/// <summary>
/// Ensambla el engine de TurnForge con las reglas y estrategias de BarelyAlive.
/// Responsable de la inyección de dependencias específica del juego.
/// </summary>
public sealed class GameBootstrap
{
   

    public static GameEngine GameEngineBootstrap()
    {
        // 1. Crear ServiceProvider
        var services = new SimpleServiceProvider();
            
        InMemoryDefinitionRegistry<PropTypeId,PropDefinition> registryPropDefinitions = new InMemoryDefinitionRegistry<PropTypeId, PropDefinition>();
        InMemoryDefinitionRegistry<UnitTypeId,UnitDefinition> registryUnitDefinitions = new InMemoryDefinitionRegistry<UnitTypeId, UnitDefinition>();
        InMemoryDefinitionRegistry<NpcTypeId,NpcDefinition> registryNpcDefinitions = new InMemoryDefinitionRegistry<NpcTypeId, NpcDefinition>();

        GameEngineContext gameEngineContext = new GameEngineContext(
            new InMemoryGameRepository(),
            registryPropDefinitions,
            registryUnitDefinitions,
            registryNpcDefinitions,
            new BarelyAlivePropSpawnStrategy(),
            new SurvivorUnitsSpawnStrategy());
        return GameEngineFactory.Build(gameEngineContext);

    }
}

