using System;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Infrastructure.Registration
{
    public class GameFlowBuilder
    {
        private FsmNode? _root = null;

        public GameFlowBuilder AddRoot<T>(string name, Action<BranchBuilder>? configure = null) where T : BranchNode, new()
        {
            var root = new T
            {
                Id = NodeId.New(),
                Name = name
            };

            _root = root;

            if (configure != null)
            {
                var builder = new BranchBuilder(root);
                configure(builder);
            }
            return this;
        }

        public FsmNode Build()
        {
            if (_root == null)
                throw new InvalidOperationException("FSM Error: Se debe definir un nodo Raíz (Root) para el flujo.");

            // Create System Wrapper
            var systemRoot = new TurnForge.Engine.Core.Fsm.SystemNodes.SystemRootNode
            {
                Id = NodeId.New(),
                Name = "SystemRoot"
            };

            var initial = new TurnForge.Engine.Core.Fsm.SystemNodes.InitialStateNode
            {
                Id = NodeId.New(),
                Name = "InitialState"
            };

            var prepared = new TurnForge.Engine.Core.Fsm.SystemNodes.GamePreparedNode
            {
                Id = NodeId.New(),
                Name = "GamePrepared"
            };

            // Link Hierarchy (Parent/Children)
            systemRoot.Children.Add(initial);
            systemRoot.Children.Add(prepared);
            systemRoot.Children.Add(_root);

            initial.Parent = systemRoot;
            prepared.Parent = systemRoot;
            _root.Parent = systemRoot;

            // Link Navigation (FirstChild/Siblings)
            systemRoot.FirstChild = initial;
            initial.NextSibling = prepared;
            prepared.NextSibling = _root;
            _root.NextSibling = null; // Encapsulation: User root is last.

            return systemRoot;
        }
    }
}