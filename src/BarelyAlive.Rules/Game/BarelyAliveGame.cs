using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using BarelyAlive.Rules.Adapter.Dto;
using BarelyAlive.Rules.Apis;
using BarelyAlive.Rules.Apis.Interfaces;
using TurnForge.Engine.APIs.Interfaces;
using TurnForge.Engine.Core;

using TurnForge.Engine.Registration;


namespace BarelyAlive.Rules.Game;

/// <summary>
/// Contenedor principal del juego Barely Alive. es el root del juego y orquesta la interacción entre sus componentes.
/// </summary>
public sealed class BarelyAliveGame
{
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
            // RegisterProp(prop); // Assuming implementation exists or re-adding it
            // Previous replace removed logic for props loop or maybe I missed it.
            // I will re-implement RegisterProp below.
             RegisterProp(prop);
        }
    }

    private void RegisterAgent(AgentDto agent)
    {
        var agentDef = new AgentDefinition
        {
            DefinitionId = agent.AgentName,
            AgentName = agent.AgentName,
            Category = agent.Category,
            MaxHealth = agent.MaxHealth,
            MaxMovement = agent.MaxBaseMovement
        };
        _turnForge.GameCatalog.RegisterAgentDefinition(agentDef.DefinitionId, agentDef);
    }

    private void RegisterProp(PropDto prop)
    {
        var propDef = new PropDefinition
        {
            DefinitionId = prop.TypeId,
            MaxHealth = prop.MaxHealth
        };
        _turnForge.GameCatalog.RegisterPropDefinition(propDef.DefinitionId, propDef);
    }

    private class BarelyAliveConfig
    {
        public List<AgentDto> Agents { get; set; } = new();
        public List<PropDto> Props { get; set; } = new();
    }
}

