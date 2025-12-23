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

        public GameFlowBuilder AddRoot<T>(string name, Action<BranchBuilder>? configure = null) where T : FsmNode, new()
        {
            var root = new T
            {
                Id = NodeId.New(),
                Name = name
            };

            _userSequence.Add(root);

            if (configure != null)
            {
                var builder = new BranchBuilder(_userSequence);
                configure(builder);
            }
            return this;
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

            var prepared = new TurnForge.Engine.Core.Fsm.SystemNodes.GamePreparedNode
            {
                Id = NodeId.New(),
                Name = "GamePrepared"
            };

            finalSequence.Add(initial);
            finalSequence.Add(boardReady);
            finalSequence.Add(prepared);

            // 2. User Defined Logic
            if (_userSequence.Count == 0)
            {
                throw new InvalidOperationException("FSM Error: Sequence is empty. Add at least one user node.");
            }
            finalSequence.AddRange(_userSequence);

            return finalSequence;
        }
    }
}