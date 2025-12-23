using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Core.Fsm;

public struct NodeExecutionResult
{
    public IEnumerable<IFsmApplier> Decisions { get; }
    public ICommand? CommandToLaunch { get; }
    public bool IsGameOver { get; }

    public NodeExecutionResult(IEnumerable<IFsmApplier> decisions, ICommand? commandToLaunch = null, bool isGameOver = false)
    {
        Decisions = decisions ?? Enumerable.Empty<IFsmApplier>();
        CommandToLaunch = commandToLaunch;
        IsGameOver = isGameOver;
    }

    public static NodeExecutionResult Empty() => new(Enumerable.Empty<IFsmApplier>());
    
    public static NodeExecutionResult WithDecisions(IEnumerable<IFsmApplier> decisions) => new(decisions);
    public static NodeExecutionResult WithDecisions(params IFsmApplier[] decisions) => new(decisions);
    
    public static NodeExecutionResult LaunchCommand(ICommand command) => new(Enumerable.Empty<IFsmApplier>(), command);
    
    public static NodeExecutionResult GameOver() => new(Enumerable.Empty<IFsmApplier>(), null, true);
}
