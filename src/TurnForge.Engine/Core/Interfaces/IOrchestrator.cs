using System.Collections.Generic;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Decisions;

namespace TurnForge.Engine.Core.Interfaces;

public interface IOrchestrator
{
    void SetState(global::TurnForge.Engine.Entities.GameState state);
    void Enqueue(IEnumerable<IDecision> decisions);
    global::TurnForge.Engine.Entities.GameState CurrentState { get; }
    IEnumerable<IGameEvent> ExecuteScheduled(object? context, string trigger);
    IEnumerable<IGameEvent> Apply(IDecision decision);
}
