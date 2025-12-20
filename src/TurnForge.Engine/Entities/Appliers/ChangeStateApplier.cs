
using System.Collections.Generic;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Appliers;

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
