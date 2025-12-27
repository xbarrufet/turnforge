using System.Text.Json;
using System.Text.Json.Serialization;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.Move;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Tests.Helpers;

/// <summary>
/// Serializes and deserializes test scenarios to/from JSON.
/// Enables reusable, version-controlled test cases.
/// </summary>
public static class ScenarioSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Loads a scenario from a JSON file.
    /// </summary>
    /// <param name="path">Path to the JSON file</param>
    /// <returns>Deserialized scenario definition</returns>
    public static ScenarioDefinition LoadScenario(string path)
    {
        var json = File.ReadAllText(path);
        var scenario = JsonSerializer.Deserialize<ScenarioDefinition>(json, JsonOptions);
        
        if (scenario == null)
        {
            throw new InvalidOperationException($"Failed to deserialize scenario from {path}");
        }

        return scenario;
    }

    /// <summary>
    /// Saves a scenario to a JSON file.
    /// </summary>
    /// <param name="scenario">The scenario to save</param>
    /// <param name="path">Path to save the JSON file</param>
    public static void SaveScenario(ScenarioDefinition scenario, string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(scenario, JsonOptions);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Executes a scenario definition using ScenarioRunner.
    /// </summary>
    /// <param name="scenario">The scenario to execute</param>
    /// <returns>The final game state after execution</returns>
    public static GameState ExecuteScenario(ScenarioDefinition scenario)
    {
        var runner = ScenarioRunner.Create();

        // Setup
        if (!string.IsNullOrEmpty(scenario.Setup.MissionJson))
        {
            runner.GivenMission(scenario.Setup.MissionJson);
        }

        if (scenario.Setup.Survivors?.Length > 0)
        {
            runner.GivenSurvivors(scenario.Setup.Survivors);
        }

        // Execute commands
        foreach (var cmdDef in scenario.Commands)
        {
            var command = CreateCommand(cmdDef);
            runner.WhenCommand(command);
        }

        // Validate assertions
        var state = runner.GetCurrentState();
        foreach (var assertion in scenario.Assertions)
        {
            ValidateAssertion(state, assertion);
        }

        return state;
    }

    private static IActionCommand CreateCommand(CommandDefinition cmdDef)
    {
        return cmdDef.Type switch
        {
            CommandType.Move => CreateMoveCommand(cmdDef),
            _ => throw new NotSupportedException($"Command type {cmdDef.Type} is not supported")
        };
    }

    private static MoveCommand CreateMoveCommand(CommandDefinition cmdDef)
    {
        if (string.IsNullOrEmpty(cmdDef.AgentId))
        {
            throw new InvalidOperationException("Move command requires AgentId");
        }

        if (string.IsNullOrEmpty(cmdDef.TargetTileId))
        {
            throw new InvalidOperationException("Move command requires TargetTileId");
        }

        var tileId = new TileId(Guid.Parse(cmdDef.TargetTileId));
        var position = new Position(tileId);
        return new MoveCommand(cmdDef.AgentId, hasCost: true, targetPosition: position);
    }



    private static void ValidateAssertion(GameState state, AssertionDefinition assertion)
    {
        switch (assertion.Type)
        {
            case AssertionType.AgentPosition:
                ValidateAgentPosition(state, assertion);
                break;
            case AssertionType.AgentCount:
                ValidateAgentCount(state, assertion);
                break;
            default:
                throw new NotSupportedException($"Assertion type {assertion.Type} is not supported");
        }
    }

    private static void ValidateAgentPosition(GameState state, AssertionDefinition assertion)
    {
        if (string.IsNullOrEmpty(assertion.AgentId))
        {
            throw new InvalidOperationException("AgentPosition assertion requires AgentId");
        }

        if (string.IsNullOrEmpty(assertion.ExpectedTileId))
        {
            throw new InvalidOperationException("AgentPosition assertion requires ExpectedTileId");
        }

        var agent = state.GetAgents().FirstOrDefault(a => a.DefinitionId == assertion.AgentId);
        if (agent == null)
        {
            throw new InvalidOperationException($"Agent {assertion.AgentId} not found in game state");
        }

        var expectedPosition = new Position(new TileId(Guid.Parse(assertion.ExpectedTileId)));
        if (agent.PositionComponent.CurrentPosition != expectedPosition)
        {
            throw new InvalidOperationException(
                $"Agent {assertion.AgentId} position mismatch. " +
                $"Expected: {expectedPosition}, Actual: {agent.PositionComponent.CurrentPosition}");
        }
    }

    private static void ValidateAgentCount(GameState state, AssertionDefinition assertion)
    {
        if (!assertion.ExpectedCount.HasValue)
        {
            throw new InvalidOperationException("AgentCount assertion requires ExpectedCount");
        }

        var actualCount = state.GetAgents().Count;
        if (actualCount != assertion.ExpectedCount.Value)
        {
            throw new InvalidOperationException(
                $"Agent count mismatch. Expected: {assertion.ExpectedCount.Value}, Actual: {actualCount}");
        }
    }
}

/// <summary>
/// Defines a complete test scenario including setup, commands, and assertions.
/// </summary>
public record ScenarioDefinition
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ScenarioSetup Setup { get; init; } = new();
    public List<CommandDefinition> Commands { get; init; } = new();
    public List<AssertionDefinition> Assertions { get; init; } = new();
}

/// <summary>
/// Defines the initial setup for a scenario.
/// </summary>
public record ScenarioSetup
{
    public string MissionJson { get; init; } = string.Empty;
    public string[] Survivors { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Defines a command to execute in a scenario.
/// </summary>
public record CommandDefinition
{
    public CommandType Type { get; init; }
    public string? AgentId { get; init; }
    public string? TargetTileId { get; init; }
}

/// <summary>
/// Defines an assertion to validate in a scenario.
/// </summary>
public record AssertionDefinition
{
    public AssertionType Type { get; init; }
    public string? AgentId { get; init; }
    public string? ExpectedTileId { get; init; }
    public int? ExpectedCount { get; init; }
}

/// <summary>
/// Command types supported by scenario serialization.
/// </summary>
public enum CommandType
{
    Move,
    SpawnAgent,
    Attack
}

/// <summary>
/// Assertion types supported by scenario serialization.
/// </summary>
public enum AssertionType
{
    AgentPosition,
    AgentCount,
    EffectGenerated
}
