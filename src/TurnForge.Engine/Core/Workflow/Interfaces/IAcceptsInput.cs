namespace TurnForge.Engine.Core.Workflow.Interfaces;

public interface IAcceptsInput { }

public interface IAcceptsInput<in TInput> : IAcceptsInput
        where TInput : IWorkflowInput
    {
        void MoveForward(
            WorkflowContext context,
            TInput input);
    }