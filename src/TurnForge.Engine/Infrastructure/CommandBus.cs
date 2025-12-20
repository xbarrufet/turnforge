using System.Reflection;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class CommandBus(
    IGameLoop gameLoop,
    ICommandHandlerResolver handlerResolver,
    IEffectSink effectSink)
{

    private bool _waitingForAck;

    public CommandResult Send(ICommand command)
    {
        try
        {
            if (_waitingForAck)
                throw new InvalidOperationException("Waiting for ACK");
            // 1️⃣ Validación del GameLoop (turnos, estado, etc.)
            var loopResult = gameLoop.Validate(command);
            if (!loopResult.IsAllowed)
                throw new InvalidOperationException(loopResult.Reason);
            // 2️⃣ Ejecutar el handler correspondiente
            // 2️⃣ Ejecutar el handler correspondiente
            var result = DispatchToHandler(command);
            // 4️⃣ Gestión de ACK
            _waitingForAck = loopResult.RequiresAck;
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