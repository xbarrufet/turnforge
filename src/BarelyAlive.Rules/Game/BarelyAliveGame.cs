using TurnForge.Engine.Core;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Services.Interfaces;
using BarelyAlive.Rules.Definitions;
using System.IO;
using System.Text.Json;
using BarelyAlive.Rules.Adapter.Dto;

namespace BarelyAlive.Rules.Game;

/// <summary>
/// Contenedor principal del juego Barely Alive. es el root del juego y orquesta la interacción entre sus componentes.
/// </summary>
public sealed class BarelyAliveGame
{
    private readonly TurnForge.Engine.Core.TurnForge _turnForge = GameBootstrap.GameEngineBootstrap();
    
    public IGameCatalogApi GameCatalog => _turnForge.GameCatalog;
    
    
    public static BarelyAliveGame CreateNewGame()
    {
        BarelyAliveGame game = new BarelyAliveGame();
        game.RegisterGameDefinitions();
        return game;
    }
    
    
    private void RegisterGameDefinitions()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Units_BarelyAlive.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Could not find configuration file at {filePath}");
        }

        var json = File.ReadAllText(filePath);
        var config = JsonSerializer.Deserialize<BarelyAliveConfig>(json);
        
        if (config == null) return;

        foreach (var survivor in config.Survivors)
        {
            RegisterSurvivor(survivor);
        }

        foreach (var zombie in config.Zombies)
        {
            RegisterZombie(zombie);
        }

        foreach (var prop in config.Props)
        {
            RegisterProp(prop);
        }
    }

    private void RegisterZombie(ZombieDto zombie)
    {
        var npcDef = new NpcDefinition(
            new NpcTypeId(zombie.TypeId),
            zombie.MaxHealth,
            zombie.MaxBaseMovement,
            zombie.MaxActionPoints,
            []
        );
        _turnForge.GameCatalog.RegiterNpcDefinition(npcDef.TypeId, npcDef);
    }

    private void RegisterSurvivor(SurvivorDto survivor)
    {
        var defModel = new SurvivorDefinition(survivor.TypeId);
        var unitDef = new UnitDefinition(
            new UnitTypeId(survivor.TypeId),
            survivor.MaxHealth,
            survivor.MaxBaseMovement,
            survivor.MaxActionPoints,
            []
        );
        _turnForge.GameCatalog.RegisterUnitDefinition(unitDef.TypeId, unitDef);
    }

    private void RegisterProp(PropDto prop)
    {
        var propDef = new PropDefinition(
            new PropTypeId(prop.TypeId),
            prop.MaxBaseMovement,
            prop.MaxActionPoints,
            [],
            prop.MaxHealth
        );
        _turnForge.GameCatalog.RegisterPropDefinition(propDef.TypeId, propDef);
    }

    private class BarelyAliveConfig
    {
        public List<BarelyAlive.Rules.Adapter.Dto.SurvivorDto> Survivors { get; set; } = new();
        public List<BarelyAlive.Rules.Adapter.Dto.ZombieDto> Zombies { get; set; } = new();
        public List<BarelyAlive.Rules.Adapter.Dto.PropDto> Props { get; set; } = new();
    }
}

