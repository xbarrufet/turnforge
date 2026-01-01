using System;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Interfaces;

public interface IActionOrchestrator
{
    void StartAction(IAction workflow, ActionContext context);
    void SubmitInput(Guid workflowId, IActionInput input);
}
