using System.Collections.Generic;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Core.Interfaces;

public interface IOrchestrator
{
    void SetState(global::TurnForge.Engine.Entities.GameState state);
    global::TurnForge.Engine.Entities.GameState CurrentState { get; }
    IEnumerable<IGameEvent> ExecuteScheduled(object? context, string trigger);

}
    
