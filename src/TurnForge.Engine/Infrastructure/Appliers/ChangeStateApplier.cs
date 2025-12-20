
using System.Collections.Generic;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Infrastructure.Appliers.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Infrastructure.Appliers
{
    public class ChangeStateApplier : IFsmApplier
    {
        private readonly NodeId _newStateId;

        public ChangeStateApplier(NodeId newStateId)
        {
            _newStateId = newStateId;
        }

        public GameState Apply(GameState state, IEffectSink effectSink)
        {
            return state.WithCurrentStateId(_newStateId);
        }
    }
}
