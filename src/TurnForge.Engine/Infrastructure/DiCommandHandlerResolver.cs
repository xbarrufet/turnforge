using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class DiCommandHandlerResolver
    : ICommandHandlerResolver
{
    private readonly IServiceProvider _provider;

    public DiCommandHandlerResolver(IServiceProvider provider)
    {
        _provider = provider;
    }

    public ICommandHandler<TCommand> Resolve<TCommand>()
        where TCommand : ICommand
    {
        var handler = _provider.GetService(typeof(ICommandHandler<TCommand>));

        if (handler is null)
            throw new InvalidOperationException(
                $"No handler registered for {typeof(TCommand).Name}");

        return (ICommandHandler<TCommand>)handler;
    }
}