using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.FSM;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Infrastructure.Registration
{
    public class GameFlowBuilder
    {
        private readonly List<FsmNode> _userSequence = new();

        public GameFlowBuilder AddNode<T>(string name, Action<NodeFlowBuilder>? configure = null) where T : FsmNode, new()
        {
            var node = new T
            {
                Id = NodeId.New(),
                Name = name
            };

            _userSequence.Add(node);

            // Optional config if user still wants to nest logically, though it's flat
            if (configure != null)
            {
                var builder = new NodeFlowBuilder(_userSequence);
                configure(builder);
            }
            return this;
        }

        [Obsolete("Use AddNode instead for flat FSM definition")]
        public GameFlowBuilder AddRoot<T>(string name, Action<NodeFlowBuilder>? configure = null) where T : FsmNode, new()
        {
            return AddNode<T>(name, configure);
        }

        public List<FsmNode> Build()
        {
            var finalSequence = new List<FsmNode>();

            // 1. System Initialization Nodes
            var initial = new TurnForge.Engine.Core.Fsm.SystemNodes.InitialStateNode
            {
                Id = NodeId.New(),
                Name = "InitialState"
            };

            var boardReady = new TurnForge.Engine.Core.Fsm.SystemNodes.BoardReadyNode
            {
                Id = NodeId.New(),
                Name = "BoardReady"
            };
            var gameReady = new TurnForge.Engine.Core.Fsm.SystemNodes.GamePreparedNode
            {
                Id = NodeId.New(),
                Name = "WorldReady"
            };


            finalSequence.Add(initial);
            finalSequence.Add(boardReady);
            finalSequence.Add(gameReady);
            

            // 2. User Defined Logic
            // Append user nodes sequentially.
            // If the user used AddRoot/AddBranch, they populated _userSequence.
            // We just ensure they are appended after system nodes.
            
            if (_userSequence.Count == 0)
            {
                throw new InvalidOperationException("FSM Error: Sequence is empty. Add at least one user node.");
            }
            
            finalSequence.AddRange(_userSequence);

            return finalSequence;
        }
    }
}