using System;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Workflow.Interfaces;

using TurnForge.Engine.Commands.ValueObjects;

namespace TurnForge.Engine.Commands;

public class WorkflowInputCommand : ICommand
{
    public IWorkflowInput Input { get; }
    
    // Commands typically need to identify the related entity/context, 
    // but for active workflow resumption, the context is implied by the engine state.
    // We might add WorkflowId for safety later.
    
    public WorkflowInputCommand(IWorkflowInput input)
    {
        Input = input;
    }

    public CommandType CommandType => new CommandType("WorkflowInput");
}
