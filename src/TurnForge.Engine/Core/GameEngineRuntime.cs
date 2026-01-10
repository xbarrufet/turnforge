using TurnForge.Engine.Commands;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core;

/// <summary>
/// Main game engine runtime.
/// Coordinates action execution, FSM transitions, and state management.
/// </summary>
public sealed class GameEngineRuntime : IGameEngine
{
    private readonly IGameRepository _repository;
    private readonly IGameLogger _logger;
    private readonly IActionFactory _actionFactory;
    private readonly IActionOrchestrator _actionOrchestrator;
    private readonly IOrchestrator _orchestrator;
    
    private FsmGraph? _fsmGraph;
    private GameStatus _gameStatus = GameStatus.WaitingForStart;

    private IAction? _activeAction;
    
    public GameEngineRuntime(
        IGameRepository repository, 
        IOrchestrator orchestrator, 
        IActionOrchestrator actionOrchestrator, 
        IActionFactory actionFactory,
        IGameLogger logger) 
    {
        _repository = repository;
        _orchestrator = orchestrator;
        _actionOrchestrator = actionOrchestrator;
        _actionFactory = actionFactory;
        _logger = logger;
    }

    public void SetFsmGraph(FsmGraph graph)
    {
        _fsmGraph = graph;
    }

    /// <summary>
    /// Execute an action by its registered ID.
    /// This is the primary API for external systems (UI) to trigger game actions.
    /// </summary>
    public ActionTransaction ExecuteAction(ActionId actionId, Dictionary<string, object>? parameters = null)
    {
        try
        {
            // Pre-validations
            var preValidationResult = ValidatePreConditions(actionId);
            if (!preValidationResult.IsValid)
                return preValidationResult.Transaction;

            // Action discovery & creation
            var action = DiscoverAndBuildAction(actionId);
            if (action == null)
                return ActionTransaction.Fail(actionId, $"Action '{actionId.Value}' not registered.");

            // Parameter injection
            var state = _repository.LoadGameState();
            InjectActionParameters(action, parameters);

            // Execute action and persist new state
            var (executionResult, newState) = ExecuteAndCommitAction(action, state, actionId);
            _repository.SaveGameState(newState);
            if (executionResult != null)
                return executionResult;

            // FSM processing (only if action completed)
            return ProcessFsmFlow(newState, actionId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error executing action {actionId.Value}", ex);
            return ActionTransaction.Fail(actionId, ex.Message);
        }
    }

    /// <summary>
    /// Validates game status and FSM state before action execution.
    /// </summary>
    /// <returns>Validation result with transaction if validation fails.</returns>
    private (bool IsValid, ActionTransaction Transaction) ValidatePreConditions(ActionId actionId)
    {
        // Check if game is over
        if (_gameStatus == GameStatus.GameOver)
        {
            return (false, ActionTransaction.Fail(actionId, "Game is over. Call ResetGame() to start a new game."));
        }
        
        // Check action is allowed in current FSM state
        if (_fsmGraph != null && !_fsmGraph.CurrentNode!.IsActionAllowed(actionId))
        {
            return (false, ActionTransaction.Fail(actionId, 
                $"Action '{actionId.Value}' not allowed in current state '{_fsmGraph.CurrentNode.Name}'."));
        }

        // Update status if starting
        if (_gameStatus == GameStatus.WaitingForStart)
        {
            _gameStatus = GameStatus.InProgress;
        }

        return (true, ActionTransaction.Success(actionId)); // Transaction not used when valid
    }

    /// <summary>
    /// Discovers and builds the action from the factory.
    /// </summary>
    /// <returns>The built action, or null if not found in registry.</returns>
    private IAction? DiscoverAndBuildAction(ActionId actionId)
    {
        if (!_actionFactory.GetRegisteredActionIds().Contains(actionId))
        {
            return null;
        }
        
        return _actionFactory.BuildAction(actionId);
    }

    /// <summary>
    /// Injects parameters into the action context.
    /// Handles both BatchInputs and generic parameters.
    /// </summary>
    private void InjectActionParameters(IAction action, Dictionary<string, object>? parameters)
    {
        if (parameters == null)
            return;

        // Handle BatchInputs specifically
        if (parameters.TryGetValue("BatchInputs", out var batchObj) && batchObj is IEnumerable<IActionInput> batchInputs)
        {
            foreach (var input in batchInputs)
            {
                action.Context.EnqueueInput(input);
            }
        }
        
        // Set all parameters into context data
        foreach (var kvp in parameters)
        {
            action.Context.Set(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Executes the action and commits changes if completed.
    /// </summary>
    /// <returns>Transaction if action is suspended/failed, null if completed successfully.</returns>
    private (ActionTransaction?,GameState) ExecuteAndCommitAction(IAction action, GameState state, ActionId actionId)
    {
        var gameStateView = new GameStateView(state);
        var actionResult = _actionOrchestrator.StartAction(action, gameStateView);
        
        if (actionResult == ActionStatus.Completed)
        {
            // Commit overlay changes from the state
            var newState = state.CommitOverlayChanges();
      
            
            // Return null to indicate we should continue to FSM processing
            return (null, newState);
        }
        else if (actionResult == ActionStatus.Suspended)
        {
            _activeAction = action;
            return (ActionTransaction.Suspended(actionId), state);
        }
        else
        {
            var msg = action.Context.ErrorMessage ?? "Action failed or was cancelled.";
            return (ActionTransaction.Fail(actionId, msg), state);
        }
    }

    /// <summary>
    /// Processes FSM flow after a completed action.
    /// Checks for game over conditions.
    /// </summary>
    private ActionTransaction ProcessFsmFlow(GameState state, ActionId actionId)
    {
        if (_fsmGraph != null)
        {
            var fsmResult = _fsmGraph.ProcessFlow(state);
            if(fsmResult.HasError)
            {
                string? error = fsmResult.Error;
                return ActionTransaction.Fail(actionId, error ?? "FSM Error");
            }
            if (fsmResult.IsGameOver)
            {
                _gameStatus = GameStatus.GameOver;
                return new ActionTransaction(actionId) 
                { 
                    Status = ActionStatus.Completed, 
                    IsGameOver = true 
                };
            }
        }
        
        return ActionTransaction.Success(actionId);
    }

    /// <summary>
    /// Get current game status.
    /// </summary>
    public GameStatus GetStatus() => _gameStatus;

    /// <summary>
    /// Reset the game. Clears state, resets FSM to root, and returns to WaitingForStart.
    /// </summary>
    public void ResetGame()
    {
        _logger.LogInfo("Resetting game engine...");
        
        // 1. Clear Active Actions
        _activeAction = null;
        
        // 2. Reset Status
        _gameStatus = GameStatus.WaitingForStart;
        
        // 3. Reset FSM
        _fsmGraph?.Reset();
        
        // 4. Reset State (Empty)
        var emptyState = Entities.GameState.Empty();
        _repository.SaveGameState(emptyState);
        _orchestrator.SetState(emptyState);
        
        _logger.LogInfo("Game engine reset complete.");
    }
    
    public Entities.GameState CurrentState => _repository.LoadGameState();
}


