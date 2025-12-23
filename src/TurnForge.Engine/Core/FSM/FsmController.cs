using System;
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity;
using TurnForge.Engine.Core.Orchestrator.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace TurnForge.Engine.Core.Fsm
{
    public class FsmController
    {
        private readonly List<FsmNode> _sequence;
        private readonly Dictionary<NodeId, FsmNode> _nodesById;
        private int _currentIndex;
        private NodeId _currentStateId;
        private bool _waittingForACK;
        private IOrchestrator? _orchestrator;
        private IGameLogger? _logger;

        public NodeId CurrentStateId => _currentStateId;
        public FsmNode CurrentNode => _sequence[_currentIndex];
        public bool WaittingForACK { get => _waittingForACK; set => _waittingForACK = value; }

        public FsmController(IEnumerable<FsmNode> sequence, NodeId? initialId = null)
        {
            _sequence = sequence.ToList();
            if (!_sequence.Any()) throw new ArgumentException("FSM Sequence cannot be empty");

            _nodesById = _sequence.ToDictionary(n => n.Id);

            _currentIndex = 0;
            if (initialId != null)
            {
                var idx = _sequence.FindIndex(n => n.Id == initialId.Value);
                if (idx != -1) _currentIndex = idx;
            }
            
            _currentStateId = _sequence[_currentIndex].Id;
        }

        public void SetOrchestrator(IOrchestrator orchestrator) => _orchestrator = orchestrator;
        public void SetLogger(IGameLogger logger) => _logger = logger;

        /// <summary>
        /// Main game loop processor. 
        /// Executes the current node logic and advances if completed.
        /// Can be called after a command execution, or manually to tick the FSM.
        /// </summary>
        public FsmStepResult ProcessFlow(GameState state)
        {
            var currentState = state;
            var accumulatedEffects = new List<IGameEffect>();
            
            int loopGuard = 0;
            const int MaxLoopIterations = 100;

            while (loopGuard++ < MaxLoopIterations)
            {
                var node = CurrentNode;
                
                // 1. Sync & Execute Node Logic
                _orchestrator?.SetState(currentState);
                if (_orchestrator != null && !string.IsNullOrEmpty(node.Name))
                {
                    // Optional: Hooks for specific node entry logic handled by orchestrator? 
                    // Keeping simple for now. 
                    // _orchestrator.ExecuteScheduled(node.Name, "OnEnter"); 
                }

                var execResult = node.Execute(currentState);
                
                // 2. Apply Decisions
                if (execResult.Decisions != null)
                {
                    foreach (var applier in execResult.Decisions)
                    {
                        var response = applier.Apply(currentState);
                        currentState = response.GameState;
                        accumulatedEffects.AddRange(response.GameEffects);
                    }
                }

                // 3. Check for Auto-Command Launch
                if (execResult.CommandToLaunch != null)
                {
                    _orchestrator?.SetState(currentState);
                    return new FsmStepResult(currentState, false, accumulatedEffects, execResult.CommandToLaunch, execResult.IsGameOver);
                }
                
                // 4 Check Game Over
                if (execResult.IsGameOver || node.IsGameOver(currentState))
                {
                     _logger?.Log($"[FsmController] Game Over detected at node {node.Name}");
                     _orchestrator?.SetState(currentState);
                     return new FsmStepResult(currentState, false, accumulatedEffects, null, true);
                }

                // 5. Check for Completion (Transition)
                if (node.IsCompleted(currentState))
                {
                    // Move Next
                    if (_currentIndex + 1 >= _sequence.Count)
                    {
                        // End of Sequence. Stop? Or Loop? 
                        // For now: Stop. 
                        _logger?.Log("[FsmController] End of FSM Sequence reached.");
                        _orchestrator?.SetState(currentState);
                        return new FsmStepResult(currentState, false, accumulatedEffects);
                    }

                    _currentIndex++;
                    var nextNode = _sequence[_currentIndex];
                    var prevNodeId = _currentStateId;
                    
                    // Update Pointers
                    _currentStateId = nextNode.Id;
                    
                    // Transition Effect (ChangeStateApplier) - Important for persistence
                    var changeStateResponse = new ChangeStateApplier(_currentStateId).Apply(currentState);
                    currentState = changeStateResponse.GameState;
                    accumulatedEffects.AddRange(changeStateResponse.GameEffects);
                    
                    // Continue Loop (Process next node immediately)
                    continue;
                }
                else
                {
                    // Node is NOT completed (e.g. Waiting for user input). Stop loop.
                    _orchestrator?.SetState(currentState);
                    return new FsmStepResult(currentState, false, accumulatedEffects);
                }
            }
            
            _logger?.LogError($"[FsmController] Infinite loop detected in flat sequence after {MaxLoopIterations} nodes.");
            return new FsmStepResult(currentState, false, accumulatedEffects);
        }

        // Logic wrapper for external calls (like GameLoop tick)
        public FsmStepResult MoveForwardRequest(GameState currentState)
        {
            return ProcessFlow(currentState);
        }

        public FsmStepResult HandleCommand(ICommand command, GameState state, CommandResult result)
        {
            // Sync Orchestrator
            _orchestrator?.SetState(state);
            var effects = new List<IGameEffect>();
            
            var node = CurrentNode;
            
            // Allow node to react (e.g. LeafNode applying logic)
            var execResult = node.OnCommandExecuted(command, result);
            
            // Apply React Effects
                if (execResult.Decisions != null)
                {
                   foreach (var applier in execResult.Decisions)
                   {
                        var response = applier.Apply(state);
                        state = response.GameState;
                        effects.AddRange(response.GameEffects);
                   }
                }
                
                if (execResult.IsGameOver || node.IsGameOver(state))
                {
                     // Return immediately with Game Over
                     return new FsmStepResult(state, false, effects, null, true);
                }
            
                // Check flow again (State changed => Node might be completed now)
                var flowResult = ProcessFlow(state);
            
            var finalEffects = new List<IGameEffect>(effects);
            finalEffects.AddRange(flowResult.Effects);
            
            return flowResult with { Effects = finalEffects };
        }
    }
    
    public record FsmStepResult(GameState State, bool TransitionRequested, IReadOnlyList<IGameEffect> Effects, ICommand? CommandToLaunch = null, bool IsGameOver = false);
}
