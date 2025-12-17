namespace TurnForge.Engine.Commands.Interfaces;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    CommandResult Handle(TCommand command);
}
