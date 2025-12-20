using System;
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Infrastructure.Appliers;
using TurnForge.Engine.Infrastructure.Appliers.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm
{
    public class FsmController
    {
        private readonly FlowNavigator _navigator;
        private readonly Dictionary<NodeId, FsmNode> _nodesById;
        private NodeId _currentStateId;

        // Evento para que el Engine sepa que debe aplicar cambios al GameState global
        public event Action<IEnumerable<IFsmApplier>> OnTransitionExecuted;
        public NodeId CurrentStateId => _currentStateId;


        public FsmController(FsmNode root, NodeId initialId)
        {
            _navigator = new FlowNavigator(root);
            _currentStateId = initialId;

            // Indexamos para acceso rápido a las instancias de los nodos
            _nodesById = new Dictionary<NodeId, FsmNode>();
            Flatten(root);
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
        public GameState MoveForwardRequest(GameState currentState, IEffectSink effectSink)
        {
            var nextId = _navigator.GetNextStateId(_currentStateId);

            if (nextId == null)
            {
                // Aquí se podría disparar un evento de Fin de Juego
                return currentState;
            }

            return ExecuteTransition(_currentStateId, nextId.Value, currentState, effectSink);
        }


        // Ejecuta una transición entre dos estados
        private GameState ExecuteTransition(NodeId fromId, NodeId toId, GameState state, IEffectSink effectSink)
        {
            var fromNode = _nodesById[fromId];
            var toNode = _nodesById[toId];

            var appliers = new List<IFsmApplier>();

            // 1. Ejecutamos el cierre del nodo actual y aplicamos los appliers
            var appliersOnEnd = fromNode.OnEnd(state);
            foreach (var applier in appliersOnEnd)
            {
                state = applier.Apply(state, effectSink);
            }

            // 2. Ejecutamos la apertura del nuevo nodo
            var appliersOnStart = toNode.OnStart(state);
            foreach (var applier in appliersOnStart)
            {
                state = applier.Apply(state, effectSink);
            }

            // 3. Applier interno para actualizar el puntero CurrentStateId en el GameState
            state = new ChangeStateApplier(toId).Apply(state, effectSink);

            // Actualizamos la referencia local del controlador
            _currentStateId = toId;
            return state;
        }

        /// <summary>
        /// Delega el comando al nodo hoja actual si es válido.
        /// </summary>
        public FsmStepResult HandleCommand(ICommand command, GameState state, CommandResult result,
            IEffectSink effectSink)
        {
            if (_nodesById[_currentStateId] is LeafNode leaf)
            {
                var appliersOnCommand = leaf.OnCommandExecuted(command, result, out bool transitionRequested);
                foreach (var applier in appliersOnCommand)
                {
                    state = applier.Apply(state, effectSink);
                }

                return new FsmStepResult(state, transitionRequested);
            }

            return new FsmStepResult(state, false);
        }
    }

    public record FsmStepResult(GameState State, bool TransitionRequested);
}
