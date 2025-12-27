using TurnForge.Engine.Core;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Decisions.Board;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;

namespace TurnForge.Engine.Commands.Board;

/// <summary>
/// Handles board initialization by creating the board via factory
/// and returning a decision to add it to state.
/// </summary>
public sealed class InitializeBoardCommandHandler : ICommandHandler<InitializeBoardCommand>
{
    private readonly IBoardFactory _boardFactory;
    private readonly IGameRepository _repository;

    public InitializeBoardCommandHandler(
        IBoardFactory boardFactory,
        IGameRepository repository)
    {
        _boardFactory = boardFactory ?? throw new ArgumentNullException(nameof(boardFactory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public CommandResult Handle(InitializeBoardCommand command)
    {
        // 1. Load current state (for validation if needed)
        var currentState = _repository.LoadGameState();

        // 2. Build board from descriptor using factory
        var board = _boardFactory.Build(command.Descriptor);

        // 3. Create decision to add board to state
        var decision = new InitializeBoardDecision(board);

        // 4. Return successful result with decision and tag
        return CommandResult.Ok(
            decisions: new[] { decision },
            tags: "BoardInitialized"
        );
    }
}
