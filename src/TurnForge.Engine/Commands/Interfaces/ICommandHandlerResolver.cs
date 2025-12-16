namespace TurnForge.Engine.Commands.Interfaces;

public interface ICommandHandlerResolver
{
    ICommandHandler<TCommand> Resolve<TCommand>()
        where TCommand : ICommand;
}