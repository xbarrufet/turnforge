using System;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Infrastructure.Appliers.Interfaces;

namespace TurnForge.Engine.Core.Fsm.Interfaces;

public abstract class LeafNode : FsmNode
{
    public abstract bool IsCommandValid(ICommand command, GameState state);
    public abstract IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested);
}

