using System;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Workflow.Interfaces;

namespace TurnForge.Engine.Commands;

public class WorkflowInputCommand : ICommand
{
    public IInputActionResult Input { get; }
    
    // Commands typically need to identify the related entity/context, 
    // but for active workflow resumption, the context is implied by the engine state.
    // We might add WorkflowId for safety later.
    
    public WorkflowInputCommand(IInputActionResult input)
    {
        Input = input;
    }

    public Type CommandType => GetType();
}
