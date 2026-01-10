using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm;

/// <summary>
/// FSM Graph that holds nodes and manages transitions (FSM 2.0).
/// 
/// Execution flow:
/// 1. ProcessFlow() runs OnEntry workflows and auto-transitions
/// 2. Check IsCompleted() on current node
/// 3. If completed, transition to GetNextNode()
/// </summary>
public class FsmGraph
{
    private readonly Dictionary<NodeId, BaseFsmNode> _nodes = new();
    private readonly IServiceProvider? _services;
    private readonly IGameLogger? _logger;

    public BaseFsmNode? CurrentNode { get; private set; }
    public BaseFsmNode? RootNode { get; private set; }

    /// <summary>
    /// Constructor accepting IFsmNode for backward compatibility with GameEngineFactory.
    /// </summary>
    public FsmGraph(IFsmNode? rootNode, IServiceProvider? services, IGameLogger? logger)
    {
        _services = services;
        _logger = logger;

        if (rootNode is BaseFsmNode baseNode)
        {
            // Creates the graph having the GameStartNode as root
            StartGameNode startNode = new StartGameNode(rootNode as BaseFsmNode);
            SetRoot(startNode);
        }
    }

    /// <summary>
    /// Simple constructor for tests.
    /// </summary>
    public FsmGraph() : this(null, null, null) { }

    /// <summary>
    /// Register a node in the graph.
    /// </summary>
    public void AddNode(BaseFsmNode node)
    {
        _nodes[node.Id] = node;
    }

    /// <summary>
    /// Set the root node (entry point).
    /// </summary>
    public void SetRoot(BaseFsmNode node)
    {
        AddNode(node);
        RootNode = node;
        CurrentNode = node;
    }

    /// <summary>
    /// Retrieve a node by ID. Useful for graph introspection.
    /// </summary>
    public BaseFsmNode? GetNode(NodeId id)
    {
        return _nodes.TryGetValue(id, out var node) ? node : null;
    }

    /// <summary>
    /// Reset the FSM to the initial state (RootNode).
    /// </summary>
    public void Reset()
    {
        if (RootNode != null)
        {
            CurrentNode = RootNode;
        }
    }

    /// <summary>
    /// Process FSM flow: execute OnEntry workflows and auto-transitions.
    /// This is the main entry point called by GameEngineRuntime.
    /// </summary>
    public FsmFlowResult ProcessFlow(GameState state)
    {
        if (CurrentNode == null)
        {
            return FsmFlowResult.NoChange(state);
        }

        var currentState = state;
        var events = new List<IGameEvent>();
        var isGameOver = false;
        var hasError = false;
        var error = string.Empty;

        // Execute OnEntry system workflows for current node
        // JA ES FA AL BUCLE currentState = ExecuteOnEntryActions(CurrentNode, currentState, events);
        int MAX_STEPS_SAFETY_LIMIT = 5000;
        // Check for auto-transitions
        // Create a view for node queries (nodes should not access GameState directly)
        while (CurrentNode != null && CurrentNode.IsCompleted(new GameStateView(currentState)) && MAX_STEPS_SAFETY_LIMIT > 0)
        {
            MAX_STEPS_SAFETY_LIMIT--;
            var nextNode = CurrentNode.GetNextNode(new GameStateView(currentState));

            if (nextNode == null)
            {
                // No next node = end of game
                isGameOver = true;
                _logger?.LogInfo($"FSM: No next node from {CurrentNode.Name}, game over.");
                break;
            }

            _logger?.LogInfo($"FSM: Transition {CurrentNode.Name} → {nextNode.Name}");
            CurrentNode = nextNode;

            // Execute OnEntry for new node
            currentState = ExecuteOnEntryActions(CurrentNode, currentState, events);
        }
        if (MAX_STEPS_SAFETY_LIMIT <= 0)
        {
            _logger?.LogError($"FSM: Maximum steps safety limit reached. Possible infinite loop.");
            isGameOver = true;
            hasError = true;
            error = "Maximum steps safety limit reached. Possible infinite loop.";
        }
        return new FsmFlowResult(currentState, events.AsReadOnly(), isGameOver, hasError, error);
    }

    private GameState ExecuteOnEntryActions(BaseFsmNode node, GameState state, List<IGameEvent> events)
    {
        var currentState = state;

        foreach (var action in node.OnEntryActions)
        {
            // Create a view for the current state
            var gameStateView = new GameStateView(currentState);

            // Execute action nodes directly (system actions don't suspend)
            var currentNode = action.StartNode;
            while (currentNode != null)
            {
                var result = currentNode.Execute(action.Context, gameStateView);

                if (result.Status == ActionStatus.Failed)
                {
                    _logger?.LogError($"FSM OnEntry action failed: {result.ErrorMessage}");
                    break;
                }

                // Move to next node
                currentNode = currentNode.NextNode;
            }

            // Commit overlay changes to get new state
            currentState = currentState.CommitOverlayChanges();
        }

        return currentState;
    }

    /// <summary>
    /// Check if a command is allowed in current node.
    /// </summary>
    public bool IsActionAllowed(ActionId actionId)
    {
        return CurrentNode?.IsActionAllowed(actionId) ?? false;
    }
}
