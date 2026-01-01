using System;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Action.Interfaces;

using TurnForge.Engine.Commands.ValueObjects;

namespace TurnForge.Engine.Commands;

public class ActionInputCommand : ICommand
{
    public IActionInput Input { get; }
    
    // Commands typically need to identify the related entity/context, 
    // but for active workflow resumption, the context is implied by the engine state.
    // We might add ActionId for safety later.
    
    public ActionInputCommand(IActionInput input)
    {
        Input = input;
    }

    public CommandType CommandType => new CommandType("ActionInput");
}
