namespace TurnForge.Engine.Infrastructure;

using TurnForge.Engine.Commands.Interfaces;


public sealed class ServiceProviderCommandHandlerResolver
    : ICommandHandlerResolver
{
    private readonly IServiceProvider _provider;

    public ServiceProviderCommandHandlerResolver(IServiceProvider provider)
    {
        _provider = provider;
    }

    public ICommandHandler<TCommand> Resolve<TCommand>()
        where TCommand : ICommand
    {
        return (ICommandHandler<TCommand>)
            _provider.GetService(typeof(ICommandHandler<TCommand>))!;
    }
}
