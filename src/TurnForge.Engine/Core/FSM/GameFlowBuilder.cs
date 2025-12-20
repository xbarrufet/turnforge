using System;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Infrastructure.Registration
{
    public class GameFlowBuilder
    {
        private FsmNode _root;

        public GameFlowBuilder AddRoot<T>(string name, Action<BranchBuilder> configure = null) where T : BranchNode, new()
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

            return _root;
        }
    }
}