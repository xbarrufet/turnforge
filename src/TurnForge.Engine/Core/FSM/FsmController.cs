using System;
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.Core.Orchestrator.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm
{
    public class FsmController
    {
        private readonly FlowNavigator _navigator;
        private readonly Dictionary<NodeId, FsmNode> _nodesById;
        private NodeId _currentStateId;
        private bool _waittingForACK;


        public NodeId CurrentStateId => _currentStateId;
        public bool WaittingForACK { get => _waittingForACK; set => _waittingForACK = value; }

        private IOrchestrator? _orchestrator;

        public FsmNode CurrentState => _nodesById[_currentStateId];

        public FsmController(FsmNode root, NodeId initialId)
        {
            _navigator = new FlowNavigator(root);

            // Index for quick access to node instances
            _nodesById = new Dictionary<NodeId, FsmNode>();
            Flatten(root);

            _currentStateId = initialId;
            if (_nodesById.TryGetValue(initialId, out var node) && node is BranchNode branch)
            {
                var leaf = FindFirstLeaf(branch);
                if (leaf != null)
                {
                    _currentStateId = leaf.Id;
                }
            }
        }

        private FsmNode? FindFirstLeaf(BranchNode branch)
        {
            foreach (var child in branch.Children)
            {
                if (child is LeafNode leaf) return leaf;
                if (child is BranchNode b) 
                {
                    var found = FindFirstLeaf(b);
                    if (found != null) return found;
                }
            }
            return null;
        }

        public void SetOrchestrator(IOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        private IGameLogger? _logger;

        public void SetLogger(IGameLogger logger)
        {
            _logger = logger;
        }

        // Traverse the node tree and index them for quick access
        private void Flatten(FsmNode node)
        {
            _nodesById[node.Id] = node;
            if (node is BranchNode branch)
            {
                foreach (var child in branch.Children) Flatten(child);
            }
        }

        /// <summary>
        /// Requests to advance to the next logical state in the tree.
        /// </summary>
        public FsmStepResult MoveForwardRequest(GameState currentState)
        {
            var nextId = _navigator.GetNextStateId(_currentStateId);

            if (nextId == null)
            {
                // Here a Game Over event could be triggered
                return new FsmStepResult(currentState, false, []);
            }
            return ExecuteTransition(_currentStateId, nextId.Value, currentState);
        }

        // Executes a transition between two states
        private FsmStepResult ExecuteTransition(NodeId fromId, NodeId toId, GameState state)
        {
            var fromNode = _nodesById[fromId];
            var toNode = _nodesById[toId];
            var effects = new List<IGameEffect>();
            var appliers = new List<IFsmApplier>();

            // Sync Orchestrator
            _orchestrator?.SetState(state);

            // 1. Execute closure of the current node

            // Orchestrator Trigger: OnEnd State
            if (_orchestrator != null && !string.IsNullOrEmpty(fromNode.Name))
            {
                _orchestrator.ExecuteScheduled(fromNode.Name, "OnEnd");
                state = _orchestrator.CurrentState;
            }


            // Legacy Appliers
            var appliersOnEnd = fromNode.OnEnd(state);
            foreach (var applier in appliersOnEnd)
            {
                var applierResponse = applier.Apply(state);
                state = applierResponse.GameState;
                effects.AddRange(applierResponse.GameEffects);
            }

            // 2. Execute opening of the new node

            // Sync Orchestrator again if legacy changed state
            _orchestrator?.SetState(state);

            // Orchestrator Trigger: OnStart State
            if (_orchestrator != null && !string.IsNullOrEmpty(toNode.Name))
            {
                _orchestrator.ExecuteScheduled(toNode.Name, "OnStart");
                state = _orchestrator.CurrentState;
            }

            // Legacy Appliers
            var appliersOnStart = toNode.OnStart(state);
            foreach (var applier in appliersOnStart)
            {
                var applierResponse = applier.Apply(state);
                state = applierResponse.GameState;
                effects.AddRange(applierResponse.GameEffects);
            }

            // 3. Internal applier to update the CurrentStateId pointer in GameState
            var changeStateApplierResponse = new ChangeStateApplier(toId).Apply(state);
            state = changeStateApplierResponse.GameState;
            effects.AddRange(changeStateApplierResponse.GameEffects);

            // Update local controller reference
            _currentStateId = toId;

            // Final Sync
            _orchestrator?.SetState(state);

            return new FsmStepResult(state, false, effects);
        }

        /// <summary>
        /// Delegates the command to the current leaf node if it is valid.
        /// </summary>
        public FsmStepResult HandleCommand(ICommand command, GameState state, CommandResult result)
        {
            _logger?.Log($"[FsmController] HandleCommand: {command.GetType().Name} - Result: Success={result.Success}, Tags={string.Join(",", result.Tags ?? [])}");

            // Sync Orchestrator
            _orchestrator?.SetState(state);
            var effects = new List<IGameEffect>();
            if (_nodesById[_currentStateId] is LeafNode leaf)
            {
                
                var appliersOnCommand = leaf.OnCommandExecuted(command, result, out bool transitionRequested);
                // Legacy
                foreach (var applier in appliersOnCommand)
                {
                    var response = applier.Apply(state);
                    state = response.GameState;
                    effects.AddRange(response.GameEffects);
                }

                // Orchestrator Trigger: OnCommandExecutionEnd
                if (_orchestrator != null)
                {
                    _orchestrator.SetState(state); // Sync if legacy changed it
                    effects.AddRange(_orchestrator.ExecuteScheduled(null, "OnCommandExecutionEnd"));
                    _logger?.Log("[FsmController] Orchestrator OnCommandExecutionEnd executed effects size" + effects.Count);
                    state = _orchestrator.CurrentState;
                }

                return new FsmStepResult(state, transitionRequested, effects);
            }
            return new FsmStepResult(state, false, []);
        }
    }

    public record FsmStepResult(GameState State, bool TransitionRequested, IReadOnlyList<IGameEffect> Effects);
}
