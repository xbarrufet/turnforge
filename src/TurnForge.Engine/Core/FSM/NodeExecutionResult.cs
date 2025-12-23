using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Core.Fsm;

public struct NodeExecutionResult
{
    public IEnumerable<IFsmApplier> Decisions { get; }
    public ICommand? CommandToLaunch { get; }

    public NodeExecutionResult(IEnumerable<IFsmApplier> decisions, ICommand? commandToLaunch = null)
    {
        Decisions = decisions ?? Enumerable.Empty<IFsmApplier>();
        CommandToLaunch = commandToLaunch;
    }

    public static NodeExecutionResult Empty() => new(Enumerable.Empty<IFsmApplier>());
    
    public static NodeExecutionResult WithDecisions(IEnumerable<IFsmApplier> decisions) => new(decisions);
    public static NodeExecutionResult WithDecisions(params IFsmApplier[] decisions) => new(decisions);
    
    public static NodeExecutionResult LaunchCommand(ICommand command) => new(Enumerable.Empty<IFsmApplier>(), command);
}
