using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Apis;
using BarelyAlive.Rules.Apis.Interfaces;


using TurnForge.Engine.Core;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Services.Interfaces;

namespace BarelyAlive.Rules.Game;

/// <summary>
/// Contenedor principal del juego Barely Alive. es el root del juego y orquesta la interacción entre sus componentes.
/// </summary>
public sealed class BarelyAliveGame
{
    private readonly TurnForge.Engine.Core.TurnForge _turnForge = GameBootstrap.GameEngineBootstrap();

    public IGameCatalogApi GameCatalog => _turnForge.GameCatalog;

    public IBarelyAliveApis BarelyAliveApis { get; }

    private BarelyAliveGame()
    {
        BarelyAliveApis = new BarelyAliveApis(_turnForge.Runtime);

    }

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

        foreach (var agent in config.Agents)
        {
            RegisterAgent(agent);
        }

        foreach (var prop in config.Props)
        {
            RegisterProp(prop);
        }
    }

    private void RegisterAgent(AgentDto agent)
    {
        var agentDef = new AgentDefinition(
            new AgentTypeId(agent.TypeId),
            new TurnForge.Engine.Entities.Components.Definitions.PositionComponentDefinition(TurnForge.Engine.ValueObjects.Position.Empty),
            new TurnForge.Engine.Entities.Components.Definitions.HealhtComponentDefinition(agent.MaxHealth),
            new TurnForge.Engine.Entities.Components.Definitions.MovementComponentDefinition(agent.MaxBaseMovement),
            []
        );
        _turnForge.GameCatalog.RegisterAgentDefinition(agentDef.TypeId, agentDef);
    }



    private void RegisterProp(PropDto prop)
    {
        var propDef = new PropDefinition(
            new PropTypeId(prop.TypeId),
            new TurnForge.Engine.Entities.Components.Definitions.PositionComponentDefinition(TurnForge.Engine.ValueObjects.Position.Empty),
             new TurnForge.Engine.Entities.Components.Definitions.HealhtComponentDefinition(prop.MaxHealth),
            []
        );
        _turnForge.GameCatalog.RegisterPropDefinition(propDef.TypeId, propDef);
    }

    private class BarelyAliveConfig
    {
        public List<AgentDto> Agents { get; set; } = new();
        public List<PropDto> Props { get; set; } = new();
    }
}

