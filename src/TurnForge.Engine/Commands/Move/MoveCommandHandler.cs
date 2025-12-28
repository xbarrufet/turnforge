using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Move.Interfaces;

namespace TurnForge.Engine.Commands.Move;

/// <summary>
/// Handles movement commands by delegating to an injected strategy.
/// This allows game-specific logic (e.g., traps, stamina costs) to intervene.
/// </summary>
public sealed class MoveCommandHandler : ICommandHandler<MoveCommand>
{
    private readonly IMoveStrategy _strategy;
    private readonly IGameRepository _repository;

    public MoveCommandHandler(IMoveStrategy strategy, IGameRepository repository)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public CommandResult Handle(MoveCommand command)
    {
        var state = _repository.LoadGameState();
        var decisions = _strategy.Process(command, state);

        return CommandResult.Ok(
            decisions: decisions.ToArray(),
            tags: "Moved"
        );
    }
}
