namespace TurnForge.Engine.Core.Action.Interfaces;

public interface IAcceptsInput { }

public interface IAcceptsInput<in TInput> : IAcceptsInput
        where TInput : IActionInput
    {
        void MoveForward(
            ActionContext context,
            TInput input);
    }