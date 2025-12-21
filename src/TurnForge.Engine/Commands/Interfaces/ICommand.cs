namespace TurnForge.Engine.Commands.Interfaces;

public interface ICommand
{
    Type CommandType { get; }
}