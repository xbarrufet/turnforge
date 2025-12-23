using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.FSM
{
    public class NodeFlowBuilder
    {
        private readonly List<FsmNode> _sequence;

        public NodeFlowBuilder(List<FsmNode> sequence)
        {
            _sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
        }

        public NodeFlowBuilder AddNode<T>(string name, Action<NodeFlowBuilder>? configure = null) where T : FsmNode, new()
        {
            var node = new T { Id = NodeId.New(), Name = name };
            _sequence.Add(node);

            if (configure != null)
            {
                configure(this);
            }
            return this;
        }
    }
}