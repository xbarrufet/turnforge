using TurnForge.Engine.Commands.ValueObjects;

namespace TurnForge.Engine.Commands.Interfaces;

public interface ICommand
{
    CommandType CommandType { get; }
}