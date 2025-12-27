using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Appliers.Entity;

public class ChangeStateApplier : IFsmApplier
{
    private readonly NodeId _newStateId;

    public ChangeStateApplier(NodeId newStateId)
    {
        _newStateId = newStateId;
    }

    public ApplierResponse Apply(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state), "GameState cannot be null when applying ChangeStateApplier.");
        
        return new ApplierResponse(state.WithCurrentStateId(_newStateId), Array.Empty<IGameEvent>());
    }
}
