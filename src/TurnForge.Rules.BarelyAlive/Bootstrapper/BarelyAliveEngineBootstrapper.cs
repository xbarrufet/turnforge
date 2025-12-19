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
    public static global::TurnForge.Engine.Core.TurnForge Boot()
    {
        // 1. Build Engine
        global::TurnForge.Engine.Infrastructure.GameEngineContext context = new global::TurnForge.Engine.Infrastructure.GameEngineContext(
            new InMemoryGameRepository(),
            new BAPropSpawnStrategy(),
            new SurvivorSpawnStrategy());
        
        var turnForge = GameEngineFactory.Build(context);

        // 2. Register definitions into the Catalog
        // Note: We need to map the old "BarelyAliveDefinitions" to the expected definitions if possible, 
        // or just register them using the new API.
        // Assuming BarelyAliveDefinitions provides the Definition objects directly.
        
        turnForge.GameCatalog.RegisterUnitDefinition(BarelyAliveDefinitions.Survivor.TypeId, BarelyAliveDefinitions.Survivor);
        turnForge.GameCatalog.RegiterNpcDefinition(BarelyAliveDefinitions.Zombie.TypeId, BarelyAliveDefinitions.Zombie);
        turnForge.GameCatalog.RegisterPropDefinition(BarelyAliveDefinitions.Door.TypeId, BarelyAliveDefinitions.Door);
        
        return turnForge;
    }
}
