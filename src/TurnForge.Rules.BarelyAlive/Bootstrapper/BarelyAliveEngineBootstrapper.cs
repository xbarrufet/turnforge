using TurnForge.Rules.BarelyAlive.Actors;
using TurnForge.Rules.BarelyAlive.Strategies.Spawn;
using TurnForge.Engine.Infrastructure;
using TurnForge.Rules.BarelyAlive.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Rules.BarelyAlive.Bootstrapper;


using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Repositories.InMemory;
using TurnForge.Engine.Repositories.Interfaces;


public static class BarelyAliveEngineBootstrapper
{
    public static GameEngine Boot()
    {
        // 1. Registries
        var propRegistry = new TurnForge.Rules.BarelyAlive.Register.DefinitionRegistry<PropTypeId, PropDefinition>();
        var unitRegistry = new TurnForge.Rules.BarelyAlive.Register.DefinitionRegistry<UnitTypeId, UnitDefinition>();
        var npcRegistry = new TurnForge.Rules.BarelyAlive.Register.DefinitionRegistry<NpcTypeId, NpcDefinition>();

        // 2. Preload definitions
        unitRegistry.Register(BarelyAliveDefinitions.Survivor.TypeId, BarelyAliveDefinitions.Survivor);
        npcRegistry.Register(BarelyAliveDefinitions.Zombie.TypeId, BarelyAliveDefinitions.Zombie);
        propRegistry.Register(BarelyAliveDefinitions.Door.TypeId, BarelyAliveDefinitions.Door);
        // Note: Spawn points like ZombieSpawn are dynamic props based on map data usually, but for bootstrapping we might register defaults if needed.
        // For now, let's just register what we have static.

        GameEngineContext context = new GameEngineContext(
            new InMemoryGameRepository(),
            propRegistry,
            unitRegistry,
            npcRegistry,
            new BAPropSpawnStrategy(),
            new SurvivorSpawnStrategy());
        
        return GameEngineFactory.Build(context);
    }
}
