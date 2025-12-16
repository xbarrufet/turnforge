using System.Reflection;
using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class CommandBus
{
    private readonly IGameLoop _gameLoop;
    private readonly ICommandHandlerResolver _handlerResolver;
    private readonly EventBus _eventBus;

    private bool _waitingForAck;

    public CommandBus(
        IGameLoop gameLoop,
        ICommandHandlerResolver handlerResolver,
        EventBus eventBus)
    {
        _gameLoop = gameLoop;
        _handlerResolver = handlerResolver;
        _eventBus = eventBus;
    }

    public void Send(ICommand command)
    {
        if (_waitingForAck)
            throw new InvalidOperationException("Waiting for ACK");

        // 1️⃣ Validación del GameLoop (turnos, estado, etc.)
        var loopResult = _gameLoop.Validate(command);

        if (!loopResult.IsAllowed)
            throw new InvalidOperationException(loopResult.Reason);

        // 2️⃣ Ejecutar el handler correspondiente
        DispatchToHandler(command);

        // 3️⃣ Publicar resultado de dominio (si existe)
        if (loopResult.DomainResult is not null)
            _eventBus.Publish(loopResult.DomainResult);

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
        var handler = _handlerResolver.Resolve<TCommand>();
        handler.Handle(command);
    }

    public void Acknowledge()
    {
        _waitingForAck = false;
    }
}