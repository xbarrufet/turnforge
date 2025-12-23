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
using TurnForge.Engine.Entities;
using BarelyAlive.Rules.Core.Domain.Entities;
using SurvivorDefinition = BarelyAlive.Rules.Core.Domain.Definitions.SurvivorDefinition;


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

    public IBarelyAliveApis BarelyAliveApis { get; }

    private BarelyAliveGame(TurnForge.Engine.Core.Interfaces.IGameLogger? logger)
    {
        _logger = logger ?? new TurnForge.Engine.Infrastructure.ConsoleLogger();
        _turnForge = GameBootstrap.GameEngineBootstrap(logger);
        
        // Initialize FSM
        var fsmController = BarelyAliveGameFlow.CreateController();
        _turnForge.Runtime.SetFsmController(fsmController);

        BarelyAliveApis = new BarelyAliveApis(_turnForge.Runtime, _turnForge.GameCatalog);

    }

    public static BarelyAliveGame CreateNewGame(TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null)
    {
        BarelyAliveGame game = new BarelyAliveGame(logger);
        game.RegisterGameDefinitions();
        return game;
    }

    // FOR TESTING THE PROCESS OF CREATING NEW ASSETS
    private void RegisterGameDefinitions() {
        //resgstrem les definitions
        //_catalog.RegisterDefinition("Survivors.Mike", survivorDef);
        //survivors
        var mikeDef = new SurvivorDefinition(mike, "Mike", 3);
        var dougDef = new SurvivorDefinition(doug, "Doug", 3);
        _turnForge.GameCatalog.RegisterDefinition(mikeDef);
        _turnForge.GameCatalog.RegisterDefinition(dougDef);
        //zombies
        _turnForge.GameCatalog.RegisterDefinition(zrunner,"Zombie Runner", "Zombie");
        _turnForge.GameCatalog.RegisterDefinition(zfat,"Zombie Fat", "Zombie");
        _turnForge.GameCatalog.RegisterDefinition(znormal,"Zombie Normal", "Zombie");
        //spawn
        _turnForge.GameCatalog.RegisterDefinition(spawnPlayer,"Spawn Player", "Spawn");
        var zombiSpawn = new ZombieSpawnDefinition(spawZombie,"Spawn Zombie", 1);
        _turnForge.GameCatalog.RegisterDefinition(zombiSpawn);

        //zones
        _turnForge.GameCatalog.RegisterDefinition(zona,"Tile", "Board");
        //doors
        var door = new DoorDefinition(porta,"Door", "Connections");
        _turnForge.GameCatalog.RegisterDefinition(door);
    } 



   
}

