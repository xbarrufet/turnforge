using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;

namespace TurnForge.Engine.Core.Fsm;

public struct NodeExecutionResult
{
    public IEnumerable<IFsmApplier> Decisions { get; }
    public ICommand? CommandToLaunch { get; }
    public IWorkflow? WorkflowToLaunch { get; }
    public WorkflowContext? InitialContext { get; }
    public bool IsGameOver { get; }

    public NodeExecutionResult(
        IEnumerable<IFsmApplier> decisions, 
        ICommand? commandToLaunch = null, 
        IWorkflow? workflowToLaunch = null,
        WorkflowContext? initialContext = null,
        bool isGameOver = false)
    {
        Decisions = decisions ?? Enumerable.Empty<IFsmApplier>();
        CommandToLaunch = commandToLaunch;
        WorkflowToLaunch = workflowToLaunch;
        InitialContext = initialContext;
        IsGameOver = isGameOver;
    }

    public static NodeExecutionResult Empty() => new(Enumerable.Empty<IFsmApplier>());
    
    public static NodeExecutionResult WithDecisions(IEnumerable<IFsmApplier> decisions) => new(decisions);
    public static NodeExecutionResult WithDecisions(params IFsmApplier[] decisions) => new(decisions);
    
    public static NodeExecutionResult LaunchCommand(ICommand command) => new(Enumerable.Empty<IFsmApplier>(), command);
    
    public static NodeExecutionResult LaunchWorkflow(IWorkflow workflow, WorkflowContext context) 
        => new(Enumerable.Empty<IFsmApplier>(), null, workflow, context);
    
    public static NodeExecutionResult GameOver() => new(Enumerable.Empty<IFsmApplier>(), null, null, null, true);
}
