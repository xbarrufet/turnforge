using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Apis;
using BarelyAlive.Rules.Apis.Interfaces;
using TurnForge.Engine.APIs.Interfaces;
using TurnForge.Engine.Core;

using TurnForge.Engine.Registration;
using BarelyAlive.Rules.Core.Domain.Definitions;
using TurnForge.Engine.Definitions;
using BarelyAlive.Rules.Core.Domain.Entities;
using TurnForge.Engine.Repositories.Interfaces;
using SurvivorDefinition = BarelyAlive.Rules.Core.Domain.Entities.SurvivorDefinition;


namespace BarelyAlive.Rules.Game;

/// <summary>
/// Contenedor principal del juego Barely Alive. es el root del juego y orquesta la interacción entre sus componentes.
/// </summary>
public sealed class BarelyAliveGame
{


private static string mike="3f8a1d4e-9b7c-4e2a-8b1d-6c7a5f2e9d41";
private static string doug="b2c9e6a4-1f73-4d5a-9c8e-0a7f6b5d4c21";
private static string zrunner="7a4e9c1d-2b6f-4e8a-9d3c-5f1b0a2e6c47";
private static string zfat="c5d7b1a9-3e6f-4c2d-8a9b-1f0e6d4c5a72";
private static string znormal="1e6a9f2b-4d3c-8a5f-7b9c-0d1e2c4a6f58";
private static string spawZombie="9c4e6f2a-1b7d-5a3c-8f0e-2d6b9a1c4e35";
private static string spawnPlayer="4a6d2c9e-7f1b-5e8a-3c0d-9b6f1a2e4c78";
private static string porta="988c5977-d33f-4bdc-a775-53caefcab413";
private static string zona="6afac418-e205-4125-839a-48452ec273e2";


    private readonly TurnForge.Engine.Core.TurnForge _turnForge;
    private readonly TurnForge.Engine.Core.Interfaces.IGameLogger _logger;

    public IGameCatalogApi GameCatalog => _turnForge.GameCatalog;
    public IGameRepository GameRepository { get; }

    public IBarelyAliveApis BarelyAliveApis { get; }

    private BarelyAliveGame(TurnForge.Engine.Core.Interfaces.IGameLogger? logger)
    {
        _logger = logger ?? new TurnForge.Engine.Infrastructure.ConsoleLogger();
        GameRepository = new BarelyAlive.Rules.Adapter.Repositories.InMemoryGameRepository();
        _turnForge = GameBootstrap.GameEngineBootstrap(logger, GameRepository);
        
        // Initialize FSM
        var fsmController = BarelyAliveGameFlow.CreateController();
        _turnForge.Runtime.SetFsmController(fsmController);

        BarelyAliveApis = new BarelyAliveApis(_turnForge.Runtime, _turnForge.GameCatalog, GameRepository);

    }

    public static BarelyAliveGame CreateNewGame(TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null)
    {
        BarelyAliveGame game = new BarelyAliveGame(logger);
        game.RegisterGameDefinitions();
        return game;
    }

    // FOR TESTING THE PROCESS OF CREATING NEW ASSETS
    private void RegisterGameDefinitions() {
        // Survivors
        var mikeDef = new SurvivorDefinition(mike);
        mikeDef.AddTrait(new TurnForge.Engine.Traits.Standard.IdentityTrait("Survivor"));
        _turnForge.GameCatalog.RegisterDefinition(mikeDef);

        var dougDef = new SurvivorDefinition(doug);
        dougDef.AddTrait(new TurnForge.Engine.Traits.Standard.IdentityTrait("Survivor"));
        _turnForge.GameCatalog.RegisterDefinition(dougDef);

        // Zombies
        var zRunnerDef = new TurnForge.Engine.Definitions.BaseGameEntityDefinition(zrunner, "Zombie");
        _turnForge.GameCatalog.RegisterDefinition(zRunnerDef);
        
        var zFatDef = new TurnForge.Engine.Definitions.BaseGameEntityDefinition(zfat, "Zombie");
        _turnForge.GameCatalog.RegisterDefinition(zFatDef);
        
        var zNormalDef = new TurnForge.Engine.Definitions.BaseGameEntityDefinition(znormal, "Zombie");
        _turnForge.GameCatalog.RegisterDefinition(zNormalDef);

        // Spawn
        var spawnPlayerDef = new TurnForge.Engine.Definitions.BaseGameEntityDefinition(spawnPlayer, "Spawn");
        _turnForge.GameCatalog.RegisterDefinition(spawnPlayerDef);

        var zombiSpawn = new ZombieSpawnDefinition(spawZombie) { Order = 1 };
        _turnForge.GameCatalog.RegisterDefinition(zombiSpawn);

        // Zones
        var zonaDef = new TurnForge.Engine.Definitions.BaseGameEntityDefinition(zona, "Board");
        _turnForge.GameCatalog.RegisterDefinition(zonaDef);

        // Doors
        var door = new DoorDefinition(porta);
        _turnForge.GameCatalog.RegisterDefinition(door);
    } 



   
}

