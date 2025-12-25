using BarelyAlive.Rules.Adapter.Loaders;
using BarelyAlive.Rules.Tests.Infrastructure;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.Commands.ACK;
using TurnForge.Engine.Commands.Board;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Events;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board.Descriptors;

namespace BarelyAlive.Rules.Tests.Helpers;

/// <summary>
/// Fluent API for creating and executing E2E test scenarios.
/// Uses Given/When/Then pattern for readability and maintainability.
/// </summary>
public class ScenarioRunner
{
    private readonly TestBootstrap _bootstrap;
    private readonly List<IGameEvent> _capturedEvents;
    
    private ScenarioRunner()
    {
        _bootstrap = TestBootstrap.CreateNewGame();
        _capturedEvents = new List<IGameEvent>();
    }

    /// <summary>
    /// Creates a new scenario runner instance.
    /// </summary>
    /// <returns>A new scenario runner</returns>
    public static ScenarioRunner Create()
    {
        return new ScenarioRunner();
    }

    /// <summary>
    /// Initializes the game board from a mission JSON string.
    /// </summary>
    /// <param name="missionJson">Mission JSON configuration</param>
    /// <returns>This scenario runner for chaining</returns>
    public ScenarioRunner GivenMission(string missionJson)
    {
        var (spatial, zones, props, _) = MissionLoader.ParseMissionString(missionJson);
        var boardDesc = new BoardDescriptor(spatial, zones);

        // Initialize board
        var boardResult = _bootstrap.Engine.Runtime.ExecuteCommand(new InitializeBoardCommand(boardDesc));
        if (!boardResult.Result.Success)
        {
            throw new InvalidOperationException($"Failed to initialize board: {boardResult.Result.Error}");
        }
        
        CaptureEvents(boardResult.Events);
        ExecuteAck();

        // Spawn props
        var propsResult = _bootstrap.Engine.Runtime.ExecuteCommand(new SpawnPropsCommand(props));
        if (!propsResult.Result.Success)
        {
            throw new InvalidOperationException($"Failed to spawn props: {propsResult.Result.Error}");
        }
        
        CaptureEvents(propsResult.Events);
        ExecuteAck();

        return this;
    }

    /// <summary>
    /// Spawns survivors at their designated spawn points.
    /// </summary>
    /// <param name="survivorIds">Survivor type IDs to spawn</param>
    /// <returns>This scenario runner for chaining</returns>
    public ScenarioRunner GivenSurvivors(params string[] survivorIds)
    {
        var requests = survivorIds.Select(id => new SpawnRequest(id)).ToList();
        return GivenAgents(requests.ToArray());
    }

    /// <summary>
    /// Spawns agents with custom spawn requests.
    /// </summary>
    /// <param name="requests">Spawn requests for agents</param>
    /// <returns>This scenario runner for chaining</returns>
    public ScenarioRunner GivenAgents(params SpawnRequest[] requests)
    {
        var result = _bootstrap.Engine.Runtime.ExecuteCommand(new SpawnAgentsCommand(requests.ToList()));
        if (!result.Result.Success)
        {
            throw new InvalidOperationException($"Failed to spawn agents: {result.Result.Error}");
        }
        
        CaptureEvents(result.Events);
        ExecuteAck();

        return this;
    }

    /// <summary>
    /// Executes a command built using the command builder.
    /// </summary>
    /// <param name="commandAction">Action that creates the command using CommandBuilder</param>
    /// <returns>This scenario runner for chaining</returns>
    public ScenarioRunner When(Func<CommandBuilder, IActionCommand> commandAction)
    {
        var command = commandAction(new CommandBuilder());
        return WhenCommand(command);
    }

    /// <summary>
    /// Executes a pre-built command.
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <returns>This scenario runner for chaining</returns>
    public ScenarioRunner WhenCommand(IActionCommand command)
    {
        _capturedEvents.Clear(); // Reset for new command
        
        var result = _bootstrap.Engine.Runtime.ExecuteCommand(command);
        if (!result.Result.Success)
        {
            throw new InvalidOperationException($"Command failed: {result.Result.Error}");
        }
        
        CaptureEvents(result.Events);
        ExecuteAck();

        return this;
    }

    /// <summary>
    /// Validates the current game state using an assertion action.
    /// </summary>
    /// <param name="assertion">Action that validates the game state</param>
    /// <returns>This scenario runner for chaining</returns>
    public ScenarioRunner Then(Action<GameState> assertion)
    {
        var state = _bootstrap.GameRepository.LoadGameState();
        assertion(state);
        return this;
    }

    /// <summary>
    /// Validates the effects captured from the last command execution.
    /// </summary>
    /// <param name="assertion">Action that validates the effects</param>
    /// <returns>This scenario runner for chaining</returns>
    public ScenarioRunner ThenEvents(Action<IReadOnlyList<IGameEvent>> assertion)
    {
        assertion(_capturedEvents.AsReadOnly());
        return this;
    }

    /// <summary>
    /// Gets the current game state for custom validation.
    /// </summary>
    /// <returns>The current game state</returns>
    public GameState GetCurrentState()
    {
        return _bootstrap.GameRepository.LoadGameState();
    }

    /// <summary>
    /// Gets the effects captured from the last command execution.
    /// </summary>
    /// <returns>Read-only list of captured effects</returns>
    public IReadOnlyList<IGameEvent> GetCapturedEvents()
    {
        return _capturedEvents.AsReadOnly();
    }

    private void ExecuteAck()
    {
        _bootstrap.Engine.Runtime.ExecuteCommand(new ACKCommand());
    }

    private void CaptureEvents(IReadOnlyList<IGameEvent> events)
    {
        foreach (var gameEvent in events)
        {
            _capturedEvents.Add(gameEvent);
        }
    }
}
