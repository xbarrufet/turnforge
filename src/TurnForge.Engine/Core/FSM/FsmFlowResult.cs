using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Core.Fsm;

/// <summary>
/// Result of FSM ProcessFlow operation.
/// </summary>
public record FsmFlowResult(
    Entities.GameState State,
    IReadOnlyList<IGameEvent> Events,
    bool IsGameOver
)
{
    public static FsmFlowResult NoChange(Entities.GameState state) 
        => new(state, Array.Empty<IGameEvent>(), false);
}

