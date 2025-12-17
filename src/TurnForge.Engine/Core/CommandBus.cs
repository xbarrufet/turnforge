using System.Reflection;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class CommandBus(
    IGameLoop gameLoop,
    ICommandHandlerResolver handlerResolver,
    IEffectSink effectSink)
{

    private bool _waitingForAck;

    public void Send(ICommand command)
    {
        if (_waitingForAck)
            throw new InvalidOperationException("Waiting for ACK");

        // 1️⃣ Validación del GameLoop (turnos, estado, etc.)
        var loopResult = gameLoop.Validate(command);

        if (!loopResult.IsAllowed)
            throw new InvalidOperationException(loopResult.Reason);

        // 2️⃣ Ejecutar el handler correspondiente
        DispatchToHandler(command);
        
        // 4️⃣ Gestión de ACK
        _waitingForAck = loopResult.RequiresAck;
    }

    private void DispatchToHandler(ICommand command)
    {
        // Dispatch genérico controlado
        var method = typeof(CommandBus)
            .GetMethod(
                nameof(DispatchGeneric),
                BindingFlags.Instance | BindingFlags.NonPublic)!;

        var generic = method.MakeGenericMethod(command.GetType());
        generic.Invoke(this, new object[] { command });
    }

    private void DispatchGeneric<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        var handler = handlerResolver.Resolve<TCommand>();
        handler.Handle(command);
    }

    public void Acknowledge()
    {
        _waitingForAck = false;
    }
}