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
using TurnForge.Engine.Orchestrator.Interfaces;
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
            _currentStateId = initialId;

            // Indexamos para acceso rápido a las instancias de los nodos
            _nodesById = new Dictionary<NodeId, FsmNode>();
            Flatten(root);
        }

        public void SetOrchestrator(IOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        // Recorre el árbol de nodos y los indexa para acceso rápido
        private void Flatten(FsmNode node)
        {
            _nodesById[node.Id] = node;
            if (node is BranchNode branch)
            {
                foreach (var child in branch.Children) Flatten(child);
            }
        }

        /// <summary>
        /// Solicita avanzar al siguiente estado lógico en el árbol.
        /// </summary>
        public FsmStepResult MoveForwardRequest(GameState currentState, IEffectSink effectSink)
        {
            var nextId = _navigator.GetNextStateId(_currentStateId);

            if (nextId == null)
            {
                // Aquí se podría disparar un evento de Fin de Juego
                return new FsmStepResult(currentState, false, []);
            }
            return ExecuteTransition(_currentStateId, nextId.Value, currentState, effectSink);
        }

        // Ejecuta una transición entre dos estados
        private FsmStepResult ExecuteTransition(NodeId fromId, NodeId toId, GameState state, IEffectSink effectSink)
        {
            var fromNode = _nodesById[fromId];
            var toNode = _nodesById[toId];
            var effects = new List<IGameEffect>();
            var appliers = new List<IFsmApplier>();

            // Sync Orchestrator
            _orchestrator?.SetState(state);

            // 1. Ejecutamos el cierre del nodo actual

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

            // 2. Ejecutamos la apertura del nuevo nodo

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

            // 3. Applier interno para actualizar el puntero CurrentStateId en el GameState
            var changeStateApplierResponse = new ChangeStateApplier(toId).Apply(state);
            state = changeStateApplierResponse.GameState;
            effects.AddRange(changeStateApplierResponse.GameEffects);

            // Actualizamos la referencia local del controlador
            _currentStateId = toId;

            // Final Sync
            _orchestrator?.SetState(state);

            return new FsmStepResult(state, false, effects);
        }

        /// <summary>
        /// Delega el comando al nodo hoja actual si es válido.
        /// </summary>
        public FsmStepResult HandleCommand(ICommand command, GameState state, CommandResult result)
        {
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
                    _orchestrator.ExecuteScheduled(null, "OnCommandExecutionEnd");
                    state = _orchestrator.CurrentState;
                }

                return new FsmStepResult(state, transitionRequested, effects);
            }
            return new FsmStepResult(state, false, []);
        }
    }

    public record FsmStepResult(GameState State, bool TransitionRequested, IReadOnlyList<IGameEffect> Effects);
}
