using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace TurnForge.Engine.Core.Orchestrator;

public sealed class CommandTransaction(ICommand command)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public ICommand Command { get; init; } = command;
    public CommandResult Result { get; set; } = CommandResult.Fail("Unknown error");
    public IGameEffect[] Effects { get; set; } = [];

}