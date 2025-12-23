using System.Reflection;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Core;

/// <summary>
/// CommandBus routes commands to their handlers.
/// Command validation is handled by FSM (FsmController.IsCommandAllowed).
/// </summary>
public sealed class CommandBus(ICommandHandlerResolver handlerResolver)
{
    private bool _waitingForAck;

    public CommandResult Send(ICommand command)
    {
        try
        {
            if (_waitingForAck)
                throw new InvalidOperationException("Waiting for ACK");
            
            // Command validation is done by FSM before reaching here
            // Execute handler
            var result = DispatchToHandler(command);
            
            return result;
        }
        catch (Exception ex)
        {
            return CommandResult.Fail(ex.Message);
        }
    }

    private CommandResult DispatchToHandler(ICommand command)
    {
        // Dispatch genérico controlado
        var method = typeof(CommandBus)
            .GetMethod(
                nameof(DispatchGeneric),
                BindingFlags.Instance | BindingFlags.NonPublic)!;

        var generic = method.MakeGenericMethod(command.GetType());
        return (CommandResult)generic.Invoke(this, new object[] { command })!;
    }

    private CommandResult DispatchGeneric<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        var handler = handlerResolver.Resolve<TCommand>();
        return handler.Handle(command);
    }

    public void Acknowledge()
    {
        _waitingForAck = false;
    }
}