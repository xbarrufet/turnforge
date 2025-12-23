using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.FSM
{
    public class BranchBuilder
    {
        private readonly List<FsmNode> _sequence;

        public BranchBuilder(List<FsmNode> sequence)
        {
            _sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
        }

        public BranchBuilder AddLeaf<T>(string name) where T : FsmNode, new()
        {
            var node = new T { Id = NodeId.New(), Name = name };
            _sequence.Add(node);
            return this;
        }
        
        // Alias for AddLeaf - since differentiation is removed
        public BranchBuilder AddNode<T>(string name) where T : FsmNode, new()
        {
            return AddLeaf<T>(name);
        }

        public BranchBuilder AddBranch<T>(string name, Action<BranchBuilder>? configure = null) where T : FsmNode, new()
        {
            var node = new T { Id = NodeId.New(), Name = name };
            _sequence.Add(node);

            if (configure != null)
            {
                // Pass the same sequence to flatten children into the main list
                // Order: BranchNode (Container) -> Child1 -> Child2
                configure(this);
            }
            return this;
        }
    }
}